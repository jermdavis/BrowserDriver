using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using BrowserDriver.Browsers;
using BrowserDriver.States;

namespace BrowserDriver
{

    public class StateMachine
    {
        private State _currentState = NullState.Instance;
        private readonly BrowserConnection _connection;
        private readonly ClientWebSocket _ws = new();
        private readonly CancellationTokenSource _ct = new();
        private readonly JsonSerializerOptions _jso = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true };
        private State? _newState = null;
        private bool _running = true;
        private readonly AutoResetEvent _ar = new(initialState: false);

        public Dictionary<string, object> State { get; init; } = new Dictionary<string, object>();

        public StateMachine(State initialState, BrowserConnection connection)
        {
            _currentState = initialState;
            _connection = connection;
        }

        public async Task Start()
        {
            ArgumentNullException.ThrowIfNullOrEmpty(_connection.WebSocketDebuggerUrl);

            await _ws.ConnectAsync(new Uri(_connection.WebSocketDebuggerUrl), _ct.Token);

            var _ = Receive().
                ContinueWith(t => Console.WriteLine($">RECEIVE EXCEPTION: {t.Exception?.Message}"),
                TaskContinuationOptions.OnlyOnFaulted);

            await _currentState.Enter(this);
        }

        public async Task Wait()
        {
            _ar.WaitOne();
            _ar.Dispose();

            _running = false;
            _ct.Cancel();
            _ws.Abort();
            _ws.Dispose();
        }

        private async Task Receive()
        {
            var buffer = new byte[512];
            var sb = new StringBuilder();

            while (_running)
            {
                sb.Clear();
                var done = false;

                while (!done)
                {
                    var result = await _ws.ReceiveAsync(buffer, _ct.Token);
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    done = result.EndOfMessage;
                };

                var rr = sb.ToString();

                try
                {
                    var data = JsonSerializer.Deserialize<DebuggerResult>(rr, _jso) ?? throw new ApplicationException("Unable to deserialise incoming data");

                    if (data.Error != null)
                    {
                        var _ = Error(data).
                            ContinueWith(t => Console.WriteLine($">ERROR HANDLER EXCEPTION: {t.Exception?.Message}"),
                            TaskContinuationOptions.OnlyOnFaulted);
                    }
                    else
                    {
                        var _ = Update(data).
                            ContinueWith(t => Console.WriteLine($">UPDATE HANDLER EXCEPTION: {t.Exception?.Message}"),
                            TaskContinuationOptions.OnlyOnFaulted);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($">Error: {ex.Message}");
                }
            }
        }

        public async Task SendCommand<T>(T command, int id = 0) where T : IDebuggerCommandProperties
        {
            var cmd = new DebuggerCommand<T>(command) { Id = id };
            var data = JsonSerializer.Serialize<DebuggerCommand<T>>(cmd, _jso);
            var bytes = Encoding.UTF8.GetBytes(data);
            await _ws.SendAsync(bytes, WebSocketMessageType.Text, true, _ct.Token);
        }

        public async Task Update(DebuggerResult data)
        {
            await _currentState.Update(this, data);

            if (_newState != null)
            {
                await _currentState.Leave(this);
                _currentState = _newState;
                await _currentState.Enter(this);

                _newState = null;
            }
        }

        public async Task Error(DebuggerResult data)
        {
            Console.WriteLine($">>\nError: {data.Error?.ToJsonString(_jso)}\n>>");
        }

        public void TransitionToNewState(State state)
        {
            _newState = state;
            if (state == NullState.Instance)
            {
                _ar.Set();
            }
        }
    }

}
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
        private State? _newState = null;
        private bool _running = true;
        private readonly AutoResetEvent _waitFlag = new(initialState: false);
        private readonly byte[] _byteBuffer = new byte[512];
        private readonly StringBuilder _messageBuffer = new();

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
            _waitFlag.WaitOne();
            _waitFlag.Dispose();

            _running = false;
            _ct.Cancel();
            _ws.Abort();
            _ws.Dispose();
        }

        private async Task Receive()
        {
            while (_running)
            {
                _messageBuffer.Clear();
                var done = false;

                while (!done)
                {
                    var result = await _ws.ReceiveAsync(_byteBuffer, _ct.Token);
                    _messageBuffer.Append(Encoding.UTF8.GetString(_byteBuffer, 0, result.Count));
                    done = result.EndOfMessage;
                };

                var json = _messageBuffer.ToString();

                try
                {
                    var data = JsonSerializer.Deserialize<DebuggerResult>(json, Json.Options) ?? throw new ApplicationException("Unable to deserialise incoming data");

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
            var data = JsonSerializer.Serialize<DebuggerCommand<T>>(cmd, Json.Options);
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
            Console.WriteLine($">>\nError: {data.Error?.ToJsonString(Json.Options)}\n>>");
        }

        public void TransitionToNewState(State state)
        {
            _newState = state;
            if (state == NullState.Instance)
            {
                _waitFlag.Set();
            }
        }
    }

}
using System.Diagnostics;
using System.Text.Json;

namespace BrowserDriver.Browsers
{
    
    public class Browser : IDisposable
    {
        private static readonly HttpClient _client = new();
        private static readonly JsonSerializerOptions _jso = new() {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        public string Name { get; init; }
        public string Folder { get; init; }
        public string Executable { get; init; }
        public int DebuggerPort { get; set; } = 9222;
        public string UserFolder { get; set; }
        public string Arguments { get; set; } = "--new-window {0} --remote-debugging-port={1} --user-data-dir={2}";

        private Process? _process = null;

        public Browser(IBrowserDetector detector)
        {
            Name = detector.Name;
            Folder = detector.AppFolder;
            Executable = detector.AppExecutable;

            var td = Path.Combine(Path.GetTempPath(), $"cd-{detector.Name}");
            Console.WriteLine($"Temp directory: {td}");
            UserFolder = td;
        }

        public void Open(string url)
        {
            if (_process != null)
            {
                throw new ApplicationException("Browser process is already running.");
            }

            if (!Directory.Exists(UserFolder))
            {
                Directory.CreateDirectory(UserFolder);
            }

            var psi = new ProcessStartInfo()
            {
                WorkingDirectory = Folder,
                FileName = Executable,
                Arguments = string.Format(Arguments, url, DebuggerPort, UserFolder)
            };

            _process = Process.Start(psi);
        }

        public async Task<BrowserConnection> Connect()
        {
            var result = await _client.GetAsync($"http://localhost:{DebuggerPort}/json");
            var content = await result.Content.ReadAsStringAsync();
            var sessions = JsonSerializer.Deserialize<BrowserConnection[]>(content, _jso);

            if (sessions == null || sessions.Length < 1)
            {
                throw new ApplicationException("Did not get a valid debug session back from json endpoint");
            }

            return sessions[0];
        }

        public void Dispose()
        {
            if (_process != null)
            {
                _process.Kill();
                _process.Dispose();
            }
        }
    }

}
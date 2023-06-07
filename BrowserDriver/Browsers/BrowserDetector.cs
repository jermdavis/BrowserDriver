using Microsoft.Win32;

namespace BrowserDriver.Browsers
{
    public abstract class BrowserDetector : IBrowserDetector
    {
        public string Name { get; } = string.Empty;
        public string AppFolder { get; } = string.Empty;
        public string AppExecutable { get; init; } = string.Empty;
        public bool Installed { get; init; } = false;

        public BrowserDetector(string name, string regKey)
        {
            Name = name;

            var k = Registry.LocalMachine.OpenSubKey(regKey);

            if (k == null)
            {
                return;
            }

            var exec = k.GetValue(string.Empty) as string;
            var path = k.GetValue("Path") as string;

            if (string.IsNullOrEmpty(exec) || string.IsNullOrEmpty(path))
            {
                return;
            }

            Installed = true;
            AppExecutable = exec;
            AppFolder = path;
        }
    }

}
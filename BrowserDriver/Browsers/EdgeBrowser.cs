namespace BrowserDriver.Browsers
{

    public class EdgeBrowser : BrowserDetector
    {
        public const string RegKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\msedge.exe";

        public EdgeBrowser() : base("Edge", RegKey)
        {
        }
    }

}
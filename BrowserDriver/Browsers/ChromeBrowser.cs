using Microsoft.Win32;

namespace BrowserDriver.Browsers
{

    public class ChromeBrowser : BrowserDetector
    {
        public const string RegKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";

        public ChromeBrowser() : base("Chrome", RegKey)
        {
        }
    }

}
namespace BrowserDriver.Browsers
{

    public static class BrowserConnectionFactory
    {
        public static readonly IBrowserDetector[] Browsers = new IBrowserDetector[] { new ChromeBrowser(), new EdgeBrowser() };

        public static Browser Create()
        {
            foreach(var browser in Browsers)
            {
                if(browser.Installed)
                {
                    return new Browser(browser);
                }
            }

            throw new ApplicationException("No browser detected - unable to create an instance.");
        }
    }

}
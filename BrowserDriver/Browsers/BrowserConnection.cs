﻿namespace BrowserDriver.Browsers
{

    public class BrowserConnection
    {
        // type, faviconUrl, devtoolsFrontendUrl, description
        public string? Id { get; init; }
        public string? Title { get; init; }
        public string? Url { get; init; }
        public string? WebSocketDebuggerUrl { get; init; }
    }

}
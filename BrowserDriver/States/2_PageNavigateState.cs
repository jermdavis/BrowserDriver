namespace BrowserDriver.States
{

    public class PageNavigateState : State
    {
        public static State Instance { get; } = new PageNavigateState();
        private static readonly int _id = 2;

        public override async Task Enter(StateMachine owner)
        {
            var nextUrl = (string)owner.State["nextUrl"];

            var request = new PageNavigateParameters() { Url = nextUrl };
            await owner.SendCommand(request, _id);
        }

        public override async Task Update(StateMachine owner, DebuggerResult data)
        {
            // {"id":125,"result":{"frameId":"C93CBCDAB66FCA66E9EA892C630C87BE","loaderId":"6FA90665D5489119EC580B0EF2186A3F"}}
            // {"method":"Page.frameNavigated","params":{"frame":{"id":"6C0B4C4E49B862751ECFA4F3C20CFF54","loaderId":"3CFC18A275A7AAB3943B4A0F758B8F21","url":"https://www.amazon.co.uk/gp/product/B002UPVVVU","domainAndRegistry":"amazon.co.uk","securityOrigin":"https://www.amazon.co.uk","mimeType":"text/html","adFrameStatus":{"adFrameType":"none"},"secureContextType":"Secure","crossOriginIsolatedContextType":"NotIsolated","gatedAPIFeatures":[]},"type":"Navigation"}}

            if (data != null && data?.Method == "Page.loadEventFired")
            {
                owner.TransitionToNewState(FetchDocumentRootState.Instance);
            }
        }

        public override async Task Leave(StateMachine owner)
        {
        }
    }

    public class PageNavigateParameters : IDebuggerCommandProperties
    {
        public string CommandName => "Page.navigate";

        public required string Url { get; set; }
        public string Referrer { get; set; } = string.Empty;
        // transitionType
        // frameId
        // referrerPolicy
    }

}
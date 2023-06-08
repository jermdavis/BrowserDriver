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
    }

}
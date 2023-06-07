namespace BrowserDriver.States
{

    public class PageEventEnableState : State
    {
        public static State Instance { get; } = new PageEventEnableState();
        private static readonly int _id = 1;

        public override async Task Enter(StateMachine owner)
        {
            var request = new PageEventEnableParameters();
            await owner.SendCommand(request, _id);
        }

        public override async Task Update(StateMachine owner, DebuggerResult data)
        {
            // {"id":9,"result":{}}
            if (data != null && data.Id == _id)
            {
                owner.TransitionToNewState(PageNavigateState.Instance);
            }
        }

        public override async Task Leave(StateMachine owner)
        {
        }
    }

    public class PageEventEnableParameters : IDebuggerCommandProperties
    {
        public string CommandName => "Page.enable";
    }

}
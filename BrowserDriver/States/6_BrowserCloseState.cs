namespace BrowserDriver.States
{

    public class BrowserCloseState : State
    {
        public static State Instance { get; } = new BrowserCloseState();
        private static readonly int _id = 5;

        public override async Task Enter(StateMachine owner)
        {
            var request = new BrowserCloseParameters();
            await owner.SendCommand(request, _id);
        }

        public override async Task Update(StateMachine owner, DebuggerResult data)
        {
            // {"id":48,"result":{}}
            if (data != null && data.Id == _id)
            {
                owner.TransitionToNewState(NullState.Instance);
            }
        }

        public override async Task Leave(StateMachine owner)
        {
        }
    }

    public class BrowserCloseParameters : IDebuggerCommandProperties
    {
        public string CommandName => "Browser.close";
    }

}
namespace BrowserDriver.States
{

    public class NullState : State
    {
        public static State Instance { get; } = new NullState();

        public override async Task Enter(StateMachine owner) { }
        public override async Task Update(StateMachine owner, DebuggerResult data) { }
        public override async Task Leave(StateMachine owner) { }
    }

}
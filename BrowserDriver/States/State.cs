namespace BrowserDriver.States
{

    public abstract class State
    {
        public abstract Task Enter(StateMachine owner);
        public abstract Task Update(StateMachine owner, DebuggerResult data);
        public abstract Task Leave(StateMachine owner);
    }

}
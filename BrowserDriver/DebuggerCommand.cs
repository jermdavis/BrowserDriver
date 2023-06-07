namespace BrowserDriver
{

    public class DebuggerCommand<T> where T : IDebuggerCommandProperties
    {
        public int Id { get; set; }
        public string Method { get; private set; }
        public T Params { get; private set; }

        public DebuggerCommand(T parameters)
        {
            Method = parameters.CommandName;
            Params = parameters;
        }
    }

}
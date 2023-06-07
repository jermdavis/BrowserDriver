using System.Text.Json.Nodes;

namespace BrowserDriver
{

    public class DebuggerResult
    {
        public int Id { get; set; }
        public JsonObject? Result { get; set; }
        public JsonObject? Error { get; set; }

        public string? Method { get; set; }
        public JsonObject? Params { get; set; }
    }

}
using System.Text.Json.Serialization;

namespace BrowserDriver.States
{
    
    public class GetOuterHtmlState : State
    {
        public static State Instance { get; } = new GetOuterHtmlState();
        private static readonly int _id = 4;

        public override async Task Enter(StateMachine owner)
        {
            var nodeId = (int)owner.State["NodeID"];

            var request = new GetOuterHtmlParameters() { NodeId = nodeId };
            await owner.SendCommand(request, _id);
        }

        public override async Task Update(StateMachine owner, DebuggerResult data)
        {
            if (data != null && data.Id == _id)
            {
                var html = data?.Result?["outerHTML"]?.GetValue<string>() ?? string.Empty;
                owner.State["HTML"] = html;

                owner.TransitionToNewState(NullState.Instance);
            }
        }
        public override async Task Leave(StateMachine owner)
        {
        }
    }

    public class GetOuterHtmlParameters : IDebuggerCommandProperties
    {
        [JsonIgnore]
        public string CommandName => "DOM.getOuterHTML";

        public required int NodeId { get; set; }
    }

}
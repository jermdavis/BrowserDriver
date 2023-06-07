using System.Text.Json.Serialization;

namespace BrowserDriver.States
{

    public class FetchDocumentRootState : State
    {
        public static State Instance { get; } = new FetchDocumentRootState();
        private static readonly int _id = 3;

        public override async Task Enter(StateMachine owner)
        {
            var request = new FetchDocumentRootParameters() { };
            await owner.SendCommand(request, _id);
        }

        public override async Task Update(StateMachine owner, DebuggerResult data)
        {
            // {"id":0,"result":{"root":{"nodeId":1,"backendNodeId":1,"nodeType":9,"nodeName":"#document","localName":"","nodeValue":"","childNodeCount":2,"documentURL":"https://www.bbc.co.uk/news","baseURL":"https://www.bbc.co.uk/news","xmlVersion":"","compatibilityMode":"NoQuirksMode"}}}

            if (data != null && data.Id == _id)
            {
                var nodeId = data.Result?["root"]?["children"]?[1]?["nodeId"]?.GetValue<int>() ?? -1;

                owner.State["NodeID"] = nodeId;
                owner.TransitionToNewState(GetOuterHtmlState.Instance);
            }
        }

        public override async Task Leave(StateMachine owner)
        {
        }
    }

    public class FetchDocumentRootParameters : IDebuggerCommandProperties
    {
        [JsonIgnore]
        public string CommandName => "DOM.getDocument";

        public int Depth { get; set; } = 1;
        public bool Pierce { get; set; }
    }

}
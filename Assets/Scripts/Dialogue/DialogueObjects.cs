using System.Collections.Generic;

namespace Dialogue
{
    public class DialogueResponse
    {
        public string Text { get; }
        public string Destination { get; }

        public DialogueResponse(string responseText, string responseDestination)
        {
            Text = responseText;
            Destination = responseDestination;
        }
    }

    public class DialogueNode
    {
        public string Text { get; }
        public List<DialogueResponse> Responses { get; }
        public bool FinalNode { get; }

        public DialogueNode(string dialogueText, List<DialogueResponse> dialogueResponses)
        {
            Text = dialogueText;
            Responses = dialogueResponses;
            FinalNode = dialogueResponses.Count == 0;
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace Dialogue
{
    public class DialogueResponse
    {
        public enum ResponseSentiment
        {
            Neutral,
            Bad,
            Good
        }
        private readonly IDictionary<string, ResponseSentiment> _sentimentMapping = new Dictionary<string, ResponseSentiment>()
        {
            {"neutral", ResponseSentiment.Neutral},
            {"bad", ResponseSentiment.Bad},
            {"good", ResponseSentiment.Good},
        };
        
        public string Text { get; }
        public string Destination { get; }
        public ResponseSentiment Sentiment { get; }

        public DialogueResponse(string responseText, string responseDestination, string responseSentiment)
        {
            Text = responseText;
            Destination = responseDestination;
            Sentiment = _sentimentMapping.ContainsKey(responseSentiment) ? _sentimentMapping[responseSentiment] : ResponseSentiment.Neutral;
        }
    }

    public class DialogueNode
    {
        public enum DialogueEmotion
        {
            Neutral,
            Angry,
            Happy,
            Sad,
            Blush
        }
        private readonly IDictionary<string, DialogueEmotion> _emotionsMapping = new Dictionary<string, DialogueEmotion>()
        {
            {"neutral", DialogueEmotion.Neutral},
            {"angry", DialogueEmotion.Angry},
            {"happy", DialogueEmotion.Happy},
            {"sad", DialogueEmotion.Sad},
            {"blush", DialogueEmotion.Blush}
        };

        public enum DialogueAction
        {
            None,
            Enter
        }
        private readonly IDictionary<string, DialogueAction> _actionsMapping = new Dictionary<string, DialogueAction>()
        {
            {"none", DialogueAction.None},
            {"enter", DialogueAction.Enter},
        };

        public string Text { get; }
        public List<DialogueResponse> Responses { get; }
        public DialogueEmotion Emotion { get; }
        public string Sound { get; }
        public DialogueAction Action { get; }
        public bool FinalNode { get; }

        public DialogueNode(string dialogueText, List<DialogueResponse> dialogueResponses, string dialogueEmotion="", string dialogueSound="", string dialogueAction="")
        {
            Text = dialogueText;
            Responses = dialogueResponses;
            Emotion = DialogueEmotion.Neutral;
            if (dialogueEmotion != "")
            {
                if (_emotionsMapping.ContainsKey(dialogueEmotion))
                {
                    Emotion = _emotionsMapping[dialogueEmotion];
                }
                else
                {
                    Debug.LogError("Parsing error, invalid emotion: '" + dialogueEmotion + "'");
                }
            }
            Sound = dialogueSound;
            Action = DialogueAction.None;
            if (dialogueAction != "")
            {
                if (_actionsMapping.ContainsKey(dialogueAction))
                {
                    Action = _actionsMapping[dialogueAction];
                }
                else
                {
                    Debug.LogError("Parsing error, invalid emotion: '" + dialogueAction + "'");
                }
            }
            FinalNode = dialogueResponses.Count == 0;
        }
    }
}

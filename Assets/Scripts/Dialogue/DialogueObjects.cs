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

        public enum DialogueActionValue
        {
            NotSpecified,
            False,
            True
        }
        private readonly IDictionary<string, DialogueActionValue> _actionValueMapping = new Dictionary<string, DialogueActionValue>()
        {
            {"", DialogueActionValue.NotSpecified},
            {"false", DialogueActionValue.False},
            {"true", DialogueActionValue.True},
        };
        public struct DialogueActions
        {
            public DialogueActionValue Onscreen;
            public DialogueActionValue Shadow;
            public DialogueActionValue ZoomIn;
        }

        public readonly string Text;
        public readonly List<DialogueResponse> Responses;
        public readonly DialogueEmotion Emotion;
        public readonly string Sound;
        public readonly DialogueActions Actions;
        public readonly bool FinalNode;

        public DialogueNode(string dialogueText, List<DialogueResponse> dialogueResponses, string dialogueEmotion="",
            string dialogueSound="", string onscreen="", string shadow="", string zoomIn="")
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
            Actions = new DialogueActions();
            if (_actionValueMapping.ContainsKey(onscreen))
            {
                Actions.Onscreen = _actionValueMapping[onscreen];
            }
            else
            {
                Debug.LogError("Parsing error, invalid onscreen action: '" + onscreen + "'");
            }
            if (_actionValueMapping.ContainsKey(shadow))
            {
                Actions.Shadow = _actionValueMapping[shadow];
            }
            else
            {
                Debug.LogError("Parsing error, invalid shadow action: '" + shadow + "'");
            }
            if (_actionValueMapping.ContainsKey(zoomIn))
            {
                Actions.ZoomIn = _actionValueMapping[zoomIn];
            }
            else
            {
                Debug.LogError("Parsing error, invalid zoom in action: '" + zoomIn + "'");
            }
            FinalNode = dialogueResponses.Count == 0;
        }
    }
}

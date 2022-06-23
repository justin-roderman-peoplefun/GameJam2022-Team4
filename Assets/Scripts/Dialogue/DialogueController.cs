using System.Collections.Generic;
using Newtonsoft.Json;
using ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialogueController : MonoBehaviour
    {
        public Image companionAvatar;
        public TMP_Text dialoguePrompt;
        public TMP_Text dialogueOption1;
        public TMP_Text dialogueOption2;
        public GameObject tapToContinue;

        private CompanionInfo _companionInfo;
        private IDictionary<string, DialogueNode> _dialogueNodes;
        private DialogueNode _currNode;
    
        private void Start()
        {
            _companionInfo = GameManager.Instance.GetSelectedCompanionInfo();
            _dialogueNodes = new Dictionary<string, DialogueNode>();
            InitializeDialogueUI();
            InitializeDialogue();
        }
    
        private void InitializeDialogueUI()
        {
            companionAvatar.sprite = _companionInfo.image;
        }
    
        private void InitializeDialogue()
        {
            string dialogText;
            if (!GameManager.Instance.introPlayed)
            {
                dialogText = _companionInfo.introDialogue.text;
            }
            else if (GameManager.Instance.CurrStage < (GameManager.Instance.numStages - 1))
            {
                dialogText = _companionInfo.stageDialogues[GameManager.Instance.CurrStage].text;
            }
            else if ((GameManager.Instance.TotalHeartsCollected + GameManager.Instance.CurrHeartsCollected) >= GameManager.Instance.goodEndingHeartRequirement)
            {
                dialogText = _companionInfo.goodEndingDialogue.text;
            }
            else
            {
                dialogText = _companionInfo.badEndingDialogue.text;
            }
            var rootModel = JsonConvert.DeserializeObject<DialogueRootModel>(dialogText);
            Assert.IsNotNull(rootModel);
            foreach(var entry in rootModel.Data.Stitches)
            {
                var emotion = "";
                var sound = "";
                var action = "";

                var dialogueText = entry.Value.Content[0].ToString();
                // I could make this a regex, but I don't feel like it
                if (dialogueText.Contains("[") && dialogueText.Contains("]"))
                {
                    var tagStart = dialogueText.IndexOf("[");
                    var tagLength = dialogueText.IndexOf("]") - tagStart + 1;
                    var tagsStr = dialogueText.Substring(tagStart + 1, tagLength - 2);
                    var tags = tagsStr.Split('|');
                    foreach (var tagStr in tags)
                    {
                        var tagElems = tagStr.Split(':');
                        switch (tagElems[0])
                        {
                            case "emotion":
                                emotion = tagElems[1];
                                break;
                            case "sound":
                                sound = tagElems[1];
                                break;
                            case "action":
                                action = tagElems[1];
                                break;
                            default:
                                Debug.LogError("Parsing error, invalid tag: '" + tagElems[0] + "'");
                                break;
                        }
                    }
                    dialogueText = dialogueText.Substring(0, tagStart) + dialogueText.Substring(tagStart + tagLength);
                }

                var responses = new List<DialogueResponse>();
                for (var i = 1; i < entry.Value.Content.Count; i++)
                {
                    var optionStr = entry.Value.Content[i].ToString();
                    var option = JsonConvert.DeserializeObject<DialogueOptionModel>(optionStr);
                    if (option?.Option != null)
                    {
                        var sentiment = "";
                        if (option.Option.Contains("[") && option.Option.Contains("]"))
                        {
                            var tagStart = option.Option.IndexOf("[");
                            var tagLength = option.Option.IndexOf("]") - tagStart + 1;
                            sentiment = option.Option.Substring(tagStart + 1, tagLength - 2);
                            option.Option = option.Option.Substring(0, tagStart) + option.Option.Substring(tagStart + tagLength);
                        }
                        responses.Add(new DialogueResponse(option.Option.Trim(), option.LinkPath.Trim(), sentiment));
                    }
                }

                _dialogueNodes[entry.Key] = new DialogueNode(dialogueText.Trim(), responses, emotion, sound, action);
            }
    
            _currNode = _dialogueNodes[rootModel.Data.Initial];
            UpdateDialogueUI();
        }

        private void Update()
        {
            if (_currNode.FinalNode && Input.GetMouseButtonDown(0) && !GameManager.Instance.inBubbleTransition)
            {
                EndDialogue();
            }
        }

        private void EndDialogue()
        {
            if (!GameManager.Instance.introPlayed)
            {
                GameManager.Instance.StartGame();
            }
            else if (GameManager.Instance.CurrStage < (GameManager.Instance.numStages - 1))
            {
                GameManager.Instance.BubbleTransitionScene("DialogueEndScene", GameManager.AsyncTransition.AsyncLoad);
            }
            else
            {
                // TODO Switch to end credits scene
                GameManager.Instance.BubbleTransitionScene("MainMenuScene");
            }
        }

        public void ChooseResponse(int responseIndex)
        {
            var response = _currNode.Responses[responseIndex];
            if (GameManager.Instance.introPlayed)
            {
                if (response.Sentiment == DialogueResponse.ResponseSentiment.Good)
                {
                    GameManager.Instance.EarnHearts(GameManager.Instance.heartsGoodResponse);
                }
                else if (response.Sentiment == DialogueResponse.ResponseSentiment.Bad)
                {
                    GameManager.Instance.EarnHearts(GameManager.Instance.heartsBadResponse);
                }
            }
            _currNode = _dialogueNodes[response.Destination];
            UpdateDialogueUI();
        }
    
        private void UpdateDialogueUI()
        {
            dialoguePrompt.text = _currNode.Text;
            if (!_currNode.FinalNode)
            {
                dialogueOption1.text = _currNode.Responses[0].Text;
    
                var hasSecondResponse = _currNode.Responses.Count > 1;
                dialogueOption2.transform.parent.gameObject.SetActive(hasSecondResponse);
                if (hasSecondResponse)
                {
                    dialogueOption2.text = _currNode.Responses[1].Text;
                }
            }
            else
            {
                dialogueOption1.transform.parent.gameObject.SetActive(false);
                dialogueOption2.transform.parent.gameObject.SetActive(false);
                tapToContinue.SetActive(true);
            }

            switch (_currNode.Emotion)
            {
                case DialogueNode.DialogueEmotion.Angry:
                    companionAvatar.sprite = _companionInfo.angryImage;
                    break;
                case DialogueNode.DialogueEmotion.Happy:
                    companionAvatar.sprite = _companionInfo.happyImage;
                    break;
                case DialogueNode.DialogueEmotion.Sad:
                    companionAvatar.sprite = _companionInfo.sadImage;
                    break;
                case DialogueNode.DialogueEmotion.Blush:
                    companionAvatar.sprite = _companionInfo.blushImage;
                    break;
                default:
                    companionAvatar.sprite = _companionInfo.image;
                    break;
            }
        }
    }   
}

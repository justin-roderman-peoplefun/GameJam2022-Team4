using System;
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
            InitializeDialogueUI();
            InitializeDialogue();
        }
    
        private void InitializeDialogueUI()
        {
            companionAvatar.sprite = _companionInfo.image;
        }
    
        private void InitializeDialogue()
        {
            _dialogueNodes = new Dictionary<string, DialogueNode>();
    
            var rootModel = JsonConvert.DeserializeObject<DialogueRootModel>(_companionInfo.dateDialogueTexts[0].text);
            Assert.IsNotNull(rootModel);
            foreach(var entry in rootModel.Data.Stitches)
            {
                var dialogueText = entry.Value.Content[0].ToString();
                var responses = new List<DialogueResponse>();
                for (var i = 1; i < entry.Value.Content.Count; i++)
                {
                    var optionStr = entry.Value.Content[i].ToString();
                    var option = JsonConvert.DeserializeObject<DialogueOptionModel>(optionStr);
                    if (option?.Option != null)
                    {
                        responses.Add(new DialogueResponse(option.Option, option.LinkPath));
                    }
                }
    
                _dialogueNodes[entry.Key] = new DialogueNode(dialogueText, responses);
            }
    
            _currNode = _dialogueNodes[rootModel.Data.Initial];
            UpdateDialogueUI();
        }

        private void Update()
        {
            if (_currNode.FinalNode && Input.GetMouseButtonDown(0))
            {
                StageManager.Instance.AdvanceStage();
                GameManager.Instance.BubbleTransitionScene("DialogueScene", GameManager.AsyncTransition.AsyncUnload);
            }
        }

        public void ChooseResponse(int responseIndex)
        {
            var nextNode = _currNode.Responses[responseIndex].Destination;
            _currNode = _dialogueNodes[nextNode];
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
        }
    }   
}

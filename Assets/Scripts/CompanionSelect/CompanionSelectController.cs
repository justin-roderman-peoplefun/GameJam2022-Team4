using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CompanionSelectController : MonoBehaviour
{
    public GameObject promptTextParent;
    public TMP_Text companionNameText;
    public TMP_Text companionQuoteText;
    public TMP_Text NotImplementedText;
    public Button DescendButton;
    
    // Maps the button name to the corresponding companion
    private readonly Dictionary<string, Constants.Companion> _companionMapping = new Dictionary<string, Constants.Companion>()
    {
        {"PirateButton", Constants.Companion.Pirate},
        {"LeviathanButton", Constants.Companion.Leviathan},
        {"MermaidButton", Constants.Companion.Mermaid},
        {"SharkButton", Constants.Companion.Shark},
    };

    public void SelectCompanion(Button companionButton)
    {
        if (promptTextParent.activeSelf)
        {
            promptTextParent.SetActive(false);
            companionNameText.transform.parent.gameObject.SetActive(true);
        }

        var companion = _companionMapping[companionButton.name];
        GameManager.Instance.SetSelectedCompanion(companion);

        companionButton.image.color = Color.white;
        companionButton.transform.GetChild(0).gameObject.SetActive(true);
        foreach (var button in companionButton.transform.parent.GetComponentsInChildren<Button>())
        {
            if (button.name != companionButton.name)
            {
                button.image.color = new Color(1, 1, 1, 0.5f);
                button.transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        var companionInfo = GameManager.Instance.GetCompanionInfo(companion);
        companionNameText.SetText(companionInfo.companionName);
        companionQuoteText.SetText(companionInfo.companionQuote);

        NotImplementedText.gameObject.SetActive(!companionInfo.complete);
        DescendButton.interactable = companionInfo.complete;
    }

    public void Descend()
    {
        GameManager.Instance.BubbleTransitionScene("DialogueScene");
    }
}

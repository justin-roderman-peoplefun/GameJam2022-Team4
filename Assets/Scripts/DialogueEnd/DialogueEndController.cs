using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueEndController : MonoBehaviour
{
    public Image background;
    public TMP_Text dialogueResult;
    public Image lifeResult;
    public Image maxLifeResult;
    public Image companionImage;

    public Color rewardColor; // = new Color(146, 196, 125);
    public Color noRewardColor; // = new Color(204, 204, 204);

    public string goodResultText;
    public string okayResultText;
    public string badResultText;

    public string lifeRefilledText;
    public string lifeNotRefilledText;

    public string maxLifeIncreaseText;

    public float waitTime;

    private float _timeElapsed;

    private void Start()
    {
        SceneManager.UnloadSceneAsync("DialogueScene");
        InitializeUI();
    }

    private void InitializeUI()
    {
        var lifeRefilled = GameManager.Instance.CurrHeartsCollected >= GameManager.Instance.RefillLifeRequirement;
        lifeResult.color = lifeRefilled ? rewardColor : noRewardColor;
        var lifeResultText = lifeResult.gameObject.GetComponentInChildren<TMP_Text>();
        lifeResultText.text = lifeRefilled ? lifeRefilledText : lifeNotRefilledText;

        var maxLifeIncreased = GameManager.Instance.CurrHeartsCollected >= GameManager.Instance.MaxLifeRequirement;
        maxLifeResult.color = maxLifeIncreased ? rewardColor : noRewardColor;
        var maxLifeText = maxLifeResult.gameObject.GetComponentInChildren<TMP_Text>();
        maxLifeText.text = "+" + (maxLifeIncreased ? 1 : 0) + " " + maxLifeIncreaseText;

        var companionInfo = GameManager.Instance.GetSelectedCompanionInfo();
        if (maxLifeIncreased)
        {
            companionImage.sprite = companionInfo.happyImage;
            dialogueResult.text = goodResultText;
        }
        else if (lifeRefilled)
        {
            companionImage.sprite = companionInfo.image;
            dialogueResult.text = okayResultText;
        }
        else
        {
            companionImage.sprite = companionInfo.sadImage;
            dialogueResult.text = badResultText;
        }

        background.sprite = companionInfo.backgroundImage;
    }

    private void Update()
    {
        if (_timeElapsed > waitTime && Input.GetMouseButtonDown(0) && !GameManager.Instance.inBubbleTransition)
        {
            GameManager.Instance.BubbleTransitionScene("DialogueEndScene", GameManager.AsyncTransition.AsyncUnload);
            GameManager.Instance.AdvanceStage();
        }
        _timeElapsed += Time.deltaTime;
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueEndController : MonoBehaviour
{
    public TMP_Text dialogueResult;
    public Image lifeResult;
    public Image maxLifeResult;
    public Image companionImage;

    public Color rewardColor; // = new Color(146, 196, 125);
    public Color rewardTextColor; // = Color.black;
    public Color noRewardColor; // = new Color(204, 204, 204);
    public Color noRewardTextColor; // = new Color(153, 153, 153);

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
        companionImage.sprite = GameManager.Instance.GetSelectedCompanionInfo().image;

        var lifeRefilled = GameManager.Instance.CurrHeartsCollected >= GameManager.Instance.RefillLifeRequirement;
        lifeResult.color = lifeRefilled ? rewardColor : noRewardColor;
        var lifeResultText = lifeResult.gameObject.GetComponentInChildren<TMP_Text>();
        lifeResultText.text = lifeRefilled ? lifeRefilledText : lifeNotRefilledText;
        lifeResultText.color = lifeRefilled ? rewardTextColor : noRewardTextColor;

        var maxLifeIncreased = GameManager.Instance.CurrHeartsCollected >= GameManager.Instance.MaxLifeRequirement;
        maxLifeResult.color = maxLifeIncreased ? rewardColor : noRewardColor;
        var maxLifeText = maxLifeResult.gameObject.GetComponentInChildren<TMP_Text>();
        maxLifeText.text = "+" + (maxLifeIncreased ? 1 : 0) + " " + maxLifeIncreaseText;
        maxLifeText.color = maxLifeIncreased ? rewardTextColor : noRewardTextColor;

        dialogueResult.text = maxLifeIncreased ? goodResultText : lifeRefilled ? okayResultText : badResultText;
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

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEndController : MonoBehaviour
{
    public CanvasGroup endingParent;
    public TMP_Text dialogueResult;
    public Image gameEnd;
    public Image gameEndBorder;
    public CanvasGroup tapToContinue;

    public Color borderGoodColor;
    public Color borderBadColor;
    public string goodEndText;
    public string badEndText;

    private void Start()
    {
        InitializeUI();
        StartCoroutine(FadeIn(endingParent, 0.5f));
        StartCoroutine(FadeIn(tapToContinue, 2.5f));
    }

    private void InitializeUI()
    {
        var goodEnd = GameManager.Instance.TotalHeartsCollected + GameManager.Instance.CurrHeartsCollected >=
                      GameManager.Instance.goodEndingHeartRequirement;
        dialogueResult.text = goodEnd ? goodEndText : badEndText;

        var companionInfo = GameManager.Instance.GetSelectedCompanionInfo();
        gameEnd.sprite = goodEnd ? companionInfo.goodEndingImage : companionInfo.badEndingImage;

        gameEndBorder.color = goodEnd ? borderGoodColor : borderBadColor;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && tapToContinue.alpha >= 1)
        {
            GameManager.Instance.BubbleTransitionScene("CreditsScene");
        }
    }

    private IEnumerator FadeIn(CanvasGroup canvasGroup, float waitTime)
    {
        if (waitTime > 0)
        {
            yield return new WaitForSeconds(waitTime);
        }
        do
        {
            canvasGroup.alpha += Time.deltaTime;
            yield return null;
        } while (canvasGroup.alpha < 1);
    }
}

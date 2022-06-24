using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum AsyncTransition
    {
        Sync,
        AsyncLoad,
        AsyncUnload
    }

    public static GameManager Instance;

    [HideInInspector]
    public bool inBubbleTransition;
    [HideInInspector]
    public bool introPlayed;

    private int heartsCollectedGameplay;
    private int timesHeartsCollectedDialogue;
    [SerializeField]
    private int currHeartsCollected;
    private int _totalHeartsCollected;
    private int _currStage;
    private Constants.Companion? _companion;
    public Constants.Companion? Companion => _companion;

    public CanvasGroup canvas;
    public List<CompanionInfo> companions;

    public int numStages;
    [SerializeField]
    private List<int> maxHeartsGameplay;
    public int MaxHeartsGameplay => maxHeartsGameplay[_currStage];
    [SerializeField]
    private List<int> maxDialogueHeartsTimes;
    public int MaxDialogueHeartsTimes => maxDialogueHeartsTimes[_currStage];
    public int heartsGoodResponse;
    public int heartsBadResponse;
    [SerializeField]
    private List<int> maxStageHearts;
    public int MaxStageHeart => maxStageHearts[_currStage];
    [SerializeField]
    private List<int> refillLifeRequirements;
    public int RefillLifeRequirement => refillLifeRequirements[_currStage];
    [SerializeField]
    private List<int> maxLifeRequirements;
    public int MaxLifeRequirement => maxLifeRequirements[_currStage];
    [SerializeField]
    private List<int> totalMaxHearts;
    public int TotalMaxHeart => totalMaxHearts[_currStage];
    public int goodEndingHeartRequirement;

    public int CurrHeartsCollected => currHeartsCollected;
    public int TotalHeartsCollected => _totalHeartsCollected;
    public int CurrStage => _currStage;

    private void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad (gameObject);
    }

    public void SetSelectedCompanion(Constants.Companion companion)
    {
        _companion = companion;
    }

    public CompanionInfo GetSelectedCompanionInfo()
    {
        return _companion == null ? null : GetCompanionInfo(_companion.Value);
    }

    public CompanionInfo GetCompanionInfo(Constants.Companion companion)
    {
        return companions.Find(info => info.key == companion.ToString().ToLower());
    }

    public void TransitionScene(string sceneTo, AsyncTransition async=AsyncTransition.Sync)
    {
        StartCoroutine(FadeScreen(sceneTo, async));
    }

    public void BubbleTransitionScene(string sceneTo, AsyncTransition async=AsyncTransition.Sync)
    {
        StartCoroutine(BubbleTransitionSceneInternal(sceneTo, async));
    }

    private IEnumerator BubbleTransitionSceneInternal(string sceneTo, AsyncTransition async)
    {
        inBubbleTransition = true;
        GetComponentInChildren<ParticleSystem>().Stop();
        GetComponentInChildren<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeScreen(sceneTo, async));
        yield return new WaitForSeconds(1);
        inBubbleTransition = false;
    }

    private IEnumerator FadeScreen(string sceneTo, AsyncTransition async)
    {
        canvas.gameObject.SetActive(true);
        canvas.alpha = 0;
        do
        {
            canvas.alpha += Time.deltaTime;
            yield return null;
        } while (canvas.alpha < 1);

        switch (async)
        {
            case AsyncTransition.Sync:
                SceneManager.LoadScene(sceneTo);
                yield return new WaitForSeconds(0.1f);
                break;
            case AsyncTransition.AsyncLoad:
                SceneManager.LoadSceneAsync(sceneTo, LoadSceneMode.Additive);
                yield return new WaitForSeconds(0.1f);
                break;
            case AsyncTransition.AsyncUnload:
                SceneManager.UnloadSceneAsync(sceneTo);
                break;
        }
  
        do
        {
            canvas.alpha -= Time.deltaTime;
            yield return null;
        } while (canvas.alpha > 0);
        canvas.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        introPlayed = true;
        BubbleTransitionScene("GameScene");
    }

    public void RetryStage()
    {
        heartsCollectedGameplay = 0;
        timesHeartsCollectedDialogue = 0;
        currHeartsCollected = 0;
        StageManager.Instance.gameOverUI.alpha = 0;
        StageManager.Instance.gameOverUI.gameObject.SetActive(false);
        PlayerController.Instance.TruePlayerReset();
        StartCoroutine(StageManager.Instance.LoadStage(_currStage));
    }

    public void BackToMainMenu()
    {
        _companion = null;
        introPlayed = false;
        heartsCollectedGameplay = 0;
        timesHeartsCollectedDialogue = 0;
        currHeartsCollected = 0;
        _totalHeartsCollected = 0;
        _currStage = 0;
        BubbleTransitionScene("MainMenuScene");
    }

    public void AdvanceStage()
    {
        _totalHeartsCollected += currHeartsCollected;
        StageManager.Instance.AdvanceStage();
        heartsCollectedGameplay = 0;
        timesHeartsCollectedDialogue = 0;
        currHeartsCollected = 0;
        _currStage++;
    }

    public void EarnHearts(int numHearts, bool gameplay)
    {
        if (gameplay && heartsCollectedGameplay + numHearts > MaxHeartsGameplay)
        {
            numHearts = MaxHeartsGameplay - heartsCollectedGameplay;
        }
        else if (!gameplay && timesHeartsCollectedDialogue + 1 > MaxDialogueHeartsTimes)
        {
            numHearts = 0;
        }
        if (currHeartsCollected + numHearts > MaxStageHeart)
        {
            numHearts = MaxStageHeart - currHeartsCollected;
        }

        if (numHearts <= 0)
        {
            Debug.Log("Player could not collect <color=red>hearts</color> because hit limit");
            return;
        }

        if (gameplay)
        {
            heartsCollectedGameplay += numHearts;
        }
        else
        {
            timesHeartsCollectedDialogue++;
        }
        currHeartsCollected += numHearts;
        Debug.Log("Player collected <color=red>" + numHearts + "</color> hearts! New total: <color=red>" + currHeartsCollected + "</color> (out of max <color=blue>" + MaxStageHeart + "</color>)");
    }
}
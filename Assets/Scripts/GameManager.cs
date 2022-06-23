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

    [SerializeField]
    private int _currHeartsCollected;
    private int _totalHeartsCollected;
    private int _currStage;
    private Constants.Companion? _companion;
    public Constants.Companion? Companion => _companion;

    public CanvasGroup canvas;
    public List<CompanionInfo> companions;

    public int numStages;
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

    public int CurrHeartsCollected => _currHeartsCollected;
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
        _currHeartsCollected = 0;
        StageManager.Instance.gameOverUI.alpha = 0;
        StageManager.Instance.gameOverUI.gameObject.SetActive(false);
        StartCoroutine(StageManager.Instance.LoadStage(_currStage));
    }

    public void BackToMainMenu()
    {
        _companion = null;
        introPlayed = false;
        _currHeartsCollected = 0;
        _totalHeartsCollected = 0;
        _currStage = 0;
        BubbleTransitionScene("MainMenuScene");
    }

    public void AdvanceStage()
    {
        _totalHeartsCollected += _currHeartsCollected;
        StageManager.Instance.AdvanceStage();
        _currHeartsCollected = 0;
        _currStage++;
    }

    public void EarnHearts(int numHearts)
    {
        _currHeartsCollected += numHearts;
        if (_currHeartsCollected > MaxStageHeart)
        {
            _currHeartsCollected = MaxStageHeart;
        }
        Debug.Log("Player collected <color=red>" + numHearts + "</color> hearts! New total: <color=red>" + _currHeartsCollected + "</color>");
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class Stage
{
    public string sceneName;
}

public class StageManager : MonoBehaviour
{
    private static bool m_isStagePlaying;
    private static int m_heartsCollectedInCurrentStage;
    private static float m_currentStageTime;
    private int currStageIndex = -1;

    public Stage[] stages;
    public CanvasGroup gameOverUI;

    public static bool IsStagePlaying
    {
        get
        {
            return m_isStagePlaying;
        }
    }

    public static StageManager Instance { get; private set; }
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Debug.LogError("There was more than one StageManager in the scene. Deleting the one named: <color=cyan>" + gameObject.name + "</color>.");
            Destroy(gameObject); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    
    private void Start()
    {
        if (stages.Length <= 0)
        {
            Debug.LogError("The Stage Manager wants to play, but has no stages defined.");
            return;
        }
        
        m_isStagePlaying = false;

        if (currStageIndex < 0)
            StartCoroutine(LoadStage(0));
    }

    public void AdvanceStage()
    {
        var lifeRefilled = GameManager.Instance.CurrHeartsCollected >= GameManager.Instance.RefillLifeRequirement;
        var maxLifeIncreased = GameManager.Instance.CurrHeartsCollected >= GameManager.Instance.MaxLifeRequirement;
        if(maxLifeIncreased) PlayerController.Instance.IncrementMaxLife();
        if(lifeRefilled) PlayerController.Instance.RefillLife();
        StartCoroutine(LoadStage(currStageIndex + 1));
    }
    
    public IEnumerator LoadStage(int index)
    {
        m_isStagePlaying = false;
        
        StartCoroutine(UnloadCurrentStage());

        currStageIndex = index;
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(stages[index].sceneName, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
            yield return null;

        StartCoroutine(PlayerController.Instance.ResetPlayerLocationRoutine());
        StartStage();
    }

    IEnumerator UnloadCurrentStage()
    {
        if (currStageIndex < 0 || currStageIndex >= GameManager.Instance.numStages)
            yield break;
        
        if (SceneManager.GetSceneByName(stages[currStageIndex].sceneName).IsValid())
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(stages[currStageIndex].sceneName);
            while (!asyncUnload.isDone)
                yield return null;
        }
    }

    void StartStage()
    {
        m_heartsCollectedInCurrentStage = 0;
        m_currentStageTime = 0;
        PlayerController.Instance.RefreshHealthAuraColor();
        SoundManager.Instance.PlayMusic(MusicSongs.Stage1 + currStageIndex);
        m_isStagePlaying = true;
        Debug.Log("Stage <color=green>[" + stages[currStageIndex].sceneName + "]</color> has begun.");
    }

    public void StageComplete()
    {
        Debug.Log("Stage <color=green>[" + stages[currStageIndex].sceneName + "]</color> is finished.");
        m_isStagePlaying = false;

        if ((currStageIndex + 1) >= GameManager.Instance.numStages)
        {
            StartCoroutine(UnloadCurrentStage());
            Debug.Log("<color=cyan>Final stage complete!</color>");
        }
        GameManager.Instance.BubbleTransitionScene("DialogueScene", GameManager.AsyncTransition.AsyncLoad);
    }
    
    public static void CollectHeart()
    {
        m_heartsCollectedInCurrentStage++;
    }

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1);
        gameOverUI.gameObject.SetActive(true);
        while (gameOverUI.alpha < 1)
        {
            gameOverUI.alpha += Time.deltaTime;
            yield return null;
        }
    }

    public void RetryButtonCallback()
    {
        GameManager.Instance.RetryStage();
    }

    public void MainMenuButtonCallback()
    {
        GameManager.Instance.BackToMainMenu();
    }
}

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
    public SceneAsset scene;
    public float maxTime = 60f;
    public int maxHearts = 10;
}

public class StageManager : MonoBehaviour
{
    private static bool m_isStagePlaying;
    private static int m_heartsCollectedInCurrentStage;
    private static float m_currentStageTime;
    private int currStageIndex = -1;
    
    public Stage[] stages;

    public static bool IsStagePlaying
    {
        get
        {
            return m_isStagePlaying;
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

    void AdvanceStage()
    {
        StartCoroutine(LoadStage(currStageIndex + 1));
    }
    
    IEnumerator LoadStage(int index)
    {
        m_isStagePlaying = false;

        if (currStageIndex != -1)
        {
            AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(stages[currStageIndex].scene.name);
            while (!asyncUnload.isDone)
                yield return null;
        }

        currStageIndex = index;
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(stages[index].scene.name, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
            yield return null;

        StartStage();
    }

    IEnumerator UnloadCurrentStage()
    {
        AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(stages[currStageIndex].scene.name);
        while (!asyncUnload.isDone)
            yield return null;
    }

    void StartStage()
    {
        m_heartsCollectedInCurrentStage = 0;
        m_currentStageTime = 0;

        StartCoroutine(WaitForStageToFinish());
        
        m_isStagePlaying = true;
        Debug.Log("Stage <color=green>[" + stages[currStageIndex].scene.name + "]</color> has begun.");
    }

    IEnumerator WaitForStageToFinish()
    {
        while (m_currentStageTime < stages[currStageIndex].maxTime && m_heartsCollectedInCurrentStage < stages[currStageIndex].maxHearts)
        {
            yield return null;
            m_currentStageTime += Time.deltaTime;
        }
        
        Debug.Log("Stage <color=green>[" + stages[currStageIndex].scene.name + "]</color> is finished.");
        
        if ((currStageIndex + 1) >= stages.Length)
        {
            StartCoroutine(UnloadCurrentStage());
            Debug.Log("<color=cyan>Final stage complete!</color>");
            
            //TODO: Show ending?
        }
        else
        {
            //TODO: Show dialog/date scene.
            AdvanceStage();
        }
        
        
    }

    public static void CollectHeart()
    {
        m_heartsCollectedInCurrentStage++;
    }
}

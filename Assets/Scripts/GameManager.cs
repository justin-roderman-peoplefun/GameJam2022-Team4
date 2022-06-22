using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int heartsCollected = 0;

    public CanvasGroup canvas;
    public List<CompanionInfo> companions;

    public int HeartsCollected => heartsCollected;

    public static GameManager Instance;
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

    public CompanionInfo GetCompanionInfo(Constants.Companion companion)
    {
        return companions.Find(info => info.key == companion.ToString().ToLower());
    }

    public void TransitionScene(string sceneTo)
    {
        StartCoroutine(FadeScreen(sceneTo));
    }

    public void BubbleTransitionScene(string sceneTo, bool async=false)
    {
        StartCoroutine(BubbleTransitionSceneInternal(sceneTo, async));
    }

    private IEnumerator BubbleTransitionSceneInternal(string sceneTo, bool async)
    {
        GetComponentInChildren<ParticleSystem>().Stop();
        GetComponentInChildren<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeScreen(sceneTo, async));
        yield return new WaitForSeconds(1);
    }

    private IEnumerator FadeScreen(string sceneTo, bool async=false)
    {
        canvas.gameObject.SetActive(true);
        canvas.alpha = 0;
        do
        {
            canvas.alpha += Time.deltaTime;
            yield return null;
        } while (canvas.alpha < 1);

        // Load the scene, then wait for a sec to finish loading
        if (async)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == sceneTo)
                {
                    SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                    break;
                }
            }
            SceneManager.LoadSceneAsync(sceneTo, LoadSceneMode.Additive);
        }
        else
        {
            SceneManager.LoadScene(sceneTo);
        }
        yield return new WaitForSeconds(0.1f);
  
        do
        {
            canvas.alpha -= Time.deltaTime;
            yield return null;
        } while (canvas.alpha > 0);
        canvas.gameObject.SetActive(false);
    }

    public void EarnHearts(int numHearts)
    {
        heartsCollected += numHearts;
        Debug.Log("Player collected <color=red>" + numHearts + "</color> hearts! New total: <color=red>" + heartsCollected + "</color>");
    }
}
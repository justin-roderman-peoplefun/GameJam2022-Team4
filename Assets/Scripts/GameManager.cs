using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum AsyncTransition
    {
        Sync,
        AsyncLoad,
        AsyncUnload
    }

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
        GetComponentInChildren<ParticleSystem>().Stop();
        GetComponentInChildren<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeScreen(sceneTo, async));
        yield return new WaitForSeconds(1);
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

    public void EarnHearts(int numHearts)
    {
        heartsCollected += numHearts;
        Debug.Log("Player collected <color=red>" + numHearts + "</color> hearts! New total: <color=red>" + heartsCollected + "</color>");
    }
}
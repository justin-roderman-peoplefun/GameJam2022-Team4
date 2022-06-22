using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public CanvasGroup canvas;
    public List<CompanionInfo> companions;

    // Initialize the singleton instance.
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

    public void BubbleTransitionScene(string sceneTo)
    {
        StartCoroutine(TransitionScene(sceneTo));
    }

    private IEnumerator TransitionScene(string sceneTo)
    {
        GetComponentInChildren<ParticleSystem>().Stop();
        GetComponentInChildren<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeScreen(true));
        yield return new WaitForSeconds(1);
        StartCoroutine(FadeScreen(false));
        SceneManager.LoadScene(sceneTo);
    }

    private IEnumerator FadeScreen(bool fadeIn)
    {
        if (fadeIn)
        {
            canvas.gameObject.SetActive(true);
        }
        do
        {
            canvas.alpha += Time.deltaTime * (fadeIn ? 1 : -1);
            yield return null;
        } while ((fadeIn && canvas.alpha < 1) || (!fadeIn && canvas.alpha > 0));
        if (!fadeIn)
        {
            canvas.gameObject.SetActive(false);
        }
    }
}

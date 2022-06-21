using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public CanvasGroup mainMenuUI;
    public CanvasGroup introUI;

    private Transform _introTextParent;

    private IDictionary<string, Coroutine> _fadeInCoroutines;

    private bool _inIntro;
    private int _introStep;

    private void Start()
    {
        _introTextParent = introUI.transform.GetComponentInChildren<VerticalLayoutGroup>().transform;
        _fadeInCoroutines = new Dictionary<string, Coroutine>();

        StartIntro();
    }

    private void StartIntro()
    {
        StartCoroutine(FadeInCanvasGroup(introUI));

        foreach (var childText in _introTextParent.GetComponentsInChildren<TMP_Text>())
        {
            childText.alpha = 0;
        }
        _introTextParent.GetComponentsInChildren<TMP_Text>()[0].alpha = 1; // Make sure the first one is visible
        _inIntro = true;
    }

    public void StartGame()
    {
        StartCoroutine(FadeOutCanvasGroup(mainMenuUI));
    }

    private void Update()
    {
        if (_inIntro && Input.GetMouseButtonDown(0))
        {
            NextIntroStep();
        }
    }

    private void NextIntroStep()
    {
        var currIntroText = _introTextParent.GetChild(_introStep++).GetComponent<TMP_Text>();
        StartCoroutine(FadeOutText(currIntroText));

        if (_introStep < _introTextParent.childCount)
        {
            var nextIntroText = _introTextParent.GetChild(_introStep).GetComponent<TMP_Text>();
            _fadeInCoroutines[nextIntroText.transform.name] = StartCoroutine(FadeInText(nextIntroText));
        }
        else
        {
            StartCoroutine(FadeOutCanvasGroup(introUI));
            StartCoroutine(FadeInCanvasGroup(mainMenuUI));
        }
    }

    private static IEnumerator FadeInCanvasGroup(CanvasGroup fadeGroup)
    {
        return FadeCanvasGroup(true, fadeGroup);
    }

    private static IEnumerator FadeOutCanvasGroup(CanvasGroup fadeGroup)
    {
        return FadeCanvasGroup(false, fadeGroup);
    }

    private static IEnumerator FadeCanvasGroup(bool fadeIn, CanvasGroup fadeGroup)
    {
        if (fadeIn)
        {
            fadeGroup.gameObject.SetActive(true);
        }
        do
        {
            fadeGroup.alpha += Time.deltaTime * (fadeIn ? 1 : -1);
            yield return null;
        } while ((fadeIn && fadeGroup.alpha < 1) || (!fadeIn && fadeGroup.alpha > 0));
        if (!fadeIn)
        {
            fadeGroup.gameObject.SetActive(false);
        }
    }

    private IEnumerator FadeInText(TMP_Text fadeText)
    {
        return FadeText(true, fadeText);
    }

    private IEnumerator FadeOutText(TMP_Text fadeText)
    {
        return FadeText(false, fadeText);
    }

    private IEnumerator FadeText(bool fadeIn, TMP_Text fadeText)
    {
        if (!fadeIn && _fadeInCoroutines.ContainsKey(fadeText.transform.name))
        {
            StopCoroutine(_fadeInCoroutines[fadeText.transform.name]);
        }
        fadeText.alpha = fadeIn ? 0 : 1;
        do
        {
            fadeText.alpha += Time.deltaTime * (fadeIn ? 1 : -1);
            yield return null;
        } while ((fadeIn && fadeText.alpha < 1) || (!fadeIn && fadeText.alpha > 0));
    }
}

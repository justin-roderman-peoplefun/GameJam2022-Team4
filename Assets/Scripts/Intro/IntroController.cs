using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IntroController : MonoBehaviour
{
    public CanvasGroup canvas;

    private Transform _introTextParent;
    private IDictionary<string, Coroutine> _fadeInCoroutines;
    private int _introStep;

    private void Start()
    {
        _introTextParent = canvas.transform.GetComponentInChildren<VerticalLayoutGroup>().transform;
        _fadeInCoroutines = new Dictionary<string, Coroutine>();
        StartIntro();
    }

    private void StartIntro()
    {
        StartCoroutine(FadeInCanvas());

        foreach (var childText in _introTextParent.GetComponentsInChildren<TMP_Text>())
        {
            childText.alpha = 0;
        }
        _introTextParent.GetComponentsInChildren<TMP_Text>()[0].alpha = 1; // Make sure the first one is visible
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NextIntroStep();
        }
    }

    private void NextIntroStep()
    {
        if (_introStep >= _introTextParent.childCount)
        {
            return;
        }

        var currIntroText = _introTextParent.GetChild(_introStep++).GetComponent<TMP_Text>();
        StartCoroutine(FadeOutText(currIntroText));

        if (_introStep < _introTextParent.childCount)
        {
            var nextIntroText = _introTextParent.GetChild(_introStep).GetComponent<TMP_Text>();
            _fadeInCoroutines[nextIntroText.transform.name] = StartCoroutine(FadeInText(nextIntroText));
        }
        else
        {
            GameManager.Instance.TransitionScene("MainMenuScene");
        }
    }

    private IEnumerator FadeInCanvas()
    {
        do
        {
            canvas.alpha += Time.deltaTime;
            yield return null;
        } while (canvas.alpha < 1);
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

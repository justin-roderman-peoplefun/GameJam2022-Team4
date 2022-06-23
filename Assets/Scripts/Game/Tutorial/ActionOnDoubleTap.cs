using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ActionOnDoubleTap : MonoBehaviour
{
    private bool activated = false;
    
    public UnityEvent onDoubleTap;
    
    public static ActionOnDoubleTap Instance { get; private set; }
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        {
            Destroy(gameObject); 
        } 
        else 
        { 
            Instance = this; 
        }

        activated = false;
    }

    public void PerformAction()
    {
        if (!activated)
        {
            activated = true;
            Instance.onDoubleTap.Invoke();
            StartCoroutine(FadeRoutine());
        }
    }

    IEnumerator FadeRoutine()
    {
        float timer = 0f;
        TextMeshPro text = GetComponent<TextMeshPro>();
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            text.color = Color.Lerp(Color.white, Color.clear, timer / 1f);
            yield return null;
        }
        Destroy(gameObject);
    }
}

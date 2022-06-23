using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public static bool tutorialCompleted = false;

    [SerializeField] private GameObject[] deactivateForTutorial;
    [SerializeField] private GameObject[] activateForTutorial;

    [SerializeField] private TextMeshPro collectHeartText;
    [SerializeField] private SpriteRenderer tutorialHeart;
    
    [SerializeField] private TextMeshPro goalTutorialText;
    [SerializeField] private SpriteRenderer[] goalTutorialSprites;
    
    public static TutorialController Instance { get; private set; }
    private void Awake() 
    { 
        // If there is an instance, and it's not me, delete myself.
    
        if (Instance != null && Instance != this) 
        { 
            Debug.LogError("There was more than one tutorial controller in the scene. Deleting the one named: <color=cyan>" + gameObject.name + "</color>.");
            Destroy(gameObject); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }
    
    void Start()
    {
        foreach (GameObject go in deactivateForTutorial)
            go.SetActive(tutorialCompleted);
        foreach (GameObject go in activateForTutorial)
            go.SetActive(!tutorialCompleted);
    }

    public void ShowHeartTutorial()
    {
        StartCoroutine(ShowHeartTutorialRoutine());
    }

    public IEnumerator ShowHeartTutorialRoutine()
    {
        float timer = 0f;
        TextMeshPro text = GetComponent<TextMeshPro>();
        
        collectHeartText.color = Color.clear;
        if(tutorialHeart) tutorialHeart.color = Color.clear;
        collectHeartText.gameObject.SetActive(true);
        if(tutorialHeart) tutorialHeart.gameObject.SetActive(true);
        
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            collectHeartText.color = Color.Lerp(Color.clear, Color.white, timer / 1f);
            if(tutorialHeart) tutorialHeart.color = Color.Lerp(Color.clear, Color.white, timer / 1f);
            yield return null;
        }
        
        collectHeartText.color = Color.white;
        if(tutorialHeart) tutorialHeart.color = Color.white;
    }
    
    public void HideHeartTutorial()
    {
        StartCoroutine(HideHeartTutorialRoutine());
    }

    public IEnumerator HideHeartTutorialRoutine()
    {
        float timer = 0f;
        TextMeshPro text = GetComponent<TextMeshPro>();
        
        collectHeartText.color = Color.white;
        if(tutorialHeart) tutorialHeart.color = Color.white;
        collectHeartText.gameObject.SetActive(true);
        if(tutorialHeart) tutorialHeart.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            collectHeartText.color = Color.Lerp(Color.white, Color.clear, timer / 1f);
            if(tutorialHeart) tutorialHeart.color = Color.Lerp(Color.white, Color.clear, timer / 1f);
            yield return null;
        }
        
        collectHeartText.gameObject.SetActive(false);
        if(tutorialHeart) tutorialHeart.gameObject.SetActive(false);
        
        goalTutorialText.gameObject.SetActive(true);
    }
    
    public void HideGoalTutorial()
    {
        StartCoroutine(HideGoalTutorialRoutine());
    }

    public IEnumerator HideGoalTutorialRoutine()
    {
        float timer = 0f;
        TextMeshPro text = GetComponent<TextMeshPro>();
        
        goalTutorialText.color = Color.white;
        goalTutorialText.gameObject.SetActive(true);
        foreach (SpriteRenderer spr in goalTutorialSprites)
        {
            if(spr) spr.color = Color.white;
            if (spr) spr.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.3f);
        
        while (timer < 1f)
        {
            timer += Time.deltaTime;
            goalTutorialText.color = Color.Lerp(Color.white, Color.clear, timer / 1f);
            foreach (SpriteRenderer spr in goalTutorialSprites)
            {
                if(spr) spr.color = Color.Lerp(Color.white, Color.clear, timer / 1f);
            }
            yield return null;
        }
        
        goalTutorialText.gameObject.SetActive(false);
        foreach (SpriteRenderer spr in goalTutorialSprites)
        {
            if(spr) spr.gameObject.SetActive(false);
        }

        CompleteTutorial();
    }
    
    public void CompleteTutorial()
    {
        tutorialCompleted = true;
        foreach (GameObject go in deactivateForTutorial)
            go.SetActive(tutorialCompleted);
        
        Destroy(collectHeartText.gameObject);
        Destroy(goalTutorialText.gameObject);
    }
}

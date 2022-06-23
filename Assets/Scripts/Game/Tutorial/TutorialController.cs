using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public static bool tutorialCompleted = false;

    [SerializeField] private GameObject[] deactivateForTutorial;
    [SerializeField] private GameObject[] activateForTutorial;
    
    void Start()
    {
        foreach (GameObject go in deactivateForTutorial)
            go.SetActive(tutorialCompleted);
        foreach (GameObject go in activateForTutorial)
            go.SetActive(!tutorialCompleted);
    }

    public void CompleteTutorial()
    {
        tutorialCompleted = true;
    }
}

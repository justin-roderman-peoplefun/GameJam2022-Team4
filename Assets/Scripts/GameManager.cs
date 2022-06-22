using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int heartsCollected = 0;
    
    public List<CompanionInfo> Companions;

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
        return Companions.Find(info => info.key == companion.ToString().ToLower());
    }

    public void EarnHearts(int numHearts)
    {
        heartsCollected += numHearts;
        Debug.Log("Player collected <color=red>" + numHearts + "</color> hearts! New total: <color=red>" + heartsCollected + "</color>");
    }
}
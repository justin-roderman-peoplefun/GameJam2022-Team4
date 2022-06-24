using System;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayMusic(MusicSongs.Title);
    }

    public void StartGame()
    {
        GameManager.Instance.BubbleTransitionScene("CompanionScene");
    }
}

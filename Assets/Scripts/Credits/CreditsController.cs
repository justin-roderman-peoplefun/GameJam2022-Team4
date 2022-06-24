using System;
using UnityEngine;

public class CreditsController : MonoBehaviour
{
    private void Start()
    {
        SoundManager.Instance.PlayMusic(MusicSongs.Wellerman);
    }

    public void MainMenuButtonCallback()
    {
        GameManager.Instance.BackToMainMenu();
    }
}

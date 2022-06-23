using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        GameManager.Instance.ResetVariables();
        GameManager.Instance.BubbleTransitionScene("CompanionScene");
    }
}

using UnityEngine;

public class CreditsController : MonoBehaviour
{
    public void MainMenuButtonCallback()
    {
        GameManager.Instance.BackToMainMenu();
    }
}

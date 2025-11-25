using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        // Load the main game scene
        SceneManager.LoadScene("Level 0");
    }

    public void OnTutorialButtonClicked()
    {
        // Load tutorial
        SceneManager.LoadScene("Tutorial");
    }

    public void OnBackButtonClicked()
    {
        // Go back to main menu
        SceneManager.LoadScene("Main Menu");
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        // Load the main game scene
        SceneManager.LoadScene("Level 0");
    }
}
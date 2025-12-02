using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartScreenManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSfx;

    public void OnStartButtonClicked()
    {
        // Load First Level
        StartCoroutine(LoadAfterClick("Level 0"));
    }

    public void OnTutorialButtonClicked()
    {
        // Load Tutorial
        StartCoroutine(LoadAfterClick("Tutorial"));
    }

    public void OnBackButtonClicked()
    {
        // Load Main Menu
        StartCoroutine(LoadAfterClick("Main Menu"));
    }

    private IEnumerator LoadAfterClick(string sceneName)
    {
        if (audioSource != null && clickSfx != null)
            audioSource.PlayOneShot(clickSfx);

        // Slight delay for audio to play
        yield return new WaitForSeconds(0.07f);

        SceneManager.LoadScene(sceneName);
    }
}
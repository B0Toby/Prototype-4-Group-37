using UnityEngine;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;

    public AudioSource musicSource;
    public float fadeDuration = 1.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // survives scene changes
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // fade out music
    public void FadeOutAndStop()
    {
        if (musicSource != null)
            StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        float startVolume = musicSource.volume;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume; // reset for next time
    }
}

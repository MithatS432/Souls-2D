using UnityEngine;
using System.Collections;
public class MusicAmbientManager : MonoBehaviour
{
    [Header("Component References")]
    private AudioSource audioSource;

    [Header("Music Clips")]
    [SerializeField] private AudioClip templeMusic;
    [SerializeField] private AudioClip forestMusic;
    [SerializeField] private AudioClip kingdomMusic;
    [SerializeField] private AudioClip frozenMusic;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField][Range(0f, 1f)] private float maxVolume = 0.7f;

    private string currentArea = "Temple";
    private Coroutine fadeCoroutine;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;
        audioSource.volume = maxVolume;
        audioSource.clip = templeMusic;
        audioSource.Play();
    }

    public void ChangeAreaMusic(string areaName)
    {
        if (currentArea == areaName) return;

        AudioClip newClip = null;

        switch (areaName)
        {
            case "Temple":
                newClip = templeMusic;
                break;
            case "Forest":
                newClip = forestMusic;
                break;
            case "Kingdom":
                newClip = kingdomMusic;
                break;
            case "Frozen":
                newClip = frozenMusic;
                break;
            default:
                Debug.LogWarning("Bilinmeyen bölge: " + areaName);
                return;
        }

        if (newClip != null && newClip != audioSource.clip)
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeMusic(newClip));
            currentArea = areaName;
        }
    }

    IEnumerator FadeMusic(AudioClip newClip)
    {
        float startVolume = audioSource.volume;
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();

        audioSource.clip = newClip;
        audioSource.Play();

        time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, maxVolume, time / fadeDuration);
            yield return null;
        }

        audioSource.volume = maxVolume;
        fadeCoroutine = null;
    }
}
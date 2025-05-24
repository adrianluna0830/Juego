using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioSourcePoolManager : MonoBehaviour
{
    public static AudioSourcePoolManager Instance { get; private set; }

    [SerializeField] private int initialPoolSize = 10;
    [SerializeField] private GameObject audioSourcePrefab;

    private List<AudioSource> availableSources;
    private List<AudioSource> inUseSources;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        availableSources = new List<AudioSource>(initialPoolSize);
        inUseSources = new List<AudioSource>(initialPoolSize);

        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewAudioSource();
        }
    }

    private AudioSource CreateNewAudioSource()
    {
        GameObject newObj;

        if (audioSourcePrefab != null)
        {
            newObj = Instantiate(audioSourcePrefab, transform);
        }
        else
        {
            newObj = new GameObject("PooledAudioSource");
            newObj.transform.SetParent(transform);
            newObj.AddComponent<AudioSource>();
        }

        AudioSource newAudioSource = newObj.GetComponent<AudioSource>();
        newAudioSource.playOnAwake = false;
        newObj.SetActive(false);

        availableSources.Add(newAudioSource);
        return newAudioSource;
    }

    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f, float minPitch = 1f, float maxPitch = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip nulo al intentar reproducir sonido.");
            return;
        }

        AudioSource audioSource = GetAvailableAudioSource();

        audioSource.transform.position = position;
        audioSource.volume = volume;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.clip = clip;

        audioSource.gameObject.SetActive(true);
        audioSource.Play();

        StartCoroutine(ReturnAudioSourceWhenDone(audioSource));
    }

    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip nulo al intentar reproducir sonido.");
            return;
        }

        AudioSource audioSource = GetAvailableAudioSource();

        audioSource.transform.position = position;
        audioSource.volume = volume;
        audioSource.clip = clip;

        audioSource.gameObject.SetActive(true);
        audioSource.Play();

        StartCoroutine(ReturnAudioSourceWhenDone(audioSource));
    }

    private AudioSource GetAvailableAudioSource()
    {
        AudioSource source;

        if (availableSources.Count > 0)
        {
            source = availableSources[0];
            availableSources.RemoveAt(0);
        }
        else
        {
            source = CreateNewAudioSource();
        }

        inUseSources.Add(source);
        return source;
    }

    private IEnumerator ReturnAudioSourceWhenDone(AudioSource audioSource)
    {
        yield return new WaitWhile(() => audioSource.isPlaying);

        audioSource.Stop();
        audioSource.gameObject.SetActive(false);

        inUseSources.Remove(audioSource);
        availableSources.Add(audioSource);
    }
}

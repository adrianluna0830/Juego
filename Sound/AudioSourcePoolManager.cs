using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioSourcePoolManager : MonoBehaviour
{
    // Instancia estática para implementar el patrón Singleton
    public static AudioSourcePoolManager Instance { get; private set; }

    // Tamaño inicial de la pool (puedes ajustarlo según tus necesidades)
    [SerializeField] private int initialPoolSize = 10;

    // Prefab opcional que contenga un AudioSource configurado (no es obligatorio)
    [SerializeField] private GameObject audioSourcePrefab;

    // Listas para manejar AudioSources disponibles y en uso
    private List<AudioSource> availableSources;
    private List<AudioSource> inUseSources;

    private void Awake()
    {
        // Configuramos el Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Inicializamos las listas y creamos algunas fuentes de audio
        availableSources = new List<AudioSource>(initialPoolSize);
        inUseSources = new List<AudioSource>(initialPoolSize);

        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewAudioSource();
        }
    }

    /// <summary>
    /// Crea un nuevo AudioSource y lo agrega a la lista de disponibles.
    /// </summary>
    private AudioSource CreateNewAudioSource()
    {
        GameObject newObj;

        // Si se ha asignado un prefab, lo instanciamos; si no, creamos un GameObject vacío
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

    /// <summary>
    /// Solicita la reproducción de un sonido con volumen, pitch mínimo y pitch máximo.
    /// </summary>
    /// <param name="clip">AudioClip a reproducir.</param>
    /// <param name="position">Posición en el espacio donde se reproducirá el sonido.</param>
    /// <param name="volume">Volumen de 0.0 a 1.0.</param>
    /// <param name="minPitch">Pitch mínimo (ej. 0.9).</param>
    /// <param name="maxPitch">Pitch máximo (ej. 1.1).</param>
    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f, float minPitch = 1f, float maxPitch = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip nulo al intentar reproducir sonido.");
            return;
        }

        // Tomamos un AudioSource de la pool
        AudioSource audioSource = GetAvailableAudioSource();

        // Configuramos el AudioSource
        audioSource.transform.position = position;
        audioSource.volume = volume;
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.clip = clip;

        // Activamos el GameObject y reproducimos
        audioSource.gameObject.SetActive(true);
        audioSource.Play();

        // Iniciamos la rutina para regresar el AudioSource a la pool cuando termine
        StartCoroutine(ReturnAudioSourceWhenDone(audioSource));
    }
    
    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioClip nulo al intentar reproducir sonido.");
            return;
        }

        // Tomamos un AudioSource de la pool
        AudioSource audioSource = GetAvailableAudioSource();

        // Configuramos el AudioSource
        audioSource.transform.position = position;
        audioSource.volume = volume;
        audioSource.clip = clip;

        // Activamos el GameObject y reproducimos
        audioSource.gameObject.SetActive(true);
        audioSource.Play();

        // Iniciamos la rutina para regresar el AudioSource a la pool cuando termine
        StartCoroutine(ReturnAudioSourceWhenDone(audioSource));
    }

    /// <summary>
    /// Obtiene un AudioSource de la lista de disponibles; si no hay, crea uno nuevo.
    /// </summary>
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

    /// <summary>
    /// Corrutina que espera a que el sonido termine de reproducirse para regresar el AudioSource a la pool.
    /// </summary>
    private IEnumerator ReturnAudioSourceWhenDone(AudioSource audioSource)
    {
        // Esperamos mientras el sonido se está reproduciendo
        yield return new WaitWhile(() => audioSource.isPlaying);

        // Detenemos y desactivamos el GameObject asociado
        audioSource.Stop();
        audioSource.gameObject.SetActive(false);

        // Lo retiramos de la lista de en uso y lo agregamos a la de disponibles
        inUseSources.Remove(audioSource);
        availableSources.Add(audioSource);
    }
}

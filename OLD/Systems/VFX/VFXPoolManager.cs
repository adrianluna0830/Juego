using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// VFXPoolManager gestiona instancias de ParticleSystems según el prefab 
/// que se pase por parámetro, para evitar la creación y destrucción constante.
/// </summary>
public class VFXPoolManager : MonoBehaviour
{
    // Singleton para acceso global
    public static VFXPoolManager Instance { get; private set; }

    /// <summary>
    /// Diccionario principal que relaciona cada prefab (clave) con una lista de ParticleSystems disponibles.
    /// </summary>
    private Dictionary<ParticleSystem, List<ParticleSystem>> pooledEffects 
        = new Dictionary<ParticleSystem, List<ParticleSystem>>();

    /// <summary>
    /// Este diccionario nos permite saber de qué prefab proviene una instancia en uso, 
    /// para así regresarla al lugar correcto en pooledEffects.
    /// </summary>
    private Dictionary<ParticleSystem, ParticleSystem> instanceToPrefab 
        = new Dictionary<ParticleSystem, ParticleSystem>();

    private void Awake()
    {
        // Configuramos el patrón Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Reproduce un ParticleSystem en la posición indicada. Se hace uso de la pool 
    /// para reutilizar instancias sin crearlas o destruirlas constantemente.
    /// </summary>
    /// <param name="effectPrefab">El prefab del ParticleSystem que quieres reproducir.</param>
    /// <param name="position">La posición en el mundo donde aparecerá el efecto.</param>
    public ParticleSystem PlayEffect(ParticleSystem effectPrefab, Vector3 position)
    {
        if (effectPrefab == null)
        {
            Debug.LogWarning("Se intentó reproducir un efecto nulo.");
            return null;
        }

        // Obtenemos (o creamos) una instancia de la pool
        ParticleSystem effectInstance = GetEffectFromPool(effectPrefab);

        // Ajustamos posición y activamos
        effectInstance.transform.position = position;
        effectInstance.gameObject.SetActive(true);

        // Reproducimos
        effectInstance.Play();

        // Iniciamos la corrutina para regresar el efecto al terminar
        StartCoroutine(ReturnEffectWhenDone(effectInstance));

        return effectInstance;
    }
    
    public void PlayEffect(GameObject effectPrefab, Vector3 position)
    {
   
        Instantiate(effectPrefab,position,Quaternion.identity);

    }

    /// <summary>
    /// Si quieres precargar/precalentar (warm up) un número específico de instancias de un prefab
    /// para evitar costos durante el juego, puedes llamar a este método al inicio.
    /// </summary>
    /// <param name="effectPrefab">Prefab del ParticleSystem a precargar.</param>
    /// <param name="count">Cantidad de instancias a crear.</param>
    public void PrewarmEffect(ParticleSystem effectPrefab, int count)
    {
        if (effectPrefab == null) 
        {
            Debug.LogWarning("Se intentó precargar un prefab nulo.");
            return;
        }

        // Aseguramos que exista la lista en el diccionario
        if (!pooledEffects.ContainsKey(effectPrefab))
        {
            pooledEffects[effectPrefab] = new List<ParticleSystem>();
        }

        // Creamos las instancias adicionales
        for (int i = 0; i < count; i++)
        {
            ParticleSystem newPS = Instantiate(effectPrefab, transform);
            newPS.gameObject.SetActive(false);

            // Asignamos la relación instancia -> prefab
            instanceToPrefab[newPS] = effectPrefab;

            // Lo agregamos a la lista de disponibles
            pooledEffects[effectPrefab].Add(newPS);
        }
    }

    /// <summary>
    /// Obtiene un ParticleSystem de la pool. Si no hay ninguno disponible, se crea uno nuevo.
    /// </summary>
    private ParticleSystem GetEffectFromPool(ParticleSystem effectPrefab)
    {
        // Si aún no tenemos registro para este prefab, creamos la lista
        if (!pooledEffects.ContainsKey(effectPrefab))
        {
            pooledEffects[effectPrefab] = new List<ParticleSystem>();
        }

        // Buscamos si hay alguno disponible
        List<ParticleSystem> availableList = pooledEffects[effectPrefab];
        ParticleSystem instance = null;

        if (availableList.Count > 0)
        {
            // Obtenemos la primera instancia disponible
            instance = availableList[0];
            availableList.RemoveAt(0);
        }
        else
        {
            // No hay instancias libres, creamos una nueva
            instance = Instantiate(effectPrefab, transform);
            instanceToPrefab[instance] = effectPrefab; 
        }

        return instance;
    }

    /// <summary>
    /// Corrutina que regresa el ParticleSystem a la pool una vez que termine su emisión y 
    /// las partículas hayan desaparecido.
    /// </summary>
    private IEnumerator ReturnEffectWhenDone(ParticleSystem effectInstance)
    {
        // Esperamos hasta que el sistema de partículas esté completamente inactivo
        // (usamos true para contar también los sub-emisores si existen)
        yield return new WaitUntil(() => !effectInstance.IsAlive(true));

        // Detenemos y limpiamos el sistema
        effectInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        effectInstance.gameObject.SetActive(false);

        // Regresamos la instancia a la lista de disponibles correspondiente a su prefab
        ParticleSystem originalPrefab = instanceToPrefab[effectInstance];
        pooledEffects[originalPrefab].Add(effectInstance);
    }
}

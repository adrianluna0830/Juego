using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatContext : MonoBehaviour
{
    public static CombatContext Instance { get; private set; }

    [SerializeField] private Transform player;

    // Variables serializables para configurar los anillos
    [SerializeField] private float startRingDistance = 3f; // Distancia para el primer anillo
    [SerializeField] private float ringStep = 3f; // Incremento de distancia entre anillos
    [SerializeField] private int startSlots = 2; // Slots del primer anillo
    [SerializeField] private int slotStep = 2; // Incremento de slots para anillos sucesivos
    [SerializeField] private int ringCount = 3; // Cantidad de anillos
    public RingManager _ringManager;
    private List<Transform> enemies = new List<Transform>();
    public List<Transform> Enemies => enemies;


    // Maneja la lógica de los anillos

    private void Awake()
    {
        // Singleton sencillo
        if (Instance != null && Instance != this)
        {
            Debug.LogError("Multiple instances of CombatContext detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
     

        Instance = this;
    }
    
    public void AddEnemey(Transform enemyTransform)
    {
        enemies.Add(enemyTransform);
        _ringManager.AgregarTransform(enemyTransform);
        // Cuando muera, se retiran de ambos sitios
        enemyTransform.GetComponent<Health>().OnDeath += (() =>
        {
            _ringManager.RemoverTransform(enemyTransform);
            enemies.Remove(enemyTransform);
        });
    }

    public Vector3 GetDirectionToPlayer(Vector3 position)
    {
        return player.position - position;
    }


    public Vector3 GetPlayerPosition()
    {
        return player.position;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatContext : MonoBehaviour
{
    public static CombatContext Instance { get; private set; }

    [SerializeField] private Transform player;

    [SerializeField] private float startRingDistance = 3f; 
    [SerializeField] private float ringStep = 3f; 
    [SerializeField] private int startSlots = 2; 
    [SerializeField] private int slotStep = 2; 
    [SerializeField] private int ringCount = 3; 
    public RingManager _ringManager;
    private List<Transform> enemies = new List<Transform>();
    public List<Transform> Enemies => enemies;



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
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private GameObject enemyPrefab;

    [SerializeField] private int enemeySpawnStep;
    [SerializeField] private int enemiesToSpawn;
    [SerializeField] private float distanceBetweenSpawn;
    [SerializeField] private float angleVariation;
    
    [SerializeField] private CombatContext combatContext;

    private void Start()
    {
        DontDestroyOnLoad(this);

        EnemySpawner.SpawnEnemiesRandom(enemiesToSpawn, distanceBetweenSpawn, angleVariation, spawnPosition.position, enemyPrefab,OnEnemySpawned);
    }
    
    int spawnIndex = 0;
    public void OnEnemySpawned(GameObject gameObject)
    {
        combatContext.AddEnemey(gameObject.transform);
    }
}
using UnityEngine;

public class WeaponSpawnPosition : MonoBehaviour
{
    [SerializeField] private SpawnPosition  spawnPosition;
    
    public SpawnPosition GetSpawnPosition() => spawnPosition;
}
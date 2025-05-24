using UnityEngine;
using System;

public static class EnemySpawner
{
    public static void SpawnEnemiesRandom(
        int enemyCount,
        float distanceBetweenEnemies,
        float angleVariation,
        Vector3 spawnCenter,
        GameObject enemyPrefab,
        Action<GameObject> onEnemySpawned = null
    )
    {
        float angleStep = 360f / enemyCount;

        for (int i = 0; i < enemyCount; i++)
        {
            float baseAngle = i * angleStep;
            float randomizedAngle = baseAngle + UnityEngine.Random.Range(-angleVariation, angleVariation);
            float angleInRadians = randomizedAngle * Mathf.Deg2Rad;

            Vector3 direction = new Vector3(
                Mathf.Cos(angleInRadians),
                0,
                Mathf.Sin(angleInRadians)
            );

            Vector3 spawnPosition = spawnCenter;
            bool positionFound = false;

            while (!positionFound)
            {
                if (IsValidSpawnPosition(spawnPosition, distanceBetweenEnemies))
                {
                    positionFound = true;
                }
                else
                {
                    spawnPosition += direction * distanceBetweenEnemies;
                }
            }

            GameObject spawnedEnemy = GameObject.Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            onEnemySpawned?.Invoke(spawnedEnemy);
        }
    }

    private static bool IsValidSpawnPosition(Vector3 position, float distanceBetweenEnemies)
    {
        var colliders = Physics.OverlapSphere(position, distanceBetweenEnemies * 0.5f);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("CharacterCore"))
                return false;
        }

        return true;
    }
}
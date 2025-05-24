using UnityEngine;
using System;

public static class EnemySpawner
{
    /// <summary>
    /// Spawns enemies in a circular pattern with calculated angles and random variation.
    /// </summary>
    /// <param name="enemyCount">The number of enemies to spawn. Recommended value: Between 1 and 50 for performance scalability.</param>
    /// <param name="distanceBetweenEnemies">The minimum distance between enemies to avoid overlap. Recommended value: At least 1.0f.</param>
    /// <param name="angleVariation">Maximum random angle variation in degrees to apply to each spawn position. Higher values create more randomness.</param>
    /// <param name="spawnCenter">The central point from which enemies will spawn.</param>
    /// <param name="enemyPrefab">The enemy prefab to instantiate. Ensure it has proper colliders and tagging as "CharacterCore".</param>
    /// <param name="onEnemySpawned">Optional callback that is invoked with the spawned enemy GameObject. Can be null.</param>
    public static void SpawnEnemiesRandom(
        int enemyCount,
        float distanceBetweenEnemies,
        float angleVariation,
        Vector3 spawnCenter,
        GameObject enemyPrefab,
        Action<GameObject> onEnemySpawned = null
    )
    {
        // Calculate the base angle step
        float angleStep = 360f / enemyCount;

        for (int i = 0; i < enemyCount; i++)
        {
            // Calculate base angle
            float baseAngle = i * angleStep;

            // Apply random variation to angle within the specified range
            float randomizedAngle = baseAngle + UnityEngine.Random.Range(-angleVariation, angleVariation);

            // Convert to radians for position calculation
            float angleInRadians = randomizedAngle * Mathf.Deg2Rad;

            Vector3 direction = new Vector3(
                Mathf.Cos(angleInRadians),
                0,
                Mathf.Sin(angleInRadians)
            );

            Vector3 spawnPosition = spawnCenter;
            bool positionFound = false;
            int attempts = 0;

            while (!positionFound)
            {
                attempts++;

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

            // Invoke the callback with the spawned enemy if it's not null
            onEnemySpawned?.Invoke(spawnedEnemy);
        }
    }

    private static bool IsValidSpawnPosition(
        Vector3 position,
        float distanceBetweenEnemies)
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
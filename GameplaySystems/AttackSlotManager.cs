using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AttackSlotManager : MonoBehaviour
{
    [SerializeField]
    private float alignmentWeight = 0.5f;

    [SerializeField]
    private float rangeWeight = 0.8f;

    [SerializeField]
    private float healthWeight = 1.2f;

    [SerializeField]
    private float requiredMinimumWeight = 0.5f;

    [SerializeField]
    private int maxSlots = 3;

    [SerializeField]
    private float slotRecoveryTime = 2f;

    [SerializeField]
    private float attackCooldownTime = 4f;

    [SerializeField]
    private float slotAssignmentInterval = 2f;

    [SerializeField]
    private Transform player;

    public int currentSlots;

    private float slotRecoveryTimer;
    private float slotAssignmentTimer;
    private readonly Dictionary<EnemyCombat, float> enemyCooldowns = new Dictionary<EnemyCombat, float>();

    private const float MAX_HEALTH_REFERENCE = 100f;

    private class EnemyWeightData
    {
        public EnemyCombat Enemy;
        public float TotalWeight;
    }

    private void Start()
    {
        currentSlots = maxSlots;
        Debug.Log($"[AttackSlotManager] Slots disponibles al inicio: {currentSlots}/{maxSlots}");
    }

    private void Update()
    {
        UpdateCooldowns();
        RegenerateSlots();

        slotAssignmentTimer += Time.deltaTime;
        while (slotAssignmentTimer >= slotAssignmentInterval)
        {
            slotAssignmentTimer -= slotAssignmentInterval;
            AttemptSingleSlotAssignment();
        }
    }

    private void AttemptSingleSlotAssignment()
    {
        if (currentSlots <= 0)
        {
            Debug.Log("[AttemptSingleSlotAssignment] No hay slots disponibles para asignar.");
            return;
        }

        if (player == null)
        {
            Debug.LogWarning("[AttemptSingleSlotAssignment] Falta la referencia al jugador.");
            return;
        }

        var combatEnemies = CombatContext.Instance.Enemies;
        var weightedEnemies = new List<EnemyWeightData>();

        var playerPosition = player.position;
        var playerForward = player.forward;

        foreach (var enemyTransform in combatEnemies)
        {
            if (enemyTransform == null)
                continue;

            var enemyCombat = enemyTransform.GetComponent<EnemyCombat>();
            if (enemyCombat == null)
                continue;

            if (enemyCombat.attacking || enemyCombat.isInHitState || IsInCooldown(enemyCombat))
                continue;

            float totalWeight = CalculateEnemyWeight(enemyTransform, playerPosition, playerForward);
            weightedEnemies.Add(new EnemyWeightData
            {
                Enemy = enemyCombat,
                TotalWeight = totalWeight
            });
        }

        if (weightedEnemies.Count == 0)
        {
            Debug.Log("[AttemptSingleSlotAssignment] No hay enemigos elegibles para asignar slot.");
            return;
        }

        var bestEnemy = weightedEnemies.OrderByDescending(x => x.TotalWeight).First();

        if (bestEnemy.TotalWeight < requiredMinimumWeight)
        {
            Debug.Log(
                $"[AttemptSingleSlotAssignment] El enemigo con mayor peso ({bestEnemy.TotalWeight:F2}) " +
                $"NO supera el peso mínimo requerido ({requiredMinimumWeight:F2}). " +
                $"No se asignará slot en este intervalo."
            );
            return;
        }

        AssignSingleSlot(bestEnemy);
    }

    private void AssignSingleSlot(EnemyWeightData enemyData)
    {
        enemyData.Enemy.StartAttacking();
        StartCooldown(enemyData.Enemy);
        currentSlots--;

        Debug.Log($"[AssignSingleSlot] Asignado slot a {enemyData.Enemy.name}. " +
                  $"Slots restantes: {currentSlots}/{maxSlots}. " +
                  $"Peso total: {enemyData.TotalWeight:F2}");
    }

    private void UpdateCooldowns()
    {
        if (enemyCooldowns.Count == 0) return;

        var keys = enemyCooldowns.Keys.ToList();

        foreach (var enemy in keys)
        {
            if (enemy == null)
            {
                enemyCooldowns.Remove(enemy);
                continue;
            }

            enemyCooldowns[enemy] -= Time.deltaTime;

            if (enemyCooldowns[enemy] <= 0f)
            {
                if (enemy != null)
                {
                    Debug.Log($"[Cooldown] Enemigo {enemy.name} sale de cooldown.");
                }
                else
                {
                    Debug.Log("[Cooldown] Un enemigo destruido ha salido de cooldown.");
                }

                enemyCooldowns.Remove(enemy);
            }
        }
    }

    private bool IsInCooldown(EnemyCombat enemy)
    {
        bool inCooldown = enemyCooldowns.TryGetValue(enemy, out float timeLeft) && timeLeft > 0f;
        if (inCooldown)
        {
            Debug.Log($"[IsInCooldown] {enemy.name} está en cooldown. Tiempo restante: {timeLeft:F1}s");
        }

        return inCooldown;
    }

    private void StartCooldown(EnemyCombat enemy)
    {
        enemyCooldowns[enemy] = attackCooldownTime;
        Debug.Log($"[StartCooldown] Iniciado cooldown en {enemy.name} por {attackCooldownTime}s.");
    }

    private float CalculateEnemyWeight(Transform enemy, Vector3 playerPos, Vector3 playerForward)
    {
        Vector3 directionToEnemy = (enemy.position - playerPos).normalized;

        float alignmentDot = Vector3.Dot(playerForward, directionToEnemy);
        float alignmentScore = (1f - alignmentDot) * alignmentWeight;

        float distance = Vector3.Distance(playerPos, enemy.position);
        float maxRange = 20f;
        float rangeScore = (1f - Mathf.Clamp01(distance / maxRange)) * rangeWeight;

        float healthScore = 0f;
        var enemyHealth = enemy.GetComponent<Health>();
        if (enemyHealth != null)
        {
            float normalizedHealth = Mathf.Clamp01(enemyHealth._currentHits / MAX_HEALTH_REFERENCE);
            healthScore = normalizedHealth * healthWeight;
        }

        return alignmentScore + rangeScore + healthScore;
    }

    private void RegenerateSlots()
    {
        if (currentSlots >= maxSlots)
            return;

        slotRecoveryTimer += Time.deltaTime;
        if (slotRecoveryTimer >= slotRecoveryTime)
        {
            currentSlots++;
            slotRecoveryTimer = 0f;
            Debug.Log($"[RegenerateSlots] Un slot regenerado. Total: {currentSlots}/{maxSlots}");
        }
    }
}

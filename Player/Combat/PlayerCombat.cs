using System;
using UnityEngine;
using System.Collections.Generic;
using Animancer;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine.Serialization;
using System.Collections;
using Unity.VisualScripting;

public class PlayerCombat : MonoBehaviour, IAttackStatusNotifier
{
    [SerializeField] private SoundCollection leapAudio;
    [SerializeField] private Combo[] combos;
    [SerializeField] private AnimancerComponent animancer;
    [SerializeField] private StringAsset HitEventName;
    [SerializeField] private StringAsset SwooshEventName;
    [SerializeField] private float canAttackCooldown;
    [SerializeField] private float maxLeapDistance;
    [SerializeField] private float maxLeapAngle;
    [SerializeField] private float leapDurationCoefficient;
    [SerializeField] private float leapDestinationOffset;
    [SerializeField] private float attackRadius;
    [SerializeField] private float attackAngle;
    [SerializeField] private float keepComboCooldown;
    [SerializeField] private float timeScaleChangeRate = 1f;
    [SerializeField] private float targetTimeScale = 0.5f;

    private ComboManager _comboManager;
    public bool CanAttack { get; private set; } = true;

    public event Action OnAttackEndEvent;

    private IHitManager GetCounterableTargetInRange()
    {
        List<IHitManager> enemiesInRange = TargetsUtils.GetObjectsInRadius(transform.position, maxLeapDistance);
        enemiesInRange = enemiesInRange.FindAll(enemy =>
            enemy.Transform.CompareTag("CharacterCore") &&
            enemy.Transform != transform &&
            enemy.CanBeHit());

        if (enemiesInRange.Count == 0)
            return null;

        foreach (var enemy in enemiesInRange)
        {
            EnemyCombat enemyCombat = enemy.Transform.GetComponent<EnemyCombat>();
            if (enemyCombat != null && enemyCombat.canCounter)
            {
                return enemy;
            }
        }
        return null;
    }

    public bool CanDoCounter()
    {
        IHitManager target = GetCounterableTargetInRange();
        return target != null;
    }

    public void DoCounter()
    {
        IHitManager target = GetCounterableTargetInRange();
        if (target == null) return;
        target.Transform.GetComponent<EnemyCombat>().Counter();
        TryLeapTowards(target);
        Vector3 directionToTarget = target.Transform.position - transform.position;
        DoAttack(directionToTarget);
    }

    private void Awake()
    {
        _comboManager = new ComboManager(keepComboCooldown, combos);
    }

    private void OnEnable()
    {
        OnAttackSwoosh += CheckEnemyHealthAndAdjustTime;
    }

    private void OnDisable()
    {
        OnAttackSwoosh -= CheckEnemyHealthAndAdjustTime;
    }

    private void CheckEnemyHealthAndAdjustTime()
    {
        var target = GetTarget(transform.forward, attackRadius, attackAngle);
        if (target == null) return;

        var health = target.Transform.GetComponent<Health>();
        if (health == null) return;

        if (health._currentHits == 1)
        {
            DOTween.Kill("TimeScaleTween");
            DOTween.To(() => Time.timeScale,
                       value => Time.timeScale = value,
                       targetTimeScale,
                       timeScaleChangeRate)
                   .SetId("TimeScaleTween")
                   .SetEase(Ease.OutQuad);
        }
    }

    public void TryLeapAndAttack(Vector3 direction)
    {
        IHitManager target = GetTarget(direction, maxLeapDistance, maxLeapAngle);
        if (target != null)
            TryLeapTowards(target);

        DoAttack(direction);
    }

    private IHitManager GetTarget(Vector3 referenceDirection, float radius, float viewAngle)
    {
        List<IHitManager> enemiesInRange = TargetsUtils.GetObjectsInRadius(transform.position, radius);
        enemiesInRange = enemiesInRange.FindAll(enemy =>
            enemy.Transform.CompareTag("CharacterCore") &&
            enemy.Transform != transform &&
            enemy.CanBeHit());

        if (enemiesInRange.Count == 0) return null;

        float halfViewAngle = viewAngle / 2f;

        enemiesInRange = enemiesInRange.FindAll(enemy =>
        {
            Vector3 directionToEnemy = enemy.Transform.position - transform.position;
            directionToEnemy.y = 0;
            directionToEnemy.Normalize();

            Vector3 normalizedDirection = referenceDirection;
            normalizedDirection.y = 0;
            normalizedDirection.Normalize();

            float angleToEnemy = Vector3.Angle(normalizedDirection, directionToEnemy);
            return angleToEnemy <= halfViewAngle;
        });

        if (enemiesInRange.Count == 0) return null;

        return TargetsUtils.GetClosestObjectByAngle(enemiesInRange, transform.position, referenceDirection);
    }

    private bool TryLeapTowards(IHitManager target)
    {
        if (target == null) return false;

        Vector3 directionToTarget = (transform.position - target.Transform.position).normalized;
        Vector3 endValue = target.Transform.position + directionToTarget * leapDestinationOffset;
        endValue.y = 0;

        float duration = Vector3.Distance(transform.position, endValue) / leapDurationCoefficient;

        transform.DOKill();
        transform.DOMove(endValue, duration);

        Vector3 lookDirection = (target.Transform.position - transform.position).normalized;
        lookDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDirection);

        var sound = leapAudio.GetRandomClipWithVariation();
        AudioSourcePoolManager.Instance.PlaySound(sound.clip, transform.position, sound.volume, sound.pitch);

        return true;
    }

    private IHitManager _lastHitManager;
    private AnimancerState currentAttackState;

    private void DoAttack(Vector3 direction)
    {
        CanAttack = false;

        bool isLastAttack = _comboManager.IsCurrentAttackLastInCombo();
        var newHit = GetTarget(direction, maxLeapDistance, maxLeapAngle);
        Attack newAttack = null;

        if (_lastHitManager != null && _lastHitManager == newHit)
        {
            _comboManager.GetAttackAndUpdateIndex(out var attack);
            newAttack = attack;
        }
        else
        {
            _comboManager.UpdateComboIndex();
            newAttack = _comboManager.GetRandomAttack();
        }

        _lastHitManager = newHit;

        currentAttackState = animancer.Play(newAttack.animation, 0.05f, FadeMode.FromStart);
        Action checkHit = () => CheckHit(isLastAttack);

        currentAttackState.Events(this).SetCallback(HitEventName, checkHit);
        currentAttackState.Events(this).SetCallback(SwooshEventName, () => OnAttackSwoosh?.Invoke());
        currentAttackState.Events(this).OnEnd = OnEndAttack;

        OnAttackStart?.Invoke();
    }

    private void CheckHit(bool isLastAttack)
    {
        Time.timeScale = 1;
        var target = GetTarget(transform.forward, attackRadius, attackAngle);
        if (target != null)
        {
            HitContext hitContext = new HitContext();
            target.Hit(hitContext);
            target.Transform.rotation = Quaternion.LookRotation(transform.position - target.Transform.position);

            var hitDealtEvent = new AttackInfo
            {
                attackAnimationState = currentAttackState
            };
            OnHitDealtEvent?.Invoke(hitDealtEvent);
        }

        if (!isLastAttack) StartCoroutine(CanAttackCooldownCorutine());

        IEnumerator CanAttackCooldownCorutine()
        {
            yield return new WaitForSeconds(canAttackCooldown);
            CanAttack = true;
        }
    }

    private void OnEndAttack()
    {
        CanAttack = true;
        OnAttackEndEvent?.Invoke();
    }

    public void InterruptAttack()
    {
        Time.timeScale = 1;
        StopAllCoroutines();
        CanAttack = true;
    }

    public event Action OnAttackStart;
    public event Action OnAttackSwoosh;
    public event Action<AttackInfo> OnHitDealtEvent;
}

public interface IAttackStatusNotifier
{
    event Action OnAttackStart;
    event Action OnAttackSwoosh;
    event Action<AttackInfo> OnHitDealtEvent;
}

public struct AttackInfo
{
    public AnimancerState attackAnimationState;
}

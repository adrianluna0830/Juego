using System;
using System.Collections;
using System.Collections.Generic;
using Animancer;
using DefaultNamespace;
using UnityEngine;

public class EnemyCombat : MonoBehaviour, IAttackStatusNotifier
{
    [Header("Attack Range")] [SerializeField]
    private float distanceToAttack;

    [Header("Combo Settings")] [SerializeField]
    private Combo[] combos;

    [Header("Animation Settings")] [SerializeField]
    public AnimancerComponent animancer;

    [Header("Hit Event Settings")] [SerializeField]
    private StringAsset HitEventName;

    [SerializeField] private StringAsset SwooshEventName;

    [Header("Cooldown Settings")] [SerializeField]
    private float canAttackCooldown; 

    [Header("Attack Settings")] [SerializeField]
    private float attackRadius;
    
    [Header("Attack Settings")] [SerializeField]
    public float distanceToShowCounter;

    [Header("Attack Settings")] [SerializeField]
    public Counter _counter;

    public bool canCounter = false;


    public event Action OnAttackStart;
    public event Action OnAttackSwoosh;
    public event Action<AttackInfo> OnHitDealtEvent;


    public event Action OnAttackEndEvent;
    public event Action OnComboFinished;

    private AnimancerState _currentAttackState;
    public bool _canAttack = true;
    private Coroutine _comboCoroutine;


    public bool attacking;

    public bool isInHitState = false;
    public void StartAttacking()
    {
        startAttackphase?.Invoke();
    }

    public event Action startAttackphase;
    public event Action onCounter;

    public void Counter()
    {
        var state = animancer.States.Current;
        if (state != null)
        {
            onCounter?.Invoke();
            state.IsPlaying = false;
            state.Speed = 0;
        }
    }
    public bool CanAttack(Vector3 targetPosition)
    {
        
        if (!_canAttack) return false;
        return Vector3.Distance(targetPosition, transform.position) < distanceToAttack;
    }

    public void DoSimpleRandomAttack()
    {
        if (!_canAttack || combos.Length == 0) return;

        var randomCombo = combos[UnityEngine.Random.Range(0, combos.Length)];
        if (randomCombo == null || randomCombo.AttackSet == null || randomCombo.AttackSet.Length == 0)
            return;

        var chosenAttack = randomCombo.AttackSet[UnityEngine.Random.Range(0, randomCombo.AttackSet.Length)];

        StartCoroutine(PerformSingleAttackRoutine(chosenAttack));
    }

    public void DoRandomCombo()
    {
        if (!_canAttack || combos.Length == 0) return;

        if (_comboCoroutine != null)
            StopCoroutine(_comboCoroutine);

        _comboCoroutine = StartCoroutine(PerformFullComboRoutine());
    }

    public void InterruptAttack()
    {
        _counter.SetCounterIconActive(false);

        canCounter = false;

        if (_comboCoroutine != null)
        {
            StopCoroutine(_comboCoroutine);
            _comboCoroutine = null;
        }

        StopAllCoroutines();
        _canAttack = true;
    }


    private IEnumerator PerformSingleAttackRoutine(Attack attack)
    {
        _canAttack = false;
        OnAttackStart?.Invoke();

        _currentAttackState = animancer.Play(attack.animation, 0.05f);
        _currentAttackState.Events(this).SetCallback(HitEventName, CheckHit);
        _currentAttackState.Events(this).SetCallback(SwooshEventName, () => OnAttackSwoosh?.Invoke());
        _currentAttackState.Events(this).OnEnd = OnEndAttack;

        while (_currentAttackState.IsPlaying)
        {
            yield return null;
        }

        _canAttack = true;
    }

    private IEnumerator PerformFullComboRoutine()
    {
        _canAttack = false;

        var randomCombo = combos[UnityEngine.Random.Range(0, combos.Length)];
        if (randomCombo == null || randomCombo.AttackSet == null || randomCombo.AttackSet.Length == 0)
            yield break;

        for (int i = 0; i < randomCombo.AttackSet.Length; i++)
        {
            OnAttackStart?.Invoke();
            var attack = randomCombo.AttackSet[i];

            _currentAttackState = animancer.Play(attack.animation, 0.05f);
            _currentAttackState.Events(this).SetCallback(HitEventName, CheckHit);
            _currentAttackState.Events(this).SetCallback(SwooshEventName, () => OnAttackSwoosh?.Invoke());
            _currentAttackState.Events(this).OnEnd = OnEndAttack;

            while (_currentAttackState.IsPlaying)
            {
                yield return null;
            }

            if (_comboCoroutine == null)
                yield break;
        }

        OnComboFinished?.Invoke();

        _canAttack = true;
        _comboCoroutine = null;
    }

 

    private void CheckHit()
    {
        _counter.SetCounterIconActive(false);

        canCounter = false;
        var target = GetTarget(attackRadius);
        if (target != null)
        {
            var hitContext = new HitContext();
            target.Hit(hitContext);

            target.Transform.rotation = Quaternion.LookRotation(transform.position - target.Transform.position);

            var hitDealtEvent = new AttackInfo
            {
                attackAnimationState = _currentAttackState
            };
            OnHitDealtEvent?.Invoke(hitDealtEvent);
        }
    }


    private void OnEndAttack()
    {
        OnAttackEndEvent?.Invoke();
    }

  
    private IHitManager GetTarget(float radius)
    {
        List<IHitManager> enemiesInRange = TargetsUtils.GetObjectsInRadius(transform.position, radius);

        // Filtrar enemigos válidos
        enemiesInRange = enemiesInRange.FindAll(enemy =>
            enemy.Transform.CompareTag("CharacterCore") &&
            enemy.Transform != transform &&
            enemy.CanBeHit());

        if (enemiesInRange.Count == 0)
            return null;

        return TargetsUtils.GetClosestObjectByDistance(enemiesInRange, transform.position);
    }
}
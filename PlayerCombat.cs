using System;
using UnityEngine;
using System.Collections.Generic;
using Animancer;
using DefaultNamespace;
using DG.Tweening;
using UnityEngine.Serialization;
using System.Collections;
using Unity.VisualScripting;

public class PlayerCombat : MonoBehaviour,IAttackStatusNotifier
{
    [SerializeField] private SoundCollection leapAudio;
    [Header("Combo Settings")]
[SerializeField] private Combo[] combos;

[Header("Animation Settings")]
[SerializeField] private AnimancerComponent animancer;

[Header("Hit Event Settings")]
[SerializeField]
private StringAsset HitEventName;
[SerializeField] private StringAsset SwooshEventName;

[Header("Cooldown Settings")]
[SerializeField] private float canAttackCooldown;

[Header("Leap Settings")]
[SerializeField] private float maxLeapDistance;
[SerializeField] private float maxLeapAngle;
[SerializeField] private float leapDurationCoefficient;
[SerializeField] private float leapDestinationOffset;

[Header("Attack Settings")]
[SerializeField] private float attackRadius;
[SerializeField] private float attackAngle;
[Header("Combo Timings")]
[SerializeField] private float keepComboCooldown;
[Header("Time Scale Settings")]
[SerializeField] private float timeScaleChangeRate = 1f;  // Tasa de cambio del timeScale
[SerializeField] private float targetTimeScale = 0.5f;    // Valor objetivo del 

    private ComboManager _comboManager;
    public bool CanAttack { get; private set; } = true;

    public event Action OnAttackEndEvent;
    private IHitManager GetCounterableTargetInRange()
    {
        // Obtener todos los objetivos en un radio maxLeapDistance alrededor del jugador
        List<IHitManager> enemiesInRange = TargetsUtils.GetObjectsInRadius(transform.position, maxLeapDistance);

        // Filtrar para que sean objetivos con Tag "Character", 
        // que no sean el propio jugador y que puedan ser golpeados
        enemiesInRange = enemiesInRange.FindAll(enemy =>
            enemy.Transform.CompareTag("Character") &&
            enemy.Transform != transform &&
            enemy.CanBeHit());

        // Revisar si no hay enemigos válidos
        if (enemiesInRange.Count == 0) 
            return null;

        // Buscar el primer enemigo que tenga "EnemyCombat" y cuyo "canCounter" sea true
        foreach (var enemy in enemiesInRange)
        {
            EnemyCombat enemyCombat = enemy.Transform.GetComponent<EnemyCombat>();
            if (enemyCombat != null && enemyCombat.canCounter)
            {
                // Si se encuentra, retornamos el enemigo
                return enemy;
            }
        }

        // Si ningún objetivo cumple, retorna null
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
        // Primero realizamos el leap hacia el objetivo.
        TryLeapTowards(target);

        // Luego, calculamos la dirección y ejecutamos el ataque.
        Vector3 directionToTarget = target.Transform.position - transform.position;
        DoAttack(directionToTarget);
    }



    private void Awake()
    {
        _comboManager = new ComboManager(keepComboCooldown, combos);
    }
    private void OnEnable()
    {
        // Suscribimos la función al evento del Swoosh
        OnAttackSwoosh += CheckEnemyHealthAndAdjustTime;
    }

    private void OnDisable()
    {
        // Para evitar problemas, removemos la suscripción al deshabilitar el objeto
        OnAttackSwoosh -= CheckEnemyHealthAndAdjustTime;
    }
    
    // Esta es la función que se llamará en el momento de "Swoosh"
    private void CheckEnemyHealthAndAdjustTime()
    {
        // Obtenemos el enemigo actual (si existe)
        var target = GetTarget(transform.forward, attackRadius, attackAngle);
        if (target == null) return;

        // Revisamos si tiene el componente de salud
        var health = target.Transform.GetComponent<Health>(); 
        if (health == null) return;

        // Si la variable _currentHits es 1, entonces realizamos la animación del cambio de tiempo
        if (health._currentHits == 1)
        {
            // Cancelamos cualquier Tween previo que cambie timeScale
            DOTween.Kill("TimeScaleTween");

            // Tween para cambiar Time.timeScale de forma suave y con Ease.OutQuad
            DOTween.To(
                    () => Time.timeScale,
                    value => Time.timeScale = value,
                    targetTimeScale, 
                    timeScaleChangeRate
                )
                .SetId("TimeScaleTween")
                .SetEase(Ease.OutQuad);
        }
    }


    public void TryLeapAndAttack(Vector3 direction)
    {
        IHitManager target = GetTarget(direction, maxLeapDistance, maxLeapAngle);

        if (target != null)
        {
            TryLeapTowards(target);
        }

        DoAttack(direction);
    }

    private IHitManager GetTarget(Vector3 referenceDirection, float radius, float viewAngle)
    {
        List<IHitManager> enemiesInRange = TargetsUtils.GetObjectsInRadius(
            transform.position,
            radius
        );

        enemiesInRange = enemiesInRange.FindAll(enemy =>
                enemy.Transform.CompareTag("Character") &&
                enemy.Transform != transform && enemy.CanBeHit() == true);

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
        currentAttackState.Events(this).SetCallback(SwooshEventName, (() => OnAttackSwoosh?.Invoke()));
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
            
            var hitDealtEvent = new AttackInfo();
            hitDealtEvent.attackAnimationState = currentAttackState;
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
    public event Action OnAttackStart;
    public event Action OnAttackSwoosh;
    public event Action<AttackInfo> OnHitDealtEvent;
}


public struct AttackInfo
{
    public AnimancerState attackAnimationState;
}

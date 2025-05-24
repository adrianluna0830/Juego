using Core.StateMachine;
using UnityEngine;

public class EnemyAttack : BaseState
{
    private FiniteStateMachine _baseStateMachine;
    private EnemyCombat _enemyCombat;
    private IHitStatus _hitStatus;

    public EnemyAttack(
        FiniteStateMachine baseStateMachine,
        IHitStatus hitStatus,
        EnemyCombat enemyCombat)
    {
        _baseStateMachine = baseStateMachine;
        _hitStatus = hitStatus;
        _enemyCombat = enemyCombat;
    }

    public override void OnEnter()
    {
        
        // Nos suscribimos al evento de iniciar un hit
        _hitStatus.OnHitStartEvent += HitStatusOnOnHitStartEvent;

        // Además, nos suscribimos al evento que se dispara
        // al terminar la animación de ataque
        _enemyCombat.OnAttackEndEvent += OnAttackAnimationEnded;

        // Iniciamos el ataque simple aleatorio
        _enemyCombat.DoSimpleRandomAttack();

    }

    private void HitStatusOnOnHitStartEvent()
    {
        // Si el enemigo recibe un golpe en medio de su ataque, 
        // cambiamos a un estado de “hit”
        _baseStateMachine.SetCurrentState<EnemytHit>();
    }

    private void OnAttackAnimationEnded()
    {
        // Cuando termine la animación de ataque, regresamos al estado de moverse
        _baseStateMachine.SetCurrentState<EnemyMoveState>();
    }

    public override void OnExit()
    {
        _enemyCombat._canAttack = true;
        _enemyCombat.canCounter = false;
        _enemyCombat.attacking = false;
        // Eliminamos las suscripciones para evitar llamadas múltiples
        _hitStatus.OnHitStartEvent -= HitStatusOnOnHitStartEvent;
        _enemyCombat.OnAttackEndEvent -= OnAttackAnimationEnded;
    }
}
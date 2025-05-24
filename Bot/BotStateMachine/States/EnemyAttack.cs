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
        
        _hitStatus.OnHitStartEvent += HitStatusOnOnHitStartEvent;

        _enemyCombat.OnAttackEndEvent += OnAttackAnimationEnded;

        _enemyCombat.DoSimpleRandomAttack();

    }

    private void HitStatusOnOnHitStartEvent()
    {
        _baseStateMachine.SetCurrentState<EnemytHit>();
    }

    private void OnAttackAnimationEnded()
    {
        _baseStateMachine.SetCurrentState<EnemyMoveState>();
    }

    public override void OnExit()
    {
        _enemyCombat._canAttack = true;
        _enemyCombat.canCounter = false;
        _enemyCombat.attacking = false;
        _hitStatus.OnHitStartEvent -= HitStatusOnOnHitStartEvent;
        _enemyCombat.OnAttackEndEvent -= OnAttackAnimationEnded;
    }
}
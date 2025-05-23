using Core.StateMachine;
using UnityEngine;

public class EnemytHit : BaseState
{
    private FiniteStateMachine _baseStateMachine;
    private IHitStatus _hitStatus;
    private Health _health;
    private EnemyCombat _enemyCombat;
    public EnemytHit(FiniteStateMachine baseStateMachine, IHitStatus hitStatus, Health health, EnemyCombat enemyCombat)
    {
        _baseStateMachine = baseStateMachine;
        _hitStatus = hitStatus;
        _health = health;
        _enemyCombat = enemyCombat;
    }

    public override void OnEnter()
    {
        _enemyCombat.InterruptAttack();
        _hitStatus.OnHitStartEvent += HitStatusOnOnHitStartEvent;
    }

    private void HitStatusOnOnHitStartEvent()
    {
        _baseStateMachine.SetCurrentState<EnemytHit>();

    }


    public override void OnUpdate()
    {
        if (_health.IsDead)
        {
            _baseStateMachine.SetCurrentState<EnemyDeathState>();
        }
        if (_hitStatus.IsHitting == false)
        {
            _baseStateMachine.SetCurrentState<EnemyMoveState>();

        }
        

    }
    
    public override void OnExit()
    {
        _hitStatus.OnHitStartEvent -= HitStatusOnOnHitStartEvent;

    }
}
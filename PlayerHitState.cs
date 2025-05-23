using Core.StateMachine;
using UnityEngine;

public class PlayerHitState : BaseState
{
    private FiniteStateMachine _baseStateMachine;
    private IHitStatus _hitStatus;
    private Health _health;
    private PlayerCombat playerCombat;
    public PlayerHitState(FiniteStateMachine baseStateMachine, IHitStatus hitStatus, Health health, PlayerCombat _playerCombat)
    {
        _baseStateMachine = baseStateMachine;
        _hitStatus = hitStatus;
        _health = health;
        playerCombat = _playerCombat;
    }
   
    public override void OnEnter()
    {
        playerCombat.InterruptAttack();
        _hitStatus.OnHitStartEvent += HitStatusOnOnHitStartEvent;
    }

    private void HitStatusOnOnHitStartEvent()
    {
        _baseStateMachine.SetCurrentState<PlayerHitState>();

    }
   

    public override void OnUpdate()
    {
        if (_health.IsDead)
        {
            _baseStateMachine.SetCurrentState<PlayerDeathState>();
        }

        Debug.Log("hitasdasd");
        if (_hitStatus.IsHitting == false)
        {
            Debug.Log("ya");
            _baseStateMachine.SetCurrentState<PlayerMoveState>();

        }
        
      
      

    }
    
    public override void OnExit()
    {
        _hitStatus.OnHitStartEvent -= HitStatusOnOnHitStartEvent;

    }
}
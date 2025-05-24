using Core.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCounterState : BaseState
{
    private FiniteStateMachine _finiteStateMachine;
    private PlayerInput _playerInput;
    private PlayerCombat _playerCombat;
    private IHitStatus _hitStatus;

    public PlayerCounterState(FiniteStateMachine finiteStateMachine, PlayerCombat playerCombat, PlayerInput playerInput, IHitStatus hitStatus)
    {
        _finiteStateMachine = finiteStateMachine;
        _playerCombat = playerCombat;
        _playerInput = playerInput;
        _hitStatus = hitStatus;
    }

    public override void OnEnter()
    {
        _hitStatus.OnHitStartEvent += HitStatusOnOnHitStartEvent;
        
        _playerCombat.DoCounter();

        _playerInput.Combat.BasicAttack.performed += Attack;
        _playerCombat.OnAttackEndEvent += PlayerCombatOnOnAttackEndEvent;

    }

    private void HitStatusOnOnHitStartEvent()
    {
        _finiteStateMachine.SetCurrentState<PlayerHitState>();
    }

    private void PlayerCombatOnOnAttackEndEvent()
    {
        _finiteStateMachine.SetCurrentState<PlayerMoveState>();
    }

    public override void OnUpdate()
    {
    }
    public void Attack(InputAction.CallbackContext context)
    {
      
        if (_playerCombat.CanAttack)
        {
            _finiteStateMachine.SetCurrentState<PlayerAttackState>();
        }
    }
   
    public override void OnExit()
    {      _hitStatus.OnHitStartEvent -= HitStatusOnOnHitStartEvent;

        _playerCombat.OnAttackEndEvent -= PlayerCombatOnOnAttackEndEvent;

        _playerInput.Combat.BasicAttack.performed -= Attack;
    }

}
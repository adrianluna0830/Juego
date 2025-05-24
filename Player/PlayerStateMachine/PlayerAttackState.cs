using Core.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackState : BaseState
{
    private FiniteStateMachine _finiteStateMachine;
    private PlayerInput _playerInput;
    private PlayerCombat _playerCombat;
    private PlayerMove _playerMove;
    private IHitStatus _hitStatus;

    public PlayerAttackState(FiniteStateMachine finiteStateMachine, PlayerCombat playerCombat, PlayerMove playerMove, PlayerInput playerInput, IHitStatus hitStatus)
    {
        _finiteStateMachine = finiteStateMachine;
        _playerCombat = playerCombat;
        _playerMove = playerMove;
        _playerInput = playerInput;
        _hitStatus = hitStatus;
    }

    public override void OnEnter()
    {
      _hitStatus.OnHitStartEvent += HitStatusOnOnHitStartEvent;
        Vector2 input = _playerInput.Combat.Move.ReadValue<Vector2>();
        Vector3 relativeDir = _playerMove.GetHorizontalMoveDirectionRelativeToCamera(input);

        _playerCombat.TryLeapAndAttack(relativeDir);
      
        _playerInput.Combat.BasicAttack.performed += Attack;
        _playerCombat.OnAttackEndEvent += PlayerCombatOnOnAttackEndEvent;
        _playerInput.Combat.Counter.performed += CounterOnperformed;

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
    private void CounterOnperformed(InputAction.CallbackContext obj)
    {
        if (_playerCombat.CanDoCounter() && _playerCombat.CanAttack)
        {
            _finiteStateMachine.SetCurrentState<PlayerCounterState>();
        }
    }

    public override void OnExit()
    {      _hitStatus.OnHitStartEvent -= HitStatusOnOnHitStartEvent;
        _playerInput.Combat.Counter.performed -= CounterOnperformed;

        _playerCombat.OnAttackEndEvent -= PlayerCombatOnOnAttackEndEvent;

        _playerInput.Combat.BasicAttack.performed -= Attack;
    }
}
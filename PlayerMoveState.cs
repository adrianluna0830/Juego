using Animancer;
using Core.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveState : BaseState
{
    private FiniteStateMachine _finiteStateMachine;
    private PlayerMove _playerMove;
    private PlayerInput _playerInput;
    private CharacterAnimation _characterAnimation;
    private PlayerCombat _playerCombat;
    private IHitStatus _hitStatus;
    public PlayerMoveState(PlayerMove playerMove, PlayerInput playerInput, CharacterAnimation characterAnimation, PlayerCombat playerCombat, FiniteStateMachine finiteStateMachine, IHitStatus hitStatus)
    {
        _playerMove = playerMove;
        _playerInput = playerInput;
        _characterAnimation = characterAnimation;
        _playerCombat = playerCombat;
        _finiteStateMachine = finiteStateMachine;
        _hitStatus = hitStatus;
    }
   
    public MixerState<float> moveAnimation;

    public override void OnEnter()
    {
        _playerInput.Combat.BasicAttack.performed += Attack;
        _hitStatus.OnHitStartEvent += HitStatusOnOnHitStartEvent;
        _playerInput.Combat.Counter.performed += CounterOnperformed;
    }

    private void CounterOnperformed(InputAction.CallbackContext obj)
    {
        if (_playerCombat.CanDoCounter())
        {
            _finiteStateMachine.SetCurrentState<PlayerCounterState>();
        }
    }

    private void HitStatusOnOnHitStartEvent()
    {
        _finiteStateMachine.SetCurrentState<PlayerHitState>();
    }

    public override void OnUpdate()
    {
        Vector2 input = _playerInput.Combat.Move.ReadValue<Vector2>();
      
        _playerMove.MoveRelativeToCamera(input,true);

    }
    public void Attack(InputAction.CallbackContext context)
    {
      
        if (_playerCombat.CanAttack)
        {
            _finiteStateMachine.SetCurrentState<PlayerAttackState>();
        }
    }
    public override void OnExit()
    {
        _hitStatus.OnHitStartEvent -= HitStatusOnOnHitStartEvent;
        _playerInput.Combat.Counter.performed -= CounterOnperformed;

        _playerInput.Combat.BasicAttack.performed -= Attack;
    }
}

using System;
using Core.StateMachine;
using UnityEngine;


public class PlayerStateBrain : MonoBehaviour
{
   private FiniteStateMachine _finiteStateMachine;
   
   [SerializeField] private PlayerInputMapper _playerInput;
   [SerializeField] private PlayerMove _playerMove;
   [SerializeField] private CharacterAnimation _characterAnimation;
   [SerializeField] private PlayerCombat _playerCombat;
   [SerializeField] private Health health;
   [SerializeField] private HitReactStatus _hitReactStatus;

   private void Start()
   {
      _finiteStateMachine = new FiniteStateMachine();

      var moveState = new PlayerMoveState(_playerMove, _playerInput.playerInput, _characterAnimation, _playerCombat, _finiteStateMachine,_hitReactStatus);
      var attackState = new PlayerAttackState(_finiteStateMachine ,_playerCombat,_playerMove,_playerInput.playerInput,_hitReactStatus);
      var playerHitState = new PlayerHitState(_finiteStateMachine, _hitReactStatus, health, _playerCombat);
      var playerDeathState = new PlayerDeathState();
      var playerCounterState = new PlayerCounterState(_finiteStateMachine, _playerCombat, _playerInput.playerInput, _hitReactStatus);
      _finiteStateMachine.RegisterState(moveState);
      _finiteStateMachine.RegisterState(attackState);
      _finiteStateMachine.RegisterState(playerHitState);
      _finiteStateMachine.RegisterState(playerDeathState);
      _finiteStateMachine.RegisterState(playerCounterState);

      _finiteStateMachine.SetCurrentState(moveState);

   }
   


   private void Update()
   {
      _finiteStateMachine.Update();
   }
}
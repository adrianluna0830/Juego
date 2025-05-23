using Core;
using UnityEngine;

namespace Gameplay.Player.States.Movement
{
    public class PlayerMovementState : BaseState
    {
        private readonly CharacterMovement  _playerMovement;
        private readonly PlayerInput playerInput;
        private readonly PlayerRotator _playerRotator;
        private readonly CharacterAnimation _characterAnimation;
        public PlayerMovementState(CharacterMovement playerMovement, PlayerInput playerInput, PlayerRotator playerRotator, CharacterAnimation characterAnimation)
        {
            _playerMovement = playerMovement;
            this.playerInput = playerInput;
            _playerRotator = playerRotator;
            _characterAnimation = characterAnimation;
        }

        public override void OnEnter()
        {
            // _characterAnimation.PlayAnimation(AnimationType.MOVE);
        }

        // public override void OnUpdate()
        // {
        //     Vector2 moveInput = playerInput.Combat.Move.ReadValue<Vector2>();
        //     _playerMovement.MoveRelative(moveInput);
        //     _playerRotator.RotateRespectToCameraSmooth(moveInput);
        //     _characterAnimation.UpdateAnimation(AnimationType.MOVE,_playerMovement.GetClampedSpeed());
        // }
    }
}
using Core;
using UnityEngine;

namespace Gameplay.Player.States.Camera
{
    public class CameraFreeState : BaseState
    {
        private readonly PlayerCameraRotation _playerCameraRotation;
        private readonly PlayerInput _playerInput;

        public CameraFreeState(PlayerCameraRotation playerCameraRotation, PlayerInput playerInput)
        {
            _playerCameraRotation = playerCameraRotation;
            _playerInput = playerInput;
        }
        
        public override void OnLateUpdate()
        {
            Vector2 aimVector = _playerInput.Combat.Aim.ReadValue<Vector2>();
            _playerCameraRotation.RotateWithInput(aimVector);
        }
    }
}
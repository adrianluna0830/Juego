using System;
using Core.StateMachine;
using Gameplay.Player.States.Camera;
using UnityEngine;

public class CameraStateController : MonoBehaviour
{
    private FiniteStateMachine _finiteStateMachine;
    
    [SerializeField] private PlayerCameraRotation _playerCameraRotation;
    [SerializeField] private PlayerInputMapper _playerInputMapper;
    private void Awake()
    {
        _finiteStateMachine = new FiniteStateMachine();

        var cameraFreeState = new CameraFreeState(_playerCameraRotation,_playerInputMapper.playerInput);
        
        _finiteStateMachine.RegisterState(cameraFreeState);
        
        _finiteStateMachine.SetCurrentState(cameraFreeState);
    }

    private void LateUpdate()
    {
        _finiteStateMachine.LateUpdate();
    }
}

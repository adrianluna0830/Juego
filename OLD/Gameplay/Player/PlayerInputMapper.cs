using System;
using Gameplay.Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputMapper  : MonoBehaviour
{
    public PlayerInput playerInput { get; private set; }


    public void Awake()
    {
        playerInput = new PlayerInput();
        
        ChangeInputMap(InputMap.GAMEPLAY);
    }

  

    public void ChangeInputMap(InputMap inputMap)
    {
        playerInput.Disable();
        
        switch (inputMap)
        {
            case InputMap.GAMEPLAY:
                playerInput.Combat.Enable();
                break;

            case InputMap.UI:
                break;

            default:
                Debug.LogWarning("InputMap no reconocido");
                break;
        }
        
    }
}

public enum InputMap
{
    GAMEPLAY,
    UI
}


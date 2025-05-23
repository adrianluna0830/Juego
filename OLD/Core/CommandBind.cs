using System;
using Core;
using UnityEngine.InputSystem;

[Serializable]
public class CommandBind
{
    public InputActionReference ActionReference;
    public InputCommand Command;
    public bool OnlyReadValue = false;
}
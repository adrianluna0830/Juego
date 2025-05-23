using System;
using System.Collections.Generic;
using Core.StateMachine;
using UnityEngine;

public interface IState
{
    public void OnEnter();
    public void OnExit();
    public void OnUpdate();
    public void OnFixedUpdate();
    public void OnLateUpdate();
    

}

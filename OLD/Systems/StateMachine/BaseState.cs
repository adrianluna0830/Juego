
using System;
using System.Collections.Generic;
using Core.StateMachine;
using UnityEngine;

public abstract class BaseState : IState
{
    

        public virtual void OnEnter()
        {
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnUpdate()
        {
        }

        public virtual void OnFixedUpdate()
        {
        }

        public virtual void OnLateUpdate()
        {
        }
    }

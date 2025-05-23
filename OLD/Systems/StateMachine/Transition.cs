using System;

namespace DefaultNamespace.Systems.StateMachines.RunTime
{
    public class Transition
    {
        public IState TargetState { get; } // Estado al que lleva la transición

        public Func<bool> Condition { get; } // Condición necesaria para activar la transición


        public Transition(IState targetState, Func<bool> condition)
        {
            TargetState = targetState;
            Condition = condition;
        }
    }
}
using System;

namespace DefaultNamespace.Systems.StateMachines.RunTime
{
    public class Transition
    {
        public IState TargetState { get; } // Estado al que lleva la transicion

        public Func<bool> Condition { get; } // Condicion necesaria para activar la transicion


        public Transition(IState targetState, Func<bool> condition)
        {
            TargetState = targetState;
            Condition = condition;
        }
    }
}
using System.Collections.Generic;

namespace DefaultNamespace.Systems.StateMachines.RunTime
{
    public class StateNode
    {
        public IState State { get; } // Representa el estado asociado al nodo
        
        
        // Transiciones automáticas del nodo
        public List<Transition> Auto { get; } = new System.Collections.Generic.List<Transition>(); 
        
        // Transiciones basadas en eventos para este nodo

        public List<Transition> Event { get; } = new System.Collections.Generic.List<Transition>();

        public StateNode(IState state)
        {
            State = state;
        }
    }
}
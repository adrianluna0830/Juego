using System;

namespace DefaultNamespace.Systems.StateMachines.RunTime
{
    public class StateEventSystem
    {
        // Evento notificado cuando el estado actual cambia

        public event Action<IState> StateChanged;
        
        // Evento notificado durante una transición de un estado a otro
        public event Action<IState, IState> StateTransition; // Estado de origen, estado de destino

        // Invoca el evento que informa que el estado ha cambiado
        public void NotifyStateChanged(IState newState)
        {
            StateChanged?.Invoke(newState);
        }
        
        
        // Invoca el evento de transición entre estados indicando el origen y destino

        public void NotifyStateTransition(IState fromState, IState toState)
        {
            StateTransition?.Invoke(fromState, toState);
        }
    }
}
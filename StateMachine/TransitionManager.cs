using System;

namespace DefaultNamespace.Systems.StateMachines.RunTime
{
    /// <summary>
    /// Maneja las transiciones entre estados dentro de una maquina de estados.
    /// Proporciona funcionalidad para agregar transiciones automaticas y basadas en eventos entre estados,
    /// asi como evaluar transiciones basadas en condiciones o eventos especificos.
    /// </summary>
    public class TransitionManager
    {
        private readonly StateRegistry _registry;
        
        public TransitionManager(StateRegistry registry)
        {
            _registry = registry;
        }

        /// <summary>
        /// Agrega una transicion automatica entre dos estados con una condicion especificada.
        /// La transicion se evaluara durante las comprobaciones y cuando la condicion se cumpla,
        /// la maquina de estados transicionara del estado "from" al estado "to".
        /// </summary>
        /// <param name="from">Estado origen donde inicia la transicion automatica.</param>
        /// <param name="to">Estado destino al que se transiciona si la condicion se cumple.</param>
        /// <param name="condition">Funcion condicion que se evalua para determinar si se debe realizar la transicion.</param>
        public void AddAutoTransition(IState from, IState to, Func<bool> condition)
        {
            _registry.CheckAndRegisterState(from);
            _registry.CheckAndRegisterState(to);

            var fromNode = _registry.GetNode(from);
            fromNode.Auto.Add(new Transition(to, condition));
        }

        /// <summary>
        /// Agrega una transicion basada en eventos entre dos estados con una condicion especificada.
        /// La transicion se dispara cuando la condicion es evaluada durante un evento.
        /// </summary>
        /// <param name="from">Estado origen de la transicion.</param>
        /// <param name="to">Estado destino de la transicion.</param>
        /// <param name="condition">Funcion que determina si la transicion debe ocurrir al evaluarse.</param>
        public void AddEventTransition(IState from, IState to, Func<bool> condition)
        {
            _registry.CheckAndRegisterState(from);
            _registry.CheckAndRegisterState(to);

            var fromNode = _registry.GetNode(from);
            fromNode.Event.Add(new Transition(to, condition));
        }

        /// <summary>
        /// Evalua las condiciones de las transiciones automaticas para el estado actual dado.
        /// Si alguna condicion se cumple, retorna el estado destino de la primera transicion valida.
        /// Si no hay transiciones validas, retorna null.
        /// </summary>
        /// <param name="currentState">Estado actual para evaluar las transiciones automaticas.</param>
        /// <returns>Estado destino de la transicion automatica valida, o null si no existe.</returns>
        public IState EvaluateAutoTransitions(IState currentState)
        {
            if (!_registry.IsRegistered(currentState))
                return null;

            var node = _registry.GetNode(currentState);

            foreach (var transition in node.Auto) // Itera sobre transiciones automaticas del nodo
                if (transition.Condition()) // Verifica si la condicion se cumple
                    return transition.TargetState; // Retorna estado destino si condicion es verdadera

            return null; // Retorna null si ninguna condicion se cumple
        }

        /// <summary>
        /// Evalua transiciones basadas en eventos para un estado actual dado e intenta determinar
        /// si es posible transicionar al estado destino especificado basado en las condiciones definidas.
        /// </summary>
        /// <param name="currentState">Estado actual desde el cual se evaluan transiciones basadas en eventos. Debe estar registrado.</param>
        /// <param name="targetState">Estado potencial destino para la transicion. La transicion ocurre si la condicion se cumple.</param>
        /// <returns>Retorna el estado destino si la condicion se cumple; de lo contrario, null si no hay condicion cumplida o estado no registrado.</returns>
        public IState EvaluateEventTransition(IState currentState, IState targetState)
        {
            if (!_registry.IsRegistered(currentState))
                return null;

            var node = _registry.GetNode(currentState);

            foreach (var transition in node.Event) // Itera sobre transiciones basadas en eventos del nodo
                if (transition.TargetState == targetState &&
                    transition.Condition()) // Verifica estado destino y condicion
                    return targetState; // Retorna estado destino si condicion se cumple

            return null; // Retorna null si ninguna condicion se cumple
        }
    }
}

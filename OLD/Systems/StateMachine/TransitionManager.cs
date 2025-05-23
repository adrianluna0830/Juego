using System;

namespace DefaultNamespace.Systems.StateMachines.RunTime
{
    /// <summary>
    /// Manages transitions between states within a state machine.
    /// Provides functionality for adding automatic and event-based transitions between states,
    /// as well as evaluating transitions based on specified conditions or events.
    /// </summary>
    public class TransitionManager
    {
   
        private readonly StateRegistry _registry;
        
        public TransitionManager(StateRegistry registry)
        {
            _registry = registry;
        }


        /// Adds an automatic transition between two states with a specified condition.
        /// The transition will be evaluated during transition checks and, when the condition is met,
        /// the state machine will transition from the "from" state to the "to" state.
        /// <param name="from">The source state where the automatic transition starts.</param>
        /// <param name="to">The target state to transition to if the condition is satisfied.</param>
        /// <param name="condition">The condition function to be evaluated to determine if the transition should occur.</param>
        public void AddAutoTransition(IState from, IState to, Func<bool> condition)
        {
            _registry.CheckAndRegisterState(from);
            _registry.CheckAndRegisterState(to);

            var fromNode = _registry.GetNode(from);
            fromNode.Auto.Add(new Transition(to, condition));
        }


        /// Adds an event-based transition between two states with a specified condition.
        /// The transition is triggered when the condition is evaluated during an event.
        /// <param name="from">The state from which the transition originates.</param>
        /// <param name="to">The state to which the transition leads.</param>
        /// <param name="condition">A function that determines if the transition should occur when evaluated.</param>
        public void AddEventTransition(IState from, IState to, Func<bool> condition)
        {
            _registry.CheckAndRegisterState(from);
            _registry.CheckAndRegisterState(to);

            var fromNode = _registry.GetNode(from);
            fromNode.Event.Add(new Transition(to, condition));
        }
        



        /// Evaluates the conditions of automatic transitions for the given current state.
        /// If any condition is met, returns the target state of the first valid transition.
        /// If no valid transition is found, returns null.
        /// <param name="currentState">The current state to evaluate automatic transitions from.</param>
        /// <returns>The target state of the valid automatic transition, or null if no valid transition exists.</returns>
        public IState EvaluateAutoTransitions(IState currentState)
        {
            if (!_registry.IsRegistered(currentState))
                return null;

            var node = _registry.GetNode(currentState);

            foreach (var transition in node.Auto) // Itera sobre las transiciones automáticas del nodo
                if (transition.Condition()) // Verifica si la condición de la transición se cumple
                    return transition.TargetState; // Retorna el estado objetivo si la condición es verdadera

            return null; // Retorna null si no se cumple ninguna condición de transición
        }


        /// Evaluates event-based transitions for a given current state and attempts to determine
        /// if transitioning to the specified target state is possible based on the defined conditions.
        /// <param name="currentState">The current state from which the event-based transitions are evaluated. Must be a registered state.</param>
        /// <param name="targetState">The potential target state for the transition. The transition occurs if the associated condition is met.</param>
        /// <returns>Returns the target state if the evaluation condition is satisfied; otherwise, returns null if no condition is met or the state is not registered.</returns>
        public IState EvaluateEventTransition(IState currentState, IState targetState)
        {
            if (!_registry.IsRegistered(currentState))
                return null;

            var node = _registry.GetNode(currentState);

            foreach (var transition in node.Event) // Itera sobre las transiciones basadas en eventos del nodo
                if (transition.TargetState == targetState &&
                    transition
                        .Condition()) // Verifica si el estado objetivo coincide y si se cumple la condición de la transición
                    return targetState; // Retorna el estado objetivo si se cumple la condición

            return null; // Retorna null si no se cumple ninguna condición de transición
        }
    }
}
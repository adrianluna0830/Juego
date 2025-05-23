using System;
using DefaultNamespace.Systems.StateMachines.RunTime;
using UnityEngine;

namespace Core.StateMachine
{
    public class FiniteStateMachine
    {
        private readonly StateRegistry _stateRegistry;
        private readonly TransitionManager _transitionManager;
        private readonly StateEventSystem _eventSystem;

        private StateNode _currentNode;

        private const string NullStateUpdateError = "Attempted to Update a null state";

        public IState CurrentState => _currentNode?.State;
        public bool HasCurrentState => _currentNode != null;

        public FiniteStateMachine()
        {
            _stateRegistry = new StateRegistry();
            _transitionManager = new TransitionManager(_stateRegistry);
            _eventSystem = new StateEventSystem();
        }

        #region Configuration Methods

        /// <summary>
        /// Registers a new state into the finite state machine.
        /// </summary>
        /// <param name="state">The state to be registered. Must implement the IState interface.</param>
        /// <returns>The current instance of FiniteStateMachine to allow method chaining.</returns>
        public FiniteStateMachine RegisterState(IState state)
        {
            _stateRegistry.RegisterState(state);
            return this;
        }

        /// Adds an automatic transition between two states based on a specified condition.
        /// The transition will be evaluated continuously, and when the condition is satisfied,
        /// the state machine will switch from the "from" state to the "to" state.
        /// <param name="from">The source state from which the automatic transition begins.</param>
        /// <param name="to">The destination state to transition to if the condition is satisfied.</param>
        /// <param name="condition">A function that evaluates to a boolean; the transition occurs when it returns true.</param>
        /// <return>Returns the instance of the FiniteStateMachine to allow method chaining.</return>
        public FiniteStateMachine AddAutomaticTransition(IState from, IState to, Func<bool> condition)
        {
            _transitionManager.AddAutoTransition(from, to, condition);
            return this;
        }

        /// Adds an event-based transition between two states with a specified condition.
        /// The transition is triggered when a specific event occurs, and the condition is met.
        /// <param name="from">The originating state for the transition.</param>
        /// <param name="to">The target state for the transition.</param>
        /// <param name="condition">A function that evaluates a condition and determines whether the transition should occur when the event is triggered.</param>
        /// <returns>The current instance of <see cref="FiniteStateMachine"/> to allow method chaining.</returns>
        public FiniteStateMachine AddEventTransition(IState from, IState to, Func<bool> condition)
        {
            _transitionManager.AddEventTransition(from, to, condition);
            return this;
        }

        #endregion

        #region State Operations

        /// <summary>
        /// Sets the current state of the state machine to the specified state.
        /// </summary>
        /// <param name="state">The state to set as the current state.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the specified state is not registered in the state registry.
        /// </exception>
        public void SetCurrentState(IState state)
        {
            if (!_stateRegistry.IsRegistered(state))
            {
                throw new InvalidOperationException($"State {state.GetType().Name} is not registered");
            }

            TransitionToState(state);
        }
        
        public void SetCurrentState<T>() where T : IState
        {
            // Busca el estado del tipo genérico entre los estados registrados
            IState state = null;
            foreach (var registeredState in _stateRegistry.GetStates())
            {
                if (registeredState is T)
                {
                    state = registeredState;
                    break;
                }
            }
    
            if (state != null)
            {
                SetCurrentState(state);
            }
            else
            {
                Debug.LogError($"Estado de tipo {typeof(T)} no encontrado. Asegúrate de registrarlo primero.");
            }
        }


        /// <summary>
        /// Temporarily sets the current state of the finite state machine without registering or preserving it.
        /// </summary>
        /// <param name="state">The state to be set as the temporary current state.</param>
        public void SetTemporaryState(IState state)
        {
            TransitionToState(state, isTemporary: true);
        }

        /// Transitions the state machine to the specified state, optionally marking it as temporary.
        /// <param name="state">The new state to transition to. Must be a registered state.</param>
        /// <param name="isTemporary">Specifies whether the transition is temporary. When true, the state node is not retrieved from the state registry.</param>
        /// <exception cref="InvalidOperationException">Thrown if the given state is not registered in the state machine.</exception>
        private void TransitionToState(IState state, bool isTemporary = false)
        {
            var oldState = CurrentState;
            HandleStateExitAndEnter(oldState, state);

            _currentNode = isTemporary ? new StateNode(state) : _stateRegistry.GetNode(state);
        }

        /// <summary>
        /// Clears the current state of the state machine, if any, and performs necessary cleanup operations.
        /// </summary>
        /// <remarks>
        /// This method transitions the state machine to a null state by exiting the current state, if one exists,
        /// and resetting the internal state tracking. It handles all necessary exit logic for the current state
        /// and notifies the event system of the state change. After calling this method, the state machine will
        /// no longer have an active state.
        /// </remarks>
        public void ClearState()
        {
            var oldState = CurrentState;
            HandleStateExitAndEnter(oldState, null);
            _currentNode = null;
        }

        /// <summary>
        /// Manages the exit and entry logic when transitioning between states in the finite state machine.
        /// </summary>
        /// <param name="oldState">The previous state that is being exited. Can be null if no prior state exists.</param>
        /// <param name="newState">The new state that is being entered. Can be null if the state is being cleared.</param>
        private void HandleStateExitAndEnter(IState oldState, IState newState)
        {
            oldState?.OnExit();
            newState?.OnEnter();
            _eventSystem.NotifyStateChanged(newState);
            _eventSystem.NotifyStateTransition(oldState, newState);
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Updates the current state of the Finite State Machine and evaluates automatic transitions.
        /// </summary>
        /// <remarks>
        /// This method executes the following steps:
        /// 1. Checks if there is a current state set. If none exists, logs an error and returns.
        /// 2. Evaluates automatic transitions using the TransitionManager. If a valid next state is retrieved,
        /// switches to that state.
        /// 3. Calls the OnUpdate method of the current state.
        /// </remarks>
        /// <exception cref="UnityEngine.Debug.LogError">Thrown if the current state is null.</exception>
        public void Update()
        {
            if (_currentNode == null)
            {
                Debug.LogError(NullStateUpdateError);
                return;
            }

            var nextState = _transitionManager.EvaluateAutoTransitions(CurrentState);
            if (nextState != null)
            {
                SetCurrentState(nextState);
                return;
            }

            CurrentState.OnUpdate();
        }

        /// Executes the FixedUpdate cycle on the current state of the finite state machine,
        /// if a current state is set. This method calls the `OnFixedUpdate` method
        /// on the current state's implementation, allowing it to perform logic
        /// that should occur during fixed updates, typically associated with physics-related updates.
        public void FixedUpdate()
        {
            CurrentState?.OnFixedUpdate();
        }

        /// Executes the LateUpdate phase of the finite state machine, which invokes the
        /// `OnLateUpdate` method of the currently active state, if a state is set.
        /// This method is typically called during the Unity LateUpdate lifecycle phase
        /// to ensure late frame processing for the current state.
        public void LateUpdate()
        {
            CurrentState?.OnLateUpdate();
        }

        #endregion
    }
}
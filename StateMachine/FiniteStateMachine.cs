using System;
using DefaultNamespace.Systems.StateMachines.RunTime;
using UnityEngine;

namespace Core.StateMachine
{
    /// <summary>
    /// Implementa una maquina de estados finita que maneja estados y transiciones.
    /// Permite registrar estados, agregar transiciones automaticas y basadas en eventos,
    /// y controlar el flujo del estado actual.
    /// </summary>
    public class FiniteStateMachine
    {
        private readonly StateRegistry _stateRegistry;
        private readonly TransitionManager _transitionManager;
        private readonly StateEventSystem _eventSystem;

        private StateNode _currentNode;

        private const string NullStateUpdateError = "Attempted to Update a null state";

        /// <summary>
        /// Obtiene el estado actual activo en la maquina.
        /// </summary>
        public IState CurrentState => _currentNode?.State;

        /// <summary>
        /// Indica si existe un estado actual activo.
        /// </summary>
        public bool HasCurrentState => _currentNode != null;

        /// <summary>
        /// Constructor que inicializa el registro de estados, manejador de transiciones y sistema de eventos.
        /// </summary>
        public FiniteStateMachine()
        {
            _stateRegistry = new StateRegistry();
            _transitionManager = new TransitionManager(_stateRegistry);
            _eventSystem = new StateEventSystem();
        }

        #region Metodos de configuracion

        /// <summary>
        /// Registra un estado en la maquina de estados.
        /// </summary>
        /// <param name="state">Estado a registrar que implementa IState.</param>
        /// <returns>La instancia actual de FiniteStateMachine para permitir encadenamiento.</returns>
        public FiniteStateMachine RegisterState(IState state)
        {
            _stateRegistry.RegisterState(state);
            return this;
        }

        /// <summary>
        /// Agrega una transicion automatica entre dos estados con una condicion especificada.
        /// </summary>
        /// <param name="from">Estado origen de la transicion.</param>
        /// <param name="to">Estado destino de la transicion.</param>
        /// <param name="condition">Funcion que determina cuando se activa la transicion.</param>
        /// <returns>La instancia actual de FiniteStateMachine para permitir encadenamiento.</returns>
        public FiniteStateMachine AddAutomaticTransition(IState from, IState to, Func<bool> condition)
        {
            _transitionManager.AddAutoTransition(from, to, condition);
            return this;
        }

        /// <summary>
        /// Agrega una transicion basada en eventos entre dos estados con una condicion especificada.
        /// </summary>
        /// <param name="from">Estado origen de la transicion.</param>
        /// <param name="to">Estado destino de la transicion.</param>
        /// <param name="condition">Funcion que determina si se activa la transicion.</param>
        /// <returns>La instancia actual de FiniteStateMachine para permitir encadenamiento.</returns>
        public FiniteStateMachine AddEventTransition(IState from, IState to, Func<bool> condition)
        {
            _transitionManager.AddEventTransition(from, to, condition);
            return this;
        }

        #endregion

        #region Operaciones con estados

        /// <summary>
        /// Establece el estado actual de la maquina a un estado especifico.
        /// </summary>
        /// <param name="state">Estado a establecer como actual.</param>
        /// <exception cref="InvalidOperationException">Se lanza si el estado no esta registrado.</exception>
        public void SetCurrentState(IState state)
        {
            if (!_stateRegistry.IsRegistered(state))
            {
                throw new InvalidOperationException($"State {state.GetType().Name} is not registered");
            }

            TransitionToState(state);
        }
        
        /// <summary>
        /// Establece el estado actual usando un tipo generico.
        /// Busca un estado registrado del tipo especificado y lo establece como actual.
        /// </summary>
        /// <typeparam name="T">Tipo del estado que implementa IState.</typeparam>
        public void SetCurrentState<T>() where T : IState
        {
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
                Debug.LogError($"Estado de tipo {typeof(T)} no encontrado. Asegurate de registrarlo primero.");
            }
        }

        /// <summary>
        /// Establece un estado temporal que no se registra ni se preserva.
        /// </summary>
        /// <param name="state">Estado temporal a establecer.</param>
        public void SetTemporaryState(IState state)
        {
            TransitionToState(state, isTemporary: true);
        }

        private void TransitionToState(IState state, bool isTemporary = false)
        {
            var oldState = CurrentState;
            HandleStateExitAndEnter(oldState, state);

            _currentNode = isTemporary ? new StateNode(state) : _stateRegistry.GetNode(state);
        }

        /// <summary>
        /// Limpia el estado actual y realiza la salida adecuada.
        /// </summary>
        public void ClearState()
        {
            var oldState = CurrentState;
            HandleStateExitAndEnter(oldState, null);
            _currentNode = null;
        }

        private void HandleStateExitAndEnter(IState oldState, IState newState)
        {
            oldState?.OnExit();
            newState?.OnEnter();
            _eventSystem.NotifyStateChanged(newState);
            _eventSystem.NotifyStateTransition(oldState, newState);
        }

        #endregion

        #region Metodos del ciclo de vida

        /// <summary>
        /// Actualiza la maquina de estados ejecutando actualizacion y evaluando transiciones automaticas.
        /// </summary>
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

        /// <summary>
        /// Ejecuta el ciclo FixedUpdate en el estado actual.
        /// </summary>
        public void FixedUpdate()
        {
            CurrentState?.OnFixedUpdate();
        }

        /// <summary>
        /// Ejecuta el ciclo LateUpdate en el estado actual.
        /// </summary>
        public void LateUpdate()
        {
            CurrentState?.OnLateUpdate();
        }

        #endregion
    }
}

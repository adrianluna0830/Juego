using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace.Systems.StateMachines.RunTime
{
    /// <summary>
    /// Maintains a registry of states and their corresponding StateNode objects.
    /// This class provides mechanisms for registering states, checking if a state is registered,
    /// and retrieving the associated StateNode for a specific state. It ensures that each state
    /// is uniquely registered within the registry.
    /// </summary>
    public class StateRegistry
    {
        /// <summary>
        /// A private field in the StateRegistry class that maps instances of <see cref="IState"/>
        /// to their corresponding <see cref="StateNode"/> representations.
        /// </summary>
        /// <remarks>
        /// This dictionary acts as a registry for managing state nodes, allowing the system
        /// to track and manipulate states and their associations. It provides quick lookup
        /// functionalities to retrieve nodes associated with specific states.
        /// </remarks>
        private readonly Dictionary<IState, StateNode> _stateNodes = new Dictionary<IState, StateNode>();
        private readonly Dictionary<Type, StateNode> _stateNodesType = new Dictionary<Type, StateNode>();
        
        public IState[] GetStates() => _stateNodes.Keys.ToArray();
        /// Retrieves the StateNode associated with the given IState.
        /// <param name="state">The state for which the corresponding StateNode is to be retrieved.</param>
        /// <returns>
        /// The StateNode associated with the provided state, or null if the state is not registered in the registry.
        /// </returns>
        public StateNode GetNode(IState state)
        {
            if (!_stateNodes.TryGetValue(state, out var node))
            {
                return null;
            }
            
            return node;
        }
        


        /// <summary>
        /// Registers a new state into the state registry. Ensures a unique state instance is registered.
        /// Throws an exception if the state is already registered.
        /// </summary>
        /// <param name="state">The state instance to be registered. Must implement the IState interface.</param>
        /// <exception cref="Exception">Thrown if the state is already registered in the system with a message indicating the state type.</exception>
        public void RegisterState(IState state)
        {
            if (_stateNodes.ContainsKey(state))
            {
                throw new Exception($"Estado {state.GetType().Name} ya está registrado");
            }

            _stateNodesType[state.GetType()] = new StateNode(state);

            
            _stateNodes[state] = new StateNode(state);
        }

        /// <summary>
        /// Determines whether a given state is registered in the state registry.
        /// </summary>
        /// <param name="state">The state to check for registration in the registry.</param>
        /// <returns>True if the state is registered; otherwise, false.</returns>
        public bool IsRegistered(IState state)
        {
            return _stateNodes.ContainsKey(state);
        }



        /// Checks if the specified state is already registered in the state registry.
        /// If the state is not registered, registers the state by creating and adding a new StateNode for it.
        /// <param name="state">The state to be checked and registered if not already present in the registry.</param>
        public void CheckAndRegisterState(IState state)
        {
            if (!IsRegistered(state))
            {
                RegisterState(state);
            }
        }
    }
}
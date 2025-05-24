using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace.Systems.StateMachines.RunTime
{
    /// <summary>
    /// Mantiene un registro de estados y sus objetos StateNode correspondientes.
    /// Esta clase provee mecanismos para registrar estados, verificar si un estado
    /// esta registrado y obtener el StateNode asociado a un estado especifico.
    /// Asegura que cada estado sea registrado unicamente en el registro.
    /// </summary>
    public class StateRegistry
    {
        /// <summary>
        /// Campo privado en la clase StateRegistry que mapea instancias de <see cref="IState"/>
        /// a sus representaciones correspondientes <see cref="StateNode"/>.
        /// </summary>
        /// <remarks>
        /// Este diccionario actua como un registro para manejar nodos de estado,
        /// permitiendo al sistema rastrear y manipular estados y sus asociaciones.
        /// Provee funcionalidades de busqueda rapida para obtener nodos asociados a estados especificos.
        /// </remarks>
        private readonly Dictionary<IState, StateNode> _stateNodes = new Dictionary<IState, StateNode>();
        private readonly Dictionary<Type, StateNode> _stateNodesType = new Dictionary<Type, StateNode>();
        
        public IState[] GetStates() => _stateNodes.Keys.ToArray();

        /// <summary>
        /// Obtiene el StateNode asociado al IState dado.
        /// </summary>
        /// <param name="state">El estado para el cual se desea obtener el StateNode correspondiente.</param>
        /// <returns>
        /// El StateNode asociado al estado proporcionado, o null si el estado no esta registrado en el registro.
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
        /// Registra un nuevo estado en el registro de estados. Asegura que se registre una instancia unica.
        /// Lanza una excepcion si el estado ya esta registrado.
        /// </summary>
        /// <param name="state">La instancia del estado a registrar. Debe implementar la interfaz IState.</param>
        /// <exception cref="Exception">Se lanza si el estado ya esta registrado en el sistema con un mensaje indicando el tipo de estado.</exception>
        public void RegisterState(IState state)
        {
            if (_stateNodes.ContainsKey(state))
            {
                throw new Exception($"Estado {state.GetType().Name} ya esta registrado");
            }

            _stateNodesType[state.GetType()] = new StateNode(state);
            _stateNodes[state] = new StateNode(state);
        }

        /// <summary>
        /// Determina si un estado dado esta registrado en el registro de estados.
        /// </summary>
        /// <param name="state">El estado para verificar su registro en el registro.</param>
        /// <returns>True si el estado esta registrado; de lo contrario, false.</returns>
        public bool IsRegistered(IState state)
        {
            return _stateNodes.ContainsKey(state);
        }

        /// <summary>
        /// Verifica si el estado especificado ya esta registrado en el registro.
        /// Si no esta registrado, registra el estado creando y agregando un nuevo StateNode para el.
        /// </summary>
        /// <param name="state">El estado a verificar y registrar si no esta presente en el registro.</param>
        public void CheckAndRegisterState(IState state)
        {
            if (!IsRegistered(state))
            {
                RegisterState(state);
            }
        }
    }
}

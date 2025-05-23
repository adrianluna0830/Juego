using Core.StateMachine;
using UnityEngine;
using Core.StateMachine;
using UnityEngine;

public class Retreat : AgentBaseState
{
    private float _retreatDistanceThreshold;
    private Vector3 _initialRetreatPosition;


    public Retreat(FiniteStateMachine stateMachine, PhysicsMovement movement, AgentBoid agent, CharacterAnimation characterAnimation) : base(stateMachine, movement, agent, characterAnimation)
    {
    }

    public override void OnEnter()
    {
        // Se guarda la posición inicial antes de retroceder
        _initialRetreatPosition = Agent.transform.position;
    
        // Se calcula la distancia meta de retreat, entre valores mínimos y máximos
        _retreatDistanceThreshold = Random.Range(Agent.AgentBoidData.RetreatMinDistance, Agent.AgentBoidData.RetreatMaxDistance);

        // _characterAnimation.PlayAnimation(AnimationType.MOVE);

    }
    
    public override void OnUpdate()
    {
        base.OnUpdate();

        Debug();
        DebugExtension.DebugPoint(Agent.transform.position + new Vector3(0,3,0),Color.blue);

        // Ajustamos la rotación según la posición del jugador
    
        // Aplicamos fuerza de retroceso
        
        Vector3 retreatForce =
            Agent.GetDirectionWithEscapeFromGroupForce(
                Agent.GetAppliedAgentAccelerationDirection(-GetDirectionToPlayer()));
        
        Movement.ApplySmoothForce(retreatForce);
    
        // Se verifica si el agente ha retrocedido la distancia suficiente
        if (Vector3.Distance(_initialRetreatPosition, Agent.transform.position) > _retreatDistanceThreshold)
        {
            StateMachine.SetCurrentState<BotStrafe>();
        }
        
    }
}

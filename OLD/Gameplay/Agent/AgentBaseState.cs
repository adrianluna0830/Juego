using Animancer;
using Core.StateMachine;
using UnityEngine;

public abstract class AgentBaseState : BaseState
{
    protected readonly FiniteStateMachine StateMachine;
    protected readonly PhysicsMovement Movement;
    protected readonly AgentBoid Agent;
    protected readonly CharacterAnimation _characterAnimation;

    protected AgentBaseState(FiniteStateMachine stateMachine, PhysicsMovement movement, AgentBoid agent,CharacterAnimation characterAnimation)
    {
        StateMachine = stateMachine;
        Movement = movement;
        Agent = agent;
        _characterAnimation = characterAnimation;
    }

   


    protected Vector3 GetDirectionToPlayer()
    {
        var direction = CombatContext.Instance.GetDirectionToPlayer(Movement.transform.position);
        direction.y = 0;
        return direction;
    }

    public override void OnUpdate()
    {
        PlayAnimationSmooth();
        UpdateAgentRotation();
    }

    private void PlayAnimationSmooth()
    {
        Vector3 velocityGlobal = Vector3.ClampMagnitude( Movement.GetVelocity(),1);
        Vector3 velocityLocalSmoothed = Movement.transform.InverseTransformDirection(velocityGlobal);
        velocityLocalSmoothed.y = 0;
        var state = (MixerState<Vector2>)_characterAnimation.GetState("DirectionalMove");
        var stateParameter = state.Parameter;
        var desiredParameter = new Vector2(velocityLocalSmoothed.x, velocityLocalSmoothed.z);

        state.Parameter = Vector2.MoveTowards(stateParameter, desiredParameter,Time.deltaTime * 0.5f);
    }

    protected void UpdateAgentRotation()
    {
 
        Movement.transform.forward = Vector3.Lerp(Movement.transform.forward, GetDirectionToPlayer(), 2.25f * Time.deltaTime);

    }

    protected void Debug()
    {
        DebugExtension.DebugCircle(Agent.transform.position + new Vector3(0,1,0),Color.blue,Agent.AgentBoidData.AgentDetectionDistance);
        DebugExtension.DebugCircle(Agent.transform.position + new Vector3(0,1,0),Color.green,Agent.AgentBoidData.CohesionDistance);
        DebugExtension.DebugCircle(Agent.transform.position + new Vector3(0,1,0),Color.red,Agent.AgentBoidData.SeparationDistance);
        
        var nearbyAgentsInSight = DetectionUtils.GetNearbyAgentsInView(
            Agent.AgentBoidData.DetectAgentsAngle,
            Agent.AgentBoidData.AgentDetectionDistance,
            Movement.transform);
        
        if (nearbyAgentsInSight.Count > 0)
        {
            DebugExtension.DebugPoint(Agent.transform.position + new Vector3(0,2,0),Color.black);
        }
    }
    

    protected bool IsPlayerInRange(float targetDistance)
    {
        return Vector3.Distance(Movement.transform.position, CombatContext.Instance.GetPlayerPosition()) <= targetDistance;
    }
}
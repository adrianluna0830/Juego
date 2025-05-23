using Core.StateMachine;
using UnityEngine;

public class BotSeek : AgentBaseState
{
    private const int MIN_ENEMIES_FOR_STRAFE = 2;
    private int _lateralDirection;


    public BotSeek(FiniteStateMachine stateMachine, PhysicsMovement movement, AgentBoid agent, CharacterAnimation characterAnimation) : base(stateMachine, movement, agent, characterAnimation)
    {
    }

    public override void OnEnter()
    {
        InitializeMovementParameters();
        SetRandomLateralDirection();
        var state = _characterAnimation.GetState("DirectionalMove");
        if (state == null)
        {
            state = _characterAnimation.PlayAnimation("DirectionalMove");
            float cycleOffset = UnityEngine.Random.Range(0, .5f);
            state.Time = cycleOffset;

        }
      
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        Debug();
        
        DebugExtension.DebugPoint(Agent.transform.position + new Vector3(0,3,0),Color.green);

        HandleMovementBehavior();

    }

    private void InitializeMovementParameters()
    {
        Movement.SetDrag(0.99f);
        Movement.SetGravity(0);
        Agent.CurrentDistanceToPlayer = Random.Range(Agent.AgentBoidData.MinDistanceToPlayer, Agent.AgentBoidData.MaxDistanceToPlayer);
    }

    private void SetRandomLateralDirection()
    {
        _lateralDirection = Random.value > 0.5f ? 1 : -1;
    }

    private void HandleMovementBehavior()
    {
        var nearbyAgentsInSight = DetectionUtils.GetNearbyAgentsInView(
            Agent.AgentBoidData.DetectAgentsAngle,
            Agent.AgentBoidData.AgentDetectionDistance,
            Movement.transform);

        if (IsPlayerInRange(Agent.CurrentDistanceToPlayer))
        {
            StateMachine.SetCurrentState<BotFlee>();
            return;
        }

        if (nearbyAgentsInSight.Count == 0)
        {
            Movement.ApplySmoothForce(Agent.GetDirectionWithEscapeFromGroupForce(Agent.GetAppliedAgentAccelerationDirection(GetDirectionToPlayer())));
        }
        else if (nearbyAgentsInSight.Count >= MIN_ENEMIES_FOR_STRAFE)
        {
            StateMachine.SetCurrentState<BotStrafe>();
        }
        else
        {
            ApplyEvasiveManeuvers();
        }
    }

    private void ApplyEvasiveManeuvers()
    {

        Vector3 strafeDirection = Movement.transform.right * (_lateralDirection * Agent.AgentBoidData.StrafeSpeed);
        Movement.ApplySmoothForce(Agent.GetDirectionWithEscapeFromGroupForce(strafeDirection));
    }
}
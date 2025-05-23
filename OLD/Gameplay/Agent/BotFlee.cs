using System;
using System.Collections.Generic;
using Core.StateMachine;
using UnityEngine;
using Random = UnityEngine.Random;
public class BotFlee : AgentBaseState
{
    private const int MIN_ENEMIES_FOR_RETREAT = 2;
    private const float TIME_INTERVAL_CHECK_SEEK = 3;

    private readonly Timer _stateTimer;
    private int _evasionDirection;


    public BotFlee(FiniteStateMachine stateMachine, PhysicsMovement movement, AgentBoid agent, CharacterAnimation characterAnimation) : base(stateMachine, movement, agent, characterAnimation)
    {
        _stateTimer = new Timer();
        ConfigureStateTimer();
    }

    public override void OnEnter()
    {
        InitializeFleeParameters();
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
        DebugExtension.DebugPoint(Agent.transform.position + new Vector3(0,3,0),Color.red);

        _stateTimer.Tick();
        HandleFleeBehavior();
        
        
    }
    
    private void ConfigureStateTimer()
    {
        _stateTimer.AddRecurringCallback(() =>
        {
            if (!IsPlayerInRange(Agent.CurrentDistanceToPlayer))
            {
                StateMachine.SetCurrentState<BotSeek>();
            }
        }, TIME_INTERVAL_CHECK_SEEK);
    }
    
    private void InitializeFleeParameters()
    {
        Movement.SetDrag(0.99f);
        Movement.SetGravity(0);
        _stateTimer.Restart();
        _evasionDirection = Random.value > 0.5f ? 1 : -1;
    }
    
    private void HandleFleeBehavior()
    {
        var nearbyAgents = DetectionUtils.GetNearbyAgentsInView(
            Agent.AgentBoidData.DetectAgentsAngle,
            Agent.AgentBoidData.AgentDetectionDistance,
            Movement.transform);
        
        
        //Se encuentra mas cerca del jugador de lo que deberia estar
        if (IsPlayerInRange(Agent.CurrentDistanceToPlayer))
        {
            HandleCloseRangeMovement(nearbyAgents);
        }
        else
        {
            ApplyEvasiveManeuvers();
        }
    }
    
    private void HandleCloseRangeMovement(List<Transform> nearbyAgents)
    {
        if (nearbyAgents.Count == 0)
        {
            Movement.ApplySmoothForce(Agent.GetDirectionWithEscapeFromGroupForce(-Agent.GetAppliedAgentAccelerationDirection( GetDirectionToPlayer())));
        }
        else
        {
            ApplyEvasiveManeuvers();
        }
    }
    
    private void ApplyEvasiveManeuvers()
    {

        Vector3 strafeDirection = Movement.transform.right * (_evasionDirection * Agent.AgentBoidData.StrafeSpeed);
        Movement.ApplySmoothForce(Agent.GetDirectionWithEscapeFromGroupForce(strafeDirection));
        
        
        
    }
}

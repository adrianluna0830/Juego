using Core.StateMachine;
using UnityEngine;

public class BotStrafe : AgentBaseState
{
    private float _strafeDuration;
    private float _elapsedTime;
    private int _strafeDirection;


    public BotStrafe(FiniteStateMachine stateMachine, PhysicsMovement movement, AgentBoid agent, CharacterAnimation characterAnimation) : base(stateMachine, movement, agent, characterAnimation)
    {
    }

    public override void OnEnter()
    {
        InitializeStrafeParameters();

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
        DebugExtension.DebugPoint(Agent.transform.position + new Vector3(0,3,0),Color.magenta);

        ApplyStrafeMovement();
        UpdateStrafeDuration();
    }
    
    private void InitializeStrafeParameters()
    {
        _elapsedTime = 0f;
        _strafeDirection = Random.value > 0.5f ? 1 : -1;
        _strafeDuration = Random.Range(Agent.AgentBoidData.MinStrafeTime, Agent.AgentBoidData.MaxStrafeTime);
    }
    
    private void ApplyStrafeMovement()
    {
        Vector3 dir = Movement.transform.right * (_strafeDirection * Agent.AgentBoidData.StrafeSpeed);
        Movement.ApplySmoothForce(Agent.GetDirectionWithEscapeFromGroupForce(dir));
    }
    
    private void UpdateStrafeDuration()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= _strafeDuration)
        {
            StateMachine.SetCurrentState<BotSeek>();
        }
    }
}
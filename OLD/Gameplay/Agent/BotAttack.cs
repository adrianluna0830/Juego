using Core.StateMachine;
using UnityEngine;
internal class BotAttack : AgentBaseState
{

    public BotAttack(FiniteStateMachine stateMachine, PhysicsMovement movement, AgentBoid agent, CharacterAnimation characterAnimation) : base(stateMachine, movement, agent, characterAnimation)
    {
    }

    public override void OnEnter()
    {
        // _characterAnimation.PlayAnimation(AnimationType.MOVE);
        
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        Vector3 dirClamped =
            Agent.GetDirectionWithEscapeFromGroupForce(Agent.GetAppliedAgentAccelerationDirection(GetDirectionToPlayer()).normalized);

        if (Vector3.Distance(CombatContext.Instance.GetPlayerPosition(), Agent.transform.position) < 1.5f)
        {
 
        }
        else
        {
            Movement.ApplySmoothForce(dirClamped.normalized * 6,1.5f);

        }
        
        
        

    }
}
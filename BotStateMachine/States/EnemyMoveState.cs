using Core.StateMachine;
using UnityEngine;

public class EnemyMoveState : BaseState
{
    private FiniteStateMachine _baseStateMachine;
    private FiniteStateMachine _subStateMachine;
    
    private IHitStatus _hitStatus;
    private readonly EnemyMovement _movement;
    private readonly AgentBoid _agentBoid;
    private EnemyCombat _enemyAttack;
    public EnemyMoveState(FiniteStateMachine baseStateMachine, IHitStatus hitStatus,EnemyMovement movement,AgentBoid agentBoid, EnemyCombat enemyAttack)
    {
        _baseStateMachine = baseStateMachine;
        _hitStatus = hitStatus;
        _movement = movement;
        _agentBoid = agentBoid;
        _enemyAttack = enemyAttack;

        _subStateMachine = new FiniteStateMachine();
        
        _subStateMachine.RegisterState(new EnemyReposition()).RegisterState(new EnemyFollowPlayer(_baseStateMachine,_movement,agentBoid,_enemyAttack));
        
        // _subStateMachine.SetCurrentState<EnemyFollowPlayer>();
        
        
    }

    public override void OnEnter()
    {
        _hitStatus.OnHitStartEvent += HitStatusOnOnHitStartEvent;
    }

    private void HitStatusOnOnHitStartEvent()
    {
        _baseStateMachine.SetCurrentState<EnemytHit>();
    }
    
    public override void OnExit()
    {
        _hitStatus.OnHitStartEvent -= HitStatusOnOnHitStartEvent;
    }

    public override void OnUpdate()
    {

        Vector3 position = CombatContext.Instance._ringManager.ObtenerPosicionEnRing(_agentBoid.transform);
        position.y = 0;
        var pos = _agentBoid.transform.position;
        var dir = position - pos;
        dir.y = 0;
        // DebugExtension.DebugArrow(_agentBoid.transform.position + new Vector3(0,2,0),dir);

        if (Vector3.Distance(position, pos) > 1)
        {
            _movement.Walk(dir);
            
        }
        
        
        
        // _subStateMachine.Update();
        
    }


}
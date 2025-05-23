using Core.StateMachine;
using UnityEngine;

public class EnemyFollowPlayer: BaseState
{
    private readonly FiniteStateMachine _baseState;
    private EnemyMovement _characterController;
    private EnemyCombat _enemyAttack;
    private readonly AgentBoid _agentBoid;

    public EnemyFollowPlayer(FiniteStateMachine baseState, EnemyMovement characterController,AgentBoid agentBoid, EnemyCombat enemyAttack)
    {
        _baseState = baseState;
        _characterController = characterController;
        _agentBoid = agentBoid;
        _enemyAttack = enemyAttack;
    }

    public override void OnUpdate()
    {
        var direction = CombatContext.Instance.GetPlayerPosition() - _agentBoid.transform.position;
        direction.y = 0;
        
        _characterController.Jog(direction);
        if (Vector3.Distance(CombatContext.Instance.GetPlayerPosition(),_enemyAttack.transform.position) < _enemyAttack.distanceToShowCounter)
        {
            _enemyAttack.canCounter = true;
            _enemyAttack._counter.SetCounterIconActive(true);
        }
        if (_enemyAttack.CanAttack( CombatContext.Instance.GetPlayerPosition()))
        {
            _baseState.SetCurrentState<EnemyAttack>();
        }
    }
}
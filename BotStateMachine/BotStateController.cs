using System;
using Core.StateMachine;
using UnityEngine;


public class BotStateController : MonoBehaviour
{
    private FiniteStateMachine _finiteStateMachine;

    [SerializeField] private AgentBoid _agentBoid;
    [SerializeField] private CharacterAnimation characterAnimation;
    [SerializeField] private HitReactStatus hitStatus;
    [SerializeField] private EnemyMovement enemyMovement;
    [SerializeField] private EnemyCombat _enemyAttack;
    [SerializeField] private Health health;

    private void Awake()
    {
        _finiteStateMachine = new FiniteStateMachine();
    }

    private void Start()
    {
        var enemyMoveState = new EnemyMoveState(_finiteStateMachine,hitStatus,enemyMovement,_agentBoid,_enemyAttack);
        var enemyHitState = new EnemytHit(_finiteStateMachine,hitStatus,health,_enemyAttack);
        var enemyAttackState = new EnemyAttack(_finiteStateMachine,hitStatus,_enemyAttack);
        var deathState = new EnemyDeathState(GetComponent<OnDeath>());
        _finiteStateMachine.RegisterState(enemyMoveState).RegisterState(enemyHitState).RegisterState(enemyAttackState)
            .RegisterState(deathState);
        _finiteStateMachine.SetCurrentState<EnemyMoveState>();

    }


    private void Update()
    {
        _finiteStateMachine.Update();
    }
}


using Core.StateMachine;

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

        _subStateMachine
            .RegisterState(new EnemyFollowPlayer(_baseStateMachine, _movement, agentBoid, _enemyAttack))
            .RegisterState(new EnemyReposition(_baseStateMachine, _movement, agentBoid, _enemyAttack))
            .RegisterState(new EnemyOnCounter(_movement, _enemyAttack));

        _subStateMachine.SetCurrentState<EnemyReposition>();
        
        
    }

    public override void OnEnter()
    {        _subStateMachine.SetCurrentState<EnemyReposition>();
        
        _enemyAttack.onCounter += EnemyAttackOnonCounter;
        _enemyAttack.startAttackphase += EnemyAttackOnstartAttackphase;
        _hitStatus.OnHitStartEvent += HitStatusOnOnHitStartEvent;
        _movement.PlayMovementAnimation();
    }

    private void EnemyAttackOnonCounter()
    {
        _subStateMachine.SetCurrentState<EnemyOnCounter>();
    }

    private void EnemyAttackOnstartAttackphase()
    {
        _subStateMachine.SetCurrentState<EnemyFollowPlayer>();

    }

    private void HitStatusOnOnHitStartEvent()
    {
        _baseStateMachine.SetCurrentState<EnemytHit>();
    }
    
    public override void OnExit()
    {
        _enemyAttack.startAttackphase -= EnemyAttackOnstartAttackphase;
        _enemyAttack.onCounter -= EnemyAttackOnonCounter;

        _hitStatus.OnHitStartEvent -= HitStatusOnOnHitStartEvent;
    }

    public override void OnUpdate()
    {


        _subStateMachine.Update();
        
    }


}

public class EnemyOnCounter : BaseState
{
    private readonly EnemyMovement _movement;
    private readonly EnemyCombat _enemyAttack;

    public EnemyOnCounter(EnemyMovement movement, EnemyCombat enemyAttack)
    {
        _movement = movement;
        _enemyAttack = enemyAttack;
    }

    public override void OnEnter()
    {
        _enemyAttack.animancer.States.Current.IsPlaying = false;
        _enemyAttack.canCounter = false;
        _enemyAttack._counter.SetCounterIconActive(false);
    }
}
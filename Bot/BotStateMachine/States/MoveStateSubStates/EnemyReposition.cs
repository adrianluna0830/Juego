using Core.StateMachine;
using UnityEngine;

public class EnemyReposition : BaseState
{
    private readonly FiniteStateMachine _baseStateMachine;
    private readonly EnemyMovement _movement;
    private readonly AgentBoid _agentBoid;
    private readonly EnemyCombat _enemyAttack;

    private const float STOP_DISTANCE = 2f;
    private const float MIN_STRAFE_TIME = 2f;
    private const float MAX_STRAFE_TIME = 4f;
    private const float STRAFE_SPEED = 1f;

    private float _strafeTimer;
    private float _nextStrafeChangeTime;
    private Vector3 _currentStrafeDirection;

    private enum StrafeDirection
    {
        None,
        Left,
        Right
    }

    public EnemyReposition(FiniteStateMachine baseStateMachine, EnemyMovement movement,
        AgentBoid agentBoid, EnemyCombat enemyAttack)
    {
        _baseStateMachine = baseStateMachine;
        _movement = movement;
        _agentBoid = agentBoid;
        _enemyAttack = enemyAttack;
    }

    public override void OnEnter()
    {
        ChangeStrafingDirection();
    }

    public override void OnUpdate()
    {
        Vector3 targetPosition = CombatContext.Instance._ringManager.ObtenerPosicionEnRing(_agentBoid.transform);
        targetPosition.y = 0;

        Vector3 directionToTarget = targetPosition - _agentBoid.transform.position;
        directionToTarget.y = 0;
        float distanceToTarget = directionToTarget.magnitude;

        UpdateStrafingTimer();

        Vector3 escape = _agentBoid.GetEscapeFromGroupForce();

        if (distanceToTarget <= STOP_DISTANCE)
        {
            HandleStrafingMovement(directionToTarget.normalized, escape);
        }
        else
        {
            Vector3 moveDirection = directionToTarget.normalized + _currentStrafeDirection;
            _movement.Walk(Vector3.ClampMagnitude(moveDirection + escape, 1f));
        }
    }

    private void UpdateStrafingTimer()
    {
        _strafeTimer += Time.deltaTime;
        if (_strafeTimer >= _nextStrafeChangeTime)
        {
            ChangeStrafingDirection();
        }
    }

    private void ChangeStrafingDirection()
    {
        _strafeTimer = 0f;
        _nextStrafeChangeTime = Random.Range(MIN_STRAFE_TIME, MAX_STRAFE_TIME);

        StrafeDirection newDirection = (StrafeDirection)Random.Range(0, 3);
        _currentStrafeDirection = newDirection switch
        {
            StrafeDirection.None => Vector3.zero,
            StrafeDirection.Left => CalculatePerpendicularDirection(true),
            StrafeDirection.Right => CalculatePerpendicularDirection(false),
            _ => Vector3.zero
        };
    }

    private Vector3 CalculatePerpendicularDirection(bool isLeft)
    {
        Vector3 toTarget = (CombatContext.Instance._ringManager.ObtenerPosicionEnRing(_agentBoid.transform) -
                            _agentBoid.transform.position).normalized;

        Vector3 perpendicular =
            isLeft ? new Vector3(-toTarget.z, 0, toTarget.x) : new Vector3(toTarget.z, 0, -toTarget.x);

        return perpendicular * STRAFE_SPEED;
    }

    private void HandleStrafingMovement(Vector3 forwardDirection, Vector3 escape)
    {
        if (_currentStrafeDirection == Vector3.zero)
        {
            _movement.Walk(escape);
            return;
        }

        Vector3 movement = _currentStrafeDirection + escape;
        _movement.Walk(Vector3.ClampMagnitude(movement, 1f));
    }
}
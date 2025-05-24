using System;
using Animancer;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private CharacterAnimation _characterAnimation;
    [SerializeField] private PhysicsMovement physicsMovement;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float jogSpeed;

    [SerializeField] private float rotationSpeed;
    [SerializeField] private float moveTowardsLerpValue;
    [SerializeField] private float smoothTime;

    [SerializeField] public float offset;


    private Vector3 _currentVelocity;

    private void Awake()
    {
        physicsMovement.SetDrag(0.99f);
    }

    public void RotateToPlayer()
    {
        Vector3 targetDirection = CombatContext.Instance.GetPlayerPosition() - transform.position;
        targetDirection.y = 0;
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(targetDirection),
            rotationSpeed * Time.deltaTime
        );
    }

    private void MoveCharacter(Vector3 direction, float speed)
    {
        direction.y = 0;
        Vector3 targetVelocity = direction * speed;
        Vector3 smoothedVelocity = Vector3.SmoothDamp(
            physicsMovement.GetVelocity(), 
            targetVelocity, 
            ref _currentVelocity, 
            smoothTime
        );
        physicsMovement.SetVelocity(smoothedVelocity);
        // physicsMovement.ApplyForce(direction * accel,speed);
        RotateToPlayer();
        UpdateMovementAnimation();

    }

    private void Update()
    {
    }

    public void Walk(Vector3 direction) => MoveCharacter(direction, walkSpeed);
    public void Jog(Vector3 direction) => MoveCharacter(direction, jogSpeed);

    private AnimancerState animation;

    public void UpdateMovementAnimation()
    {
        if(animation == null) return;
        var state = animation as MixerState<Vector2>;
        Vector3 worldVel = physicsMovement.GetVelocity();
        Vector3 localVel = transform.InverseTransformDirection(worldVel);
        Vector2 param = new Vector2(localVel.x, localVel.z);
        // DebugExtension.DebugArrow(transform.position + new Vector3(0,3,0),localVel,Color.black);
        Vector2 smoothedParam = Vector2.Lerp(state.Parameter, param, moveTowardsLerpValue * Time.deltaTime);

        state.Parameter = smoothedParam;
    }
    public void PlayMovementAnimation()
    {
        animation = _characterAnimation.PlayAnimation("DirectionalMove") as MixerState<Vector2>;
        animation.Time = UnityEngine.Random.Range(0f, 1f);

    }
}
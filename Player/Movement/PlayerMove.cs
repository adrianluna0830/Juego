using System;
using Animancer;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private PhysicsMovement physicsMovement;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float drag;
    [SerializeField] private float acceleration;
    [SerializeField] private float rotationSpeedToMoveDir;

    [SerializeField] private Transform camera;
    [SerializeField] private CharacterAnimation _characterAnimation;
    private void Awake()
    {
        physicsMovement.SetDrag(drag);
        physicsMovement.SetGravity(0);
    }

    private MixerState<float> animancerMoveState;
    public void MoveRelativeToCamera(Vector2 input,bool rotateToMoveDir = false)
    {
        Vector3 direction = GetHorizontalMoveDirectionRelativeToCamera(input);
        physicsMovement.ApplyForce(direction * acceleration, maxSpeed);
        
        if(direction != Vector3.zero && rotateToMoveDir)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), rotationSpeedToMoveDir * Time.deltaTime);

        animancerMoveState = _characterAnimation.PlayAnimation("LinealMove") as MixerState<float>;

        
    }

    private void Update()
    {
        if(animancerMoveState == null) return;
        animancerMoveState.Parameter = GetSpeedNormalized();
    }


    public float GetSpeedNormalized()
    {
        return Mathf.InverseLerp(0, maxSpeed, physicsMovement.GetVelocity().magnitude);
    }

    public Vector3 GetHorizontalMoveDirectionRelativeToCamera(Vector2 input)
    {
        var dir = camera.forward * input.y + camera.right * input.x;
        dir.y = 0;
        return dir.normalized;
    }
}
using System;
using UnityEngine;

namespace Core
{
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField] protected PhysicsMovement physicsMovement;
        [SerializeField] private Transform relativeTo;
        [SerializeField] protected float acceleration;
        [SerializeField] protected float drag;
        [SerializeField] protected float speedLimit;
        
        private void Awake()
        {
            physicsMovement.SetDrag(drag);
            physicsMovement.SetGravity(0);
        }

        public float GetClampedSpeed()
        {
            return Mathf.InverseLerp(0, speedLimit, physicsMovement.GetVelocity().magnitude);
        }

        public void MoveRelative(Vector2 input)
        {
                        
            Vector3 relativeDirection = relativeTo.forward * input.y + relativeTo.right * input.x;
            relativeDirection.y = 0;
            
            
            physicsMovement.ApplyForce(new Vector3(relativeDirection.x, 0, relativeDirection.z).normalized * acceleration,speedLimit);

        }

        public void MoveGlobal(Vector2 input)
        {
            physicsMovement.ApplyForce(new Vector3(input.x, 0, input.y).normalized * acceleration,speedLimit);

            
        }

        
    }
}
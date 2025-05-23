using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerRotator : MonoBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Transform playerTransform;
        
        [SerializeField] private float rotateSpeed;
        

        public void RotateRespectToCameraSmooth(Vector2 relativeDirection)
        {
            
            if(relativeDirection == Vector2.zero)  return;
            
            Vector3 newDir = cameraTransform.forward * relativeDirection.y + cameraTransform.right * relativeDirection.x;
            newDir.y = 0;

            playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, Quaternion.LookRotation(newDir), rotateSpeed * Time.deltaTime);
        }

        public void RotateRespectToCameraImmediate(Vector2 relativeDirection)
        {
            if(relativeDirection == Vector2.zero)  return;

            playerTransform.rotation = Quaternion.LookRotation(relativeDirection);
        }

        public void RotateTowards(Vector3 direction)
        {
            if(direction == Vector3.zero) return;
            
            playerTransform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private Transform _target;
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = _target.position;
    }
}

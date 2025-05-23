using System;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    [SerializeField] Transform target;

    private void LateUpdate()
    {
        transform.position = target.position;
    }
}

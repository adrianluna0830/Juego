using UnityEngine;

public interface IHitManager
{
    public bool CanBeHit();
    public void Hit(HitContext hitContext);

    public Transform Transform { get; }
}
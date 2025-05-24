using System;
using DefaultNamespace;
using UnityEngine;

public class HitManager :MonoBehaviour, IHitManager
{
    private IHitProcessor[] _hitProcessors;
    private IHitReact[] _hitReacts;

    private void Awake()
    {
        _hitProcessors = GetComponents<IHitProcessor>();
        _hitReacts = GetComponents<IHitReact>();

    }

    bool IHitManager.CanBeHit()
    {
        foreach (var hitProcessor in _hitProcessors)
        {
            if(!hitProcessor.HitCanBeProcessed()) return false;
        }
        return true;
    }

    public void Hit(HitContext hitContext)
    {
        foreach (var hitProcessor in _hitProcessors)
        {
            hitProcessor.ProccesssHit(hitContext);
        }
        
        foreach (var hitReact in _hitReacts)
        {
            hitReact.ReactToHit(hitContext);
        }
        
        OnHitStartEvent?.Invoke();
    }
    
    public Transform Transform => transform;
    
    public event Action OnHitStartEvent;
}

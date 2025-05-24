using System;
using Animancer;
using Core;
using UnityEngine;

public class HitReactStatus : MonoBehaviour, IHitStatus,IHitReact
{
    
    [SerializeField] private AnimancerComponent animancer;

    [SerializeField] private AnimationProfile hitAnims;

    private bool isHitting = false;
    public bool IsHitting => isHitting;
    public event Action OnHitStartEvent;
    public event Action OnHitEndEvent;

    public void ReactToHit(HitContext proccessedHit)
    {
        OnHitStartEvent?.Invoke();
        isHitting = true;
        var state = animancer.Play(hitAnims.GetRandomAnimationBind().animation, 0.05f, FadeMode.FromStart);
        bool onEndCalled = false;
        state.Events(this).OnEnd = () =>
        {
            if (onEndCalled) return;
            onEndCalled = true;
            isHitting = false;
            OnHitEndEvent?.Invoke();
        };

    }
}

using System;
using System.Collections.Generic;
using Animancer;
using Core;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    [SerializeField] private AnimationProfile _animationProfile;
    [SerializeField] private AnimancerComponent animancer;
    
    private Dictionary<string,TransitionAsset> animationsDict = new();
    private Dictionary<string,AnimancerState> stateDict = new();

    private void Awake()
    {
        SetUpAnimations();
    }

    public AnimancerState GetState(string animationName)
    {
        if (!stateDict.TryGetValue(animationName, out var state)) return null;
        return state;
    }
    public AnimancerState PlayAnimation(string animationName)
    {
        if (!animationsDict.TryGetValue(animationName, out var animation)) throw new Exception("Animation not found :" + animationName);
        var state = animancer.Play(animation);
        stateDict[animationName] = state;
        return state;
    }
    
    public AnimancerState PlayAnimation(TransitionAsset clipTransition)
    {
        return animancer.Play(clipTransition);
    }
    
    public AnimancerState PlayAnimation(ClipTransition clipTransition)
    {
        return animancer.Play(clipTransition);
    }


    private void SetUpAnimations()
    {
        if (_animationProfile == null)
        {
            throw new Exception(" Animation profile is null");
        }

        foreach (var binding in _animationProfile.bindings)
        {
            if (!animationsDict.TryAdd(binding.animationName, binding.animation))
            {
                throw new Exception("There is already an animation with that name");
            }
        }
    }
}
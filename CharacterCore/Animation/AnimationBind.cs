using System;
using Animancer;
using UnityEngine.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class AnimationBind 
{
    public string animationName;
    public TransitionAsset animation;
}
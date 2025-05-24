using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    [CreateAssetMenu(menuName = "Create AnimationProfile", fileName = "AnimationProfile", order = 0)]
    public class AnimationProfile : ScriptableObject
    {
         public AnimationBind [] bindings;
    
         public AnimationBind GetRandomAnimationBind()
         {
             if (bindings == null || bindings.Length == 0)
                 return null;
    
             return bindings[Random.Range(0, bindings.Length)];
         }
    }
}
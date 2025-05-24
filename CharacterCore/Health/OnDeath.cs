using System;
using Animancer;
using Core;
using DefaultNamespace;
using UnityEngine;

public class OnDeath : MonoBehaviour
{
    public SoundCollection deathSound;
    public AnimancerComponent Component;
    public AnimationProfile deathAnim;
    public Health Health;

    private void Awake()
    {


    }

    public void PlayDeathSound()
    {
        var clip = deathSound.GetRandomClipWithVariation();
        AudioSourcePoolManager.Instance.PlaySound(clip.clip, transform.position, clip.volume, clip.pitch);
    }
    
    public void PlayDeathAnimation()
    {
        Component.Play(deathAnim.GetRandomAnimationBind().animation);

    }

    public void Die()
    {
        Destroy(gameObject, 5f);
    }

    private void OnDestroy()
    {
   }
}

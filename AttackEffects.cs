using System;
using System.Collections;
using DefaultNamespace;
using UnityEngine;

public class AttackEffects : MonoBehaviour
{
    [Header("Anim")] [SerializeField] private float animationAttackHit = 0.2f;

    [Header("Sounds")]
    [SerializeField] private SoundCollection swooshCollection;
    [SerializeField] private SoundCollection bloodHitCollection;
    [SerializeField] private SoundCollection hitCollection;
    [SerializeField] private Transform soundPosition;
    private IAttackStatusNotifier _attackStatusNotifier;

    private void Awake()
    {
        _attackStatusNotifier = GetComponent<IAttackStatusNotifier>();
        _attackStatusNotifier.OnAttackSwoosh += AttackStatusNotifierOnOnAttackSwoosh;
        _attackStatusNotifier.OnHitDealtEvent += AttackStatusNotifierOnOnHitDealtEvent;
    }

    private void OnDisable()
    {
         _attackStatusNotifier.OnAttackSwoosh -= AttackStatusNotifierOnOnAttackSwoosh;
        _attackStatusNotifier.OnHitDealtEvent -= AttackStatusNotifierOnOnHitDealtEvent;
    }

    private void AttackStatusNotifierOnOnHitDealtEvent(AttackInfo obj)
    {

        StartCoroutine(FreezGameplay());
        
        var blood = bloodHitCollection.GetRandomClipWithVariation();
        var hitImpact = hitCollection.GetRandomClipWithVariation();
        AudioSourcePoolManager.Instance.PlaySound(blood.clip, soundPosition.position, blood.volume, blood.pitch);
        AudioSourcePoolManager.Instance.PlaySound(hitImpact.clip, soundPosition.position, hitImpact.volume, hitImpact.pitch);


    }

    private IEnumerator FreezGameplay()
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(animationAttackHit);
        Time.timeScale = 1;

        
    }
    
    
    


    private void AttackStatusNotifierOnOnAttackSwoosh()
    {
        var swoosh = swooshCollection.GetRandomClipWithVariation();
        AudioSourcePoolManager.Instance.PlaySound(swoosh.clip, soundPosition.position, swoosh.volume, swoosh.pitch);
    }
}

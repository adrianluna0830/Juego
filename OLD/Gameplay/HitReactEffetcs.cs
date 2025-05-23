// using System;
// using System.Collections;
// using Animancer;
// using UnityEngine;
// using DG.Tweening;
// using Unity.Cinemachine;
//
// public class HitReactEffetcs : MonoBehaviour
// {
//     [SerializeField] private CharacterAnimation characterAnimation;
//     [SerializeField] private animation hitReactAnimation;
//     [SerializeField] private Transform characterModel;
//     [SerializeField] private float hitFreezeTime;
//     [SerializeField] private float shakeDuration;
//     [SerializeField] private int shakeVibrato;
//     [SerializeField] private float shakeAmount;
//     [SerializeField] private float cameraSkakeForce;
//     [SerializeField] private float cameraZoomInForce;
//     [SerializeField] private CinemachineImpulseSource  shakeSource;
//     [SerializeField] private CinemachineImpulseSource zoomSource;
//     private void Update()
//     {
//                 
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             React();
//         }
//
//     }
//     
//     public void React()
//     {
//         StopAllCoroutines();
//
//         StartCoroutine(FreeAnimCorutine(hitFreezeTime,hitReactAnimation));
//     }
//
//     private IEnumerator FreeAnimCorutine(float freezeTime,animation bind)
//     {
//
//         TransitionAsset hitAnimation = bind.GetAnimation();
//         
//         bool freeze = false;
//
//         bool isPlayingAnim = true; 
//         
//         characterAnimation.PlayAnimation(hitReactAnimation,animationEventCallback: @event =>
//             {
//                 freeze = true;
//                 ShakeCharacter();
//                 shakeSource.GenerateImpulseWithForce(cameraSkakeForce);
//                 zoomSource.GenerateImpulseWithForce(cameraZoomInForce);
//             }
//             ,() => isPlayingAnim = false
//             ,true);
//         
//         float timeElapsed = 0;
//         
//         while (isPlayingAnim)
//         {
//             if (freeze)
//             {
//                 if (timeElapsed < hitFreezeTime)
//                 {
//                     characterAnimation.ChangeAnimationSpeed(hitAnimation, 0);
//                     timeElapsed += Time.deltaTime;
//                     Debug.Log(timeElapsed);
//
//                     if (timeElapsed >= freezeTime)
//                     {
//                         characterAnimation.ChangeAnimationSpeed(hitAnimation, 1);
//                     }
//                 }
//             }
//             
//             yield return null;
//         }
//         
//         
//     }
//
//     public void ShakeCharacter()
//     {
//         characterModel.DOKill(true);
//         characterModel.DOShakePosition(hitFreezeTime, shakeAmount * new Vector3(1, 0, 1), shakeVibrato).onComplete =
//             () => { characterModel.transform.localPosition = Vector3.zero; };
//     }
//
//
// }

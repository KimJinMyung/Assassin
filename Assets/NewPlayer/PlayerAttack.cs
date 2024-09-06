using EventEnum;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player 
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private Transform shakeCamera;

        private bool isShakeRotate;
        private bool isAttackAble;
        private bool isBattleMode;
        private bool isDefense;
        private bool isParryAble;

        private Vector3 originPos;
        private Quaternion originRotation;

        private Animator animator;

        private readonly int hashAttack = Animator.StringToHash("Attack");
        private readonly int hashDefense = Animator.StringToHash("Defense");
        private readonly int hashDefenseStart = Animator.StringToHash("DefenseStart");
        private readonly int hashParry = Animator.StringToHash("Parry");

        private void Awake()
        {
            animator = GetComponentInChildren<Animator>();

            AddEvent();
        }

        private void OnDestroy()
        {
            RemoveEvent();
        }

        private void AddEvent()
        {
            EventManager<PlayerAction>.Binding<bool>(true, PlayerAction.SetAttackAble, SetAttackAble);
            EventManager<PlayerAction>.Binding<bool>(true, PlayerAction.ParryAble_PlayerAttack, SetParring);
            EventManager<CameraEvent>.Binding(true, CameraEvent.PlayerAttackSuccess, ShakeCamera);
        }

        private void RemoveEvent()
        {
            EventManager<PlayerAction>.Binding<bool>(false, PlayerAction.SetAttackAble, SetAttackAble);
            EventManager<PlayerAction>.Binding<bool>(false, PlayerAction.ParryAble_PlayerAttack, SetParring);
            EventManager<CameraEvent>.Binding(true, CameraEvent.PlayerAttackSuccess, ShakeCamera);
        }

        private void OnEnable()
        {
            isAttackAble = true;
            isBattleMode = false;
            isDefense = false;
            isShakeRotate = false;
            isParryAble = true;
        }

        private void SetAttackAble(bool isAttackAble)
        {
            this.isAttackAble = isAttackAble;
        }

        private void SetParring(bool isParring) 
        {
            this.isParryAble = isParring;
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.ParryAble_PlayerView, this.isParryAble);
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                if (!isAttackAble) return;

                if (!isBattleMode)
                    isBattleMode = true;

                if (!isDefense)
                {
                    animator.SetTrigger(hashAttack);
                }                    
                else if(isParryAble)
                    animator.SetTrigger(hashParry);
            }
        }

        public void OnDefense(InputAction.CallbackContext context)
        {
            isDefense = context.ReadValue<float>() > 0.5f;

            if (isDefense && !animator.GetBool(hashDefense))
            {
                animator.SetTrigger(hashDefenseStart);
            }
            animator.SetBool(hashDefense, isDefense);           

            EventManager<AttackBoxEvent>.TriggerEvent(AttackBoxEvent.IsDefense, isDefense);
            EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsDefense, isDefense);
        }

        private void ShakeCamera()
        {
            originPos = shakeCamera.transform.localPosition;
            originRotation = shakeCamera.transform.localRotation;

            StartCoroutine(ShakingCamera());
        }

        IEnumerator ShakingCamera(float duration = 0.05f,
                                    float magnitudePos = 0.09f,
                                    float magnitudeRot = 0.1f)
        {
            float passTime = 0f;

            while(passTime < duration)
            {
                Vector3 shakePos = Random.insideUnitSphere;
                shakeCamera.localPosition += shakePos * magnitudePos;

                //if(isShakeRotate)
                //{
                //    Vector3 shakeRot = new Vector3(0, 0, Mathf.PerlinNoise(Time.time * magnitudeRot, 0.0f));
                    
                //    shakeCamera.localRotation = Quaternion.Euler(shakeRot);
                //}

                passTime += Time.deltaTime;

                yield return null;  
            }

            shakeCamera.localPosition = originPos;
            shakeCamera.localRotation = originRotation;
        }
    }
}

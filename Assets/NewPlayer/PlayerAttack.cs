using EventEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player 
{
    public class PlayerAttack : MonoBehaviour
    {
        private bool isAttackAble;
        private bool isBattleMode;
        private bool isDefense;
        private bool isParryAble;

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
            EventManager<PlayerAction>.Binding<bool>(true, PlayerAction.ParryAble, SetParring);
        }

        private void RemoveEvent()
        {
            EventManager<PlayerAction>.Binding<bool>(false, PlayerAction.SetAttackAble, SetAttackAble);
            EventManager<PlayerAction>.Binding<bool>(false, PlayerAction.ParryAble, SetParring);
        }

        private void OnEnable()
        {
            isAttackAble = true;
            isBattleMode = false;
            isDefense = false;
            isParryAble = false;
        }

        private void SetAttackAble(bool isAttackAble)
        {
            this.isAttackAble = isAttackAble;
        }

        private void SetParring(bool isParring) 
        {
            this.isParryAble = isParring;
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                if (!isAttackAble) return;

                if (!isBattleMode)
                    isBattleMode = true;

                if(!isDefense)
                    animator.SetTrigger(hashAttack);
                else if(!isParryAble)
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
    }
}

using EventEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player 
{
    public class PlayerAttack : MonoBehaviour
    {
        private bool isAttackAble;
        private bool isBattleMode;

        private Animator animator;

        private readonly int hashAttack = Animator.StringToHash("Attack");

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
        }

        private void RemoveEvent()
        {
            EventManager<PlayerAction>.Binding<bool>(false, PlayerAction.SetAttackAble, SetAttackAble);
        }

        private void OnEnable()
        {
            isAttackAble = true;
            isBattleMode = false;
        }

        private void SetAttackAble(bool isAttackAble)
        {
            this.isAttackAble = isAttackAble;
        }

        public void OnAttack()
        {
            if (!isAttackAble) return;

            if(!isBattleMode) 
                isBattleMode = true;

            animator.SetTrigger(hashAttack);
        }
    }
}

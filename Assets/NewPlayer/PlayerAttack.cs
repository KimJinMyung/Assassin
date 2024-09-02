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

        private void Awake()
        {
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

        private void SetAttackAble(bool isAttackAble)
        {
            this.isAttackAble = isAttackAble;
        }
    }
}

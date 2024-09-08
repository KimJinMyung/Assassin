using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using UnityEditor;
using EventEnum;

namespace Player
{
    public class PlayerAttackBox : MonoBehaviour
    {
        [SerializeField] private LayerMask AttackTargetLayer;
        private Collider AttackBox;

        private PlayerView playerView;

        private bool isDefense;
        private List<MonsterView> HurtMonster = new List<MonsterView>();

        private void Awake()
        {
            playerView = GetComponentInParent<PlayerView>();
            AttackBox = GetComponent<Collider>();

            AddEvent();
        }

        private void OnDestroy()
        {
            RemoveEvent();
        }

        private void AddEvent()
        {
            EventManager<AttackBoxEvent>.Binding<bool>(true, AttackBoxEvent.IsDefense, SetDefense);
            EventManager<AttackBoxEvent>.Binding<bool>(true, AttackBoxEvent.IsAttacking, SetAttackBox);
            EventManager<AttackBoxEvent>.Binding(true, AttackBoxEvent.HitMonsterReset, HitMonsterReset);
        }

        private void RemoveEvent()
        {
            EventManager<AttackBoxEvent>.Binding<bool>(false, AttackBoxEvent.IsDefense, SetDefense);
            EventManager<AttackBoxEvent>.Binding<bool>(false, AttackBoxEvent.IsAttacking, SetAttackBox);
            EventManager<AttackBoxEvent>.Binding(false, AttackBoxEvent.HitMonsterReset, HitMonsterReset);
        }

        private void Start()
        {
            AttackBox.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("RopePoint")) return;
            if (((1 << other.gameObject.layer) & AttackTargetLayer) != 0)
            {
                // 디펜스 모드가 아니면
                if(!isDefense)
                    Attack(other);
            }
        }

        private void Attack(Collider other)
        {
            var monsterView = other.GetComponent<MonsterView>();
            if (monsterView == null) return;
            if (HurtMonster.Contains(monsterView)) return;

            monsterView.Hurt(playerView, playerView.ViewModel.AttackDamage);
            if(monsterView.vm.HP <= 0)
            {
                EventManager<PlayerAction>.TriggerEvent(PlayerAction.AttackLockOnEnable, monsterView);
            }

            EventManager<CameraEvent>.TriggerEvent(CameraEvent.PlayerAttackSuccess);

            HurtMonster.Add(monsterView);
        }

        private void SetDefense(bool isDefense)
        {
            if (this.isDefense == isDefense) return;
            this.isDefense = isDefense;
        }

        private void SetAttackBox(bool isAttack)
        {
            if (isAttack == AttackBox.enabled) return;

            AttackBox.enabled = isAttack;
        }

        private void HitMonsterReset()
        {
            HurtMonster.Clear();
        }
    }
}

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

        private PlayerView playerView;

        private bool isDefense;
        private List<MonsterView> HurtMonster = new List<MonsterView>();

        private void Awake()
        {
            playerView = GetComponentInParent<PlayerView>();

            AddEvent();
        }

        private void OnDestroy()
        {
            RemoveEvent();
        }

        private void AddEvent()
        {
            EventManager<AttackBoxEvent>.Binding<bool>(true, AttackBoxEvent.IsDefense, SetDefense);
        }

        private void RemoveEvent()
        {
            EventManager<AttackBoxEvent>.Binding<bool>(false, AttackBoxEvent.IsDefense, SetDefense);
        }

        private void OnEnable()
        {
            HurtMonster.Clear();
        }

        private void Start()
        {
            this.enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
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
            if (HurtMonster.Contains(monsterView)) return;

            monsterView.Hurt(playerView, playerView.ViewModel.AttackDamage);
            EventManager<CameraEvent>.TriggerEvent(CameraEvent.PlayerAttackSuccess);

            HurtMonster.Add(monsterView);
        }

        private void SetDefense(bool isDefense)
        {
            if (this.isDefense == isDefense) return;
            this.isDefense = isDefense;
        }
    }
}

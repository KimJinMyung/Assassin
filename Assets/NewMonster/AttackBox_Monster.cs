using EventEnum;
using Player;
using UnityEngine;

namespace Monster
{
    public class AttackBox_Monster : MonoBehaviour
    {
        [SerializeField] LayerMask _attackTargetLayer;

        private BoxCollider boxCollider;
        private MonsterView MonsterView;

        private void Awake()
        {
            MonsterView = GetComponentInParent<MonsterView>();
            boxCollider = GetComponent<BoxCollider>();

            EventManager<MonsterEvent>.Binding<int, bool>(true, MonsterEvent.Attack, SetEnableAttackBox);
        }

        private void OnDestroy()
        {
            EventManager<MonsterEvent>.Binding<int, bool>(false, MonsterEvent.Attack, SetEnableAttackBox);
        }

        private void Start()
        {
            boxCollider.enabled = false;
        }

        private void SetEnableAttackBox(int instanceID,bool isEnable)
        {
            var condition1 = instanceID != MonsterView.monsterId;
            Debug.Log($"{instanceID} : {MonsterView.monsterId}");

            if (condition1) return;
            if (isEnable == boxCollider.enabled) return;

            boxCollider.enabled = isEnable;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _attackTargetLayer) != 0)
            {
                Attack(other.transform);
            }
        }

        private void Attack(Transform target)
        {
            var Player = target.GetComponent<PlayerView>();

            if(Player != null) 
                Player.Hurt(MonsterView, MonsterView._monsterData.ATK);
        }
    }
}

using EventEnum;
using Player;
using UnityEngine;

namespace Monster
{
    public class AttackBox_Monster : MonoBehaviour
    {
        [SerializeField] LayerMask _attackTargetLayer;

        private BoxCollider boxCollider;
        private SphereCollider JumpAttackCollider;
        private MonsterView MonsterView;

        private void Awake()
        {
            MonsterView = GetComponentInParent<MonsterView>();
            JumpAttackCollider = GetComponent<SphereCollider>();
            boxCollider = GetComponent<BoxCollider>();

            EventManager<MonsterEvent>.Binding<int, bool>(true, MonsterEvent.AttackColliderOn, SetEnableAttackBox);
            EventManager<MonsterEvent>.Binding<int, bool>(true, MonsterEvent.JumpAttackColliderOn, SetJumpAttackBox);
        }

        private void OnDestroy()
        {
            EventManager<MonsterEvent>.Binding<int, bool>(false, MonsterEvent.AttackColliderOn, SetEnableAttackBox);
            EventManager<MonsterEvent>.Binding<int, bool>(false, MonsterEvent.JumpAttackColliderOn, SetJumpAttackBox);
        }

        private void Start()
        {
            boxCollider.enabled = false;
            if(JumpAttackCollider == null) return;
            JumpAttackCollider.enabled = false;
        }

        private void SetEnableAttackBox(int instanceID,bool isEnable)
        {
            var condition1 = instanceID != MonsterView.monsterId;

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

        private void SetJumpAttackBox(int monsterId, bool isEnable)
        {
            if (MonsterView.monsterId != monsterId || JumpAttackCollider == null) return;

            JumpAttackCollider.enabled = isEnable;
        }
    }
}

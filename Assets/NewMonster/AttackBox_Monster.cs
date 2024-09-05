using Player;
using UnityEngine;

namespace Monster
{
    public class AttackBox_Monster : MonoBehaviour
    {
        [SerializeField] LayerMask _attackTargetLayer;

        private MonsterView MonsterView;

        private void Awake()
        {
            MonsterView = GetComponentInParent<MonsterView>();
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

        private void Update()
        {
            Debug.Log("∏ÛΩ∫≈Õ Attack Box On");
        }
    }
}

using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackBox : MonoBehaviour
{
    private LayerMask _attackLayer;

    //공격 받는 Collider
    List<Transform> hitCollider;

    private List<Transform> _attackedObject = new List<Transform>();
    public List<Transform> AttackedMonster { get { return _attackedObject; } }

    private PlayerView owner_player;
    private MonsterView owner_monster;

    private Animator animator;
    private PlayerBattleManager attackManager;

    private void Awake()
    {
        owner_player = transform.GetComponentInParent<PlayerView>();
        owner_monster = transform.GetComponentInParent<MonsterView>();

        if (owner_player != null)
        {
            _attackLayer = LayerMask.GetMask("Monster", "LockOnAble", "LockOnTarget", "Incapacitated");
            animator = owner_player.GetComponent<Animator>();
            attackManager = owner_player.GetComponent<PlayerBattleManager>();
        }
        else if (owner_monster != null)
        {
            _attackLayer = LayerMask.GetMask("Player");
        }
    }

    private void OnEnable()
    {
        hitCollider = new List<Transform>();

        hitCollider.Clear();
        _attackedObject.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _attackLayer) != 0)
        {
            hitCollider.Add(other.transform);
        }
    }

    private void Update()
    {
        Attacking(); 

        if(owner_monster != null)
        {
            Debug.Log("Monster AttackBox On");
        }
    }

    public void Attacking()
    {
        foreach (var collider in hitCollider)
        {
            if (!this.gameObject.activeSelf) return;

            if (collider.transform == this.transform) continue;
            if (!_attackedObject.Contains(collider))
            {
                if (owner_player != null)
                {
                    if (!animator.GetBool("Defense"))
                    {
                        MonsterView target = collider.GetComponent<MonsterView>();
                        if (target != null && !animator.GetBool("Defense"))
                        {
                            target.Hurt(owner_player, owner_player.playerData.ATK);
                        }
                    }                    
                }
                else if (owner_monster != null)
                {
                    PlayerView target = collider.GetComponent<PlayerView>();
                    if (target != null)
                    {                        
                        target.Hurt(owner_monster, owner_monster._monsterData.ATK);
                    }
                }

                //데미지 부여한 몬스터 목록 추가
                _attackedObject.Add(collider);
            }
        }
    }
}

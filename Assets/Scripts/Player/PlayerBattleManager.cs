using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBattleManager : MonoBehaviour
{
    private PlayerView owner;
    private PlayerMovement ownerMovement;
    private Animator animator;

    private PlayerLockOn playerSight;

    [SerializeField] private LayerMask AttackTarget;

    public bool IsUpperPlayerToMonster;
    private MonsterView ViewMonster;

    public bool isUpper { get; private set; }
    public bool isForward { get; private set; }

    public bool isParried {  get; set; }
    private bool isDefence;

    private void Awake()
    {
        owner = GetComponent<PlayerView>();
        ownerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        playerSight = GetComponent<PlayerLockOn>();
    }

    private void Update()
    {
        if (playerSight.ViewModel.LockOnAbleTarget == null) return;
        ViewMonster = playerSight.ViewModel.LockOnAbleTarget.GetComponent<MonsterView>();

        if (ViewMonster == null) return;
        if ((owner.transform.position.y - (ViewMonster.transform.position.y + ViewMonster.MonsterHeight)) > 0f) IsUpperPlayerToMonster = true;
        else IsUpperPlayerToMonster = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + Vector3.up, new Vector3(0.6f, 1f, 4f));
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (owner.ViewModel.HP <= 0) return;
            if (!animator.GetBool("AttackAble")) return;

            if (isDefence)
            {
                //패링
                if (animator.GetBool("ParryAble"))
                {
                    //animator.SetBool("Parring", true);
                    animator.SetTrigger("Parry");
                }
                return;
            }
            else if (Physics.BoxCast(transform.position + Vector3.up, new Vector3(0.3f, 0.5f, 1f), owner.transform.forward, out RaycastHit hit, Quaternion.identity, 1f, AttackTarget))
            {
                MonsterView target = hit.transform.GetComponent<MonsterView>();
                if(target != null)
                {
                    isForward = Vector3.Dot(target.transform.forward, transform.forward) < 0.5f;    // true면 전방

                    if (isForward && target.isSubdued)
                    {
                        animator.SetFloat("isForward", 0);
                        animator.SetBool("Assassinated", true);
                        animator.SetTrigger("Assassinate");

                        //몬스터 암살당하는 모션 전방
                        
                        return;
                    }
                    else if (!isForward)
                    {
                        animator.SetFloat("isForward", 1);
                        animator.SetBool("Assassinated", true);
                        animator.SetTrigger("Assassinate");

                        //몬스터 암살당하는 모션 후방

                        return;
                    }
                }
            }            
            
            if (ViewMonster != null)
            {
                float distanceY = transform.position.y - ViewMonster.transform.position.y;

                if(distanceY > ViewMonster.MonsterHeight + 1)
                {
                    animator.SetBool("isUpper", true);
                    animator.SetBool("Assassinated", true);
                    animator.SetTrigger("Assassinate");

                    isForward = Vector3.Dot(ViewMonster.transform.forward, transform.forward) < 0.5f;    // true면 전방

                    if (isForward)
                    {
                        //뒤로 누워 암살당함
                    }
                    else
                    {
                        //앞으로 누워 암살당함
                    }
                }

                //LockOnTarget을 지정하지 않았다면 LockOnAbleTarget을 바라보며 공격
                bool condition1 = owner.ViewModel.LockOnTarget == null;
                bool condition2 = ownerMovement.vm.Movement.magnitude < 0.1f;

                if (condition1 && condition2)
                {
                    Vector3 dirTarget = ViewMonster.transform.position - transform.position;
                    dirTarget.y = 0;
                    Quaternion rotation = Quaternion.LookRotation(dirTarget);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, rotation.eulerAngles.y, 0), 100f * Time.fixedDeltaTime);
                }
            }

            //공격
            animator.SetTrigger("Attack");
        }
    }

    public void OnDefense(InputAction.CallbackContext context)
    {
        if (!animator.GetBool("AttackAble")) return;
        isDefence = context.ReadValue<float>() > 0.5f;

        if (isDefence && !animator.GetBool("Defense"))
        {
            animator.SetTrigger("DefenseStart");
        }

        animator.SetBool("Defense", isDefence);
    }
}

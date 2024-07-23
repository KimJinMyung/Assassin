using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBattleManager : MonoBehaviour
{
    private PlayerView owner;
    private PlayerMovement ownerMovement;
    private Animator animator;

    private PlayerLockOn playerSight;

    public bool isParried {  get; set; }
    private bool isDefence;

    private void Awake()
    {
        owner = GetComponent<PlayerView>();
        ownerMovement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        playerSight = GetComponent<PlayerLockOn>();
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
                if(animator.GetBool("ParryAble"))
                {
                    //animator.SetBool("Parring", true);
                    animator.SetTrigger("Parry");
                }
                return;
            }
            else
            {
                //LockOnTarget을 지정하지 않았다면 LockOnAbleTarget을 바라보며 공격
                bool condition1 = owner.ViewModel.LockOnTarget == null && playerSight.ViewModel.LockOnAbleTarget != null;
                bool condition2 = ownerMovement.vm.Movement.magnitude < 0.1f;

                if (condition1 && condition2)
                {
                    Vector3 dirTarget = playerSight.ViewModel.LockOnAbleTarget.position - transform.position;
                    dirTarget.y = 0;
                    Quaternion rotation = Quaternion.LookRotation(dirTarget);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, rotation.eulerAngles.y, 0), 100f * Time.fixedDeltaTime);
                }

                //공격
                animator.SetTrigger("Attack");
            }            
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

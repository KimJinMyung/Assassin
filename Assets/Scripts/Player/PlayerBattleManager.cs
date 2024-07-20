using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBattleManager : MonoBehaviour
{
    private PlayerView owner;
    private Animator animator;

    private bool isDefence;

    private void Awake()
    {
        owner = GetComponent<PlayerView>();
        animator = GetComponent<Animator>();    
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
                if(animator.GetBool("ParryAble")) animator.SetTrigger("Parry");
                return;
            }
            else
            {
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

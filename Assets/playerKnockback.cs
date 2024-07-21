using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerKnockback : StateMachineBehaviour
{
    private PlayerView owner;
    [SerializeField] private float knockbackTimer;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.GetComponent<PlayerView>(); 
        owner.isKnockback = true;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= knockbackTimer)
        {
            owner.isKnockback = false;
            animator.SetBool("isMoveAble", true);
        }
    }
}

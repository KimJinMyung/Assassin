using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossJump : StateMachineBehaviour
{

    [SerializeField] private float JumpStartTime;

    private int hashJumpping = Animator.StringToHash("Jumpping");
    private int hashNextAction = Animator.StringToHash("NextAction");

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= JumpStartTime)
        {
            animator.SetBool(hashJumpping, true);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(hashNextAction);
    }
}

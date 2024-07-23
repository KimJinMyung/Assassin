using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAssassinated : StateMachineBehaviour
{
    MonsterView owner;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.GetComponent<MonsterView>();
        animator.SetBool("Dead", true);

        Vector3 dir;

        if (animator.GetFloat("Forward") == 1)
        {
            dir = owner.transform.position - owner.vm.TraceTarget.position;
        }
        else
        {
            dir = owner.vm.TraceTarget.position - owner.transform.position;
        }

        dir.y = 0;
        owner.transform.rotation = Quaternion.LookRotation(dir.normalized);
    }
}

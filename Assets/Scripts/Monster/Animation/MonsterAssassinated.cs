using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class MonsterAssassinated : StateMachineBehaviour
{
    private MonsterView owner;

    private int hashDead = Animator.StringToHash("Dead");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.GetComponent<MonsterView>();

        owner.vm.RequestMonsterHPChanged(owner.monsterId, 0);


        if (animator.layerCount > 1)
            animator.SetLayerWeight(1, 0);

        Vector3 dir;

        if (animator.GetFloat("Forward") == 1)
        {
            //후방
            dir = owner.transform.position - owner.vm.TraceTarget.position;
        }
        else
        {
            //전방
            dir = owner.vm.TraceTarget.position - owner.transform.position;
        }

        dir.y = 0;
        owner.transform.rotation = Quaternion.LookRotation(dir.normalized);

        if(owner.vm.HP <= 0)
            animator.SetBool(hashDead, true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class MonsterAssassinated : StateMachineBehaviour
{
    private MonsterView owner;

    private float RecoveryTimer;
    private bool isNotDead;

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

        if (owner.vm.LifeCount >= 1)
        {
            isNotDead = true;
        }

        animator.SetBool(hashDead, true);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(RecoveryTimer >= 3f)
        {
           animator.SetBool(hashDead, false);
        }

        if (isNotDead && stateInfo.normalizedTime >= 1f)
        {
            RecoveryTimer = Mathf.Clamp(RecoveryTimer + Time.deltaTime, 0, 3f);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        RecoveryTimer = 0f;
    }
}

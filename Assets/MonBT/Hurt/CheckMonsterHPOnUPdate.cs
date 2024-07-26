using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Hurt")]
public class CheckMonsterHPOnUPdate : Action
{
    private MonsterView monsterView ;
    private Animator animator;

    int hashHurt = Animator.StringToHash("Hit");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        animator.SetBool(hashHurt, true);
        KnockBackAnimation(monsterView.KnockbackDir);
        return TaskStatus.Success;
        //if(monsterView.IsHurtAnimationRunning())
        //{
        //    return TaskStatus.Success;
        //}

        //return TaskStatus.Running;
    }

    private void KnockBackAnimation(Vector3 KnockbackDir)
    {
        KnockbackDir.y = 0;
        KnockbackDir.Normalize();

        animator.SetFloat("HurtDir_z", KnockbackDir.z);
        animator.SetFloat("HurtDir_x", KnockbackDir.x);
        animator.SetTrigger("Hurt");
    }
}

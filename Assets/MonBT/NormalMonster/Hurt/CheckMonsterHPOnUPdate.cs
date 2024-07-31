using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("Hurt")]
public class CheckMonsterHPOnUPdate : Action
{
    private MonsterView monsterView ;
    private Animator animator;
    private NavMeshAgent agent;

    [SerializeField] SharedBool isHurt;

    int hashHurt = Animator.StringToHash("Hurt");
    int hashHurtDirz = Animator.StringToHash("HurtDir_z");
    int hashHurtDirx = Animator.StringToHash("HurtDir_x");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
        agent = Owner.GetComponent<NavMeshAgent>();
    }

    public override TaskStatus OnUpdate()
    {
        if(!isHurt.Value) return TaskStatus.Failure;

        agent.ResetPath();
        KnockBackAnimation(monsterView.KnockbackDir);
        return TaskStatus.Success;
    }

    private void KnockBackAnimation(Vector3 KnockbackDir)
    {
        KnockbackDir.y = 0;
        KnockbackDir.Normalize();

        animator.SetFloat(hashHurtDirz, KnockbackDir.x);
        animator.SetFloat(hashHurtDirx, KnockbackDir.z);
        animator.SetTrigger(hashHurt);
    }
}

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("Hurt")]
public class CompleteHurtAnimation : Action
{
    private MonsterView monsterView;
    private NavMeshAgent agent;
    private Animator animator;

    private Vector3 dir;

    int hashHurt = Animator.StringToHash("Hurt");

    [SerializeField] SharedBool isHurt;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        agent = Owner.GetComponent<NavMeshAgent>();
        animator = Owner.GetComponent<Animator>();
    }

    public override void OnStart()
    {
        dir = monsterView.KnockbackDir;
    }

    public override TaskStatus OnUpdate()
    {
        if (monsterView.IsHurtAnimationRunning())
        {
            ApplyKnockBack(dir);
            return TaskStatus.Running;
        }

        isHurt.Value = false;
        animator.SetTrigger(hashHurt);
        return TaskStatus.Success;
    }

    private void ApplyKnockBack(Vector3 KnockbackDir)
    {
        KnockbackDir.y = 0;
        KnockbackDir.Normalize();

        float knockbackForce = 2f;
        agent.Move(knockbackForce * KnockbackDir * Time.deltaTime);
    }
}

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("BossAssassinated")]
public class Assassinated_Conditional : Conditional
{
    private Animator animator;

    [SerializeField] SharedBool isAssassinated;

    private int hashAttack = Animator.StringToHash("Attack");
    private int hashNextAction = Animator.StringToHash("NextAction");
    private int hashJump = Animator.StringToHash("Jump");

    public override void OnAwake()
    {
        animator = Owner.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if(isAssassinated.Value)
        {
            animator.ResetTrigger(hashAttack);
            animator.ResetTrigger(hashNextAction);
            animator.ResetTrigger(hashJump);

            return TaskStatus.Running;
        }
        else return TaskStatus.Failure;
    }
}

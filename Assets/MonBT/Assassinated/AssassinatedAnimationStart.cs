using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Assassinated")]
public class AssassinatedAnimationStart : Action
{
    private MonsterView monsterView;
    private Animator animator;

    private int hashIncapacitated = Animator.StringToHash("Incapacitated");
    private int hashUpper = Animator.StringToHash("Upper");
    private int Assassinated = Animator.StringToHash("Assassinated");

    [SerializeField] SharedBool isAssassinated;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if(!isAssassinated.Value) return TaskStatus.Failure;

        if (!animator.GetBool(hashIncapacitated))
        {
            Debug.Log("암살 당함");
            animator.SetBool(hashIncapacitated, true);
            animator.SetTrigger(Assassinated);
        }

        if(!animator.GetBool(hashUpper) && monsterView.IsAnimationRunning("Assassinated")) return TaskStatus.Success;
        else if (animator.GetBool(hashUpper) && monsterView.IsAnimationRunning("AssassinatedUpper")) return TaskStatus.Success;

        return TaskStatus.Running;
    }
}

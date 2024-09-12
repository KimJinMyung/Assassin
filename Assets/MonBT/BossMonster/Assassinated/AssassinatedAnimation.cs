using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Assassinated")]
public class AssassinatedAnimation : Action
{
    Animator animator;

    [SerializeField] SharedBool isAssassinated;

    private readonly int hashAssassinated = Animator.StringToHash("Assassinated");

    public override void OnAwake()
    {
        animator = Owner.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if(!isAssassinated.Value) return TaskStatus.Failure;

        animator.SetTrigger(hashAssassinated);
        return TaskStatus.Success;
    }
}

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using EventEnum;
using UnityEngine;

[TaskCategory("MonsterDead")]
public class MonsterDeadAnimation : Action
{
    MonsterView monsterView;
    Animator animator;

    [SerializeField] SharedBool isAssassinated;
    [SerializeField] SharedBool isDead;
    [SerializeField] SharedBool isDisable;

    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashDead = Animator.StringToHash("Dead");
    private readonly int hashAssassinated = Animator.StringToHash("Assassinated");
    private readonly int hashIncapaciated = Animator.StringToHash("Incapacitated");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if (isDisable.Value)
        {
            Owner.gameObject.SetActive(false);
            return TaskStatus.Running;
        }       
        if (!isDead.Value) return TaskStatus.Failure;

        if (isAssassinated.Value)
        {
            animator.SetTrigger(hashAssassinated);
        }
        else
        {
            animator.SetBool(hashIncapaciated, false);
            animator.SetBool(hashDead, true);
            animator.SetTrigger(hashDie);
        }

        EventManager<MonsterEvent>.TriggerEvent( MonsterEvent.IsDead, true, monsterView.monsterId);
        return TaskStatus.Success;
    }

}

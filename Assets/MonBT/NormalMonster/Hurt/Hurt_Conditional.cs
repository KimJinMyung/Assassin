using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using EventEnum;
using UnityEngine;

[TaskCategory("Hurt")]
public class Hurt_Conditional : Conditional
{
    private MonsterView monsterView;

    [SerializeField] SharedBool isHurt;
    [SerializeField] SharedBool isDead;
    [SerializeField] SharedBool isAssassinated;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
    }

    public override TaskStatus OnUpdate()
    {
        if(!isHurt.Value || isDead.Value || isAssassinated.Value) return TaskStatus.Failure;
        EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.Attack, monsterView.GetInstanceID(), false);
        return TaskStatus.Running;
    }
}

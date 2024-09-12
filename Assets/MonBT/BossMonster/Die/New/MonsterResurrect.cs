using BehaviorDesigner.Runtime.Tasks;
using EventEnum;

[TaskCategory("MonsterResurrect")]
public class MonsterResurrect : Action
{
    MonsterView monster;

    public override void OnAwake()
    {
        monster = Owner.GetComponent<MonsterView>();
    }

    public override TaskStatus OnUpdate()
    {
        var LifeCount = monster.vm.LifeCount;
        if (LifeCount <= 0) return TaskStatus.Failure;

        LifeCount--;
        EventManager<MonsterUIEvent>.TriggerEvent(MonsterUIEvent.UpdateLifeCount, LifeCount);
        monster.vm.RequestMonsterLifeCountChanged(LifeCount, monster.monsterId);

        monster.Recovery();
        return TaskStatus.Success;
    }
}

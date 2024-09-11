using BehaviorDesigner.Runtime.Tasks;
using EventEnum;

[TaskCategory("BossMonsterDie")]
public class ResurrecteMonster : Action
{
    MonsterView monsterView;

    public override void OnAwake()
    {
        monsterView = GetComponent<MonsterView>();
    }

    public override void OnStart()
    {
        var lifeCount = monsterView.vm.LifeCount - 1;

        monsterView.vm.RequestMonsterLifeCountChanged(lifeCount, monsterView.monsterId);
        monsterView.Recovery();
        EventManager<MonsterUIEvent>.TriggerEvent(MonsterUIEvent.UpdateLifeCount, lifeCount);
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}

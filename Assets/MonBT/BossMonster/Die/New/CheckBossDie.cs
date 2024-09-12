using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("LifeCountCheck")]
public class CheckBossDie : Conditional
{
    MonsterView monsterView;

    [SerializeField] SharedBool isDisable;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
    }

    public override TaskStatus OnUpdate()
    {
        if(monsterView.vm.LifeCount > 0)
            return TaskStatus.Success;
        else
        {
            monsterView.MonsterDead();
            isDisable.Value = true;
            return TaskStatus.Failure;
        }
    }
}
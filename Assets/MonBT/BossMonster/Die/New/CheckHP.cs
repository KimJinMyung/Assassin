using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("CheckMonsterHP")]
public class CheckHP : Conditional
{
    MonsterView monsterView;

    [SerializeField] private SharedBool isDead;
    [SerializeField] private SharedBool isAssassinated;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
    }

    public override TaskStatus OnUpdate()
    {
        if(monsterView.vm.HP <= 0 || isAssassinated.Value)
        {
            isDead.Value = true;
            return TaskStatus.Success;
        }

        isDead.Value = false;
        return TaskStatus.Failure;
    }
}

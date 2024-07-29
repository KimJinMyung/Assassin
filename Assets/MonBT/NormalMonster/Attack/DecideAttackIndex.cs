using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Attack")]
public class DecideAttackIndex : Action
{
    private MonsterView monsterView;

    [SerializeField] SharedFloat AttackIndex;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
    }

    public override void OnStart()
    {
        AttackIndex.Value = Random.Range(0, monsterView.AttackMethodCount);
    }

    public override TaskStatus OnUpdate()
    {
        return TaskStatus.Success;
    }
}

using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("MonsterType")]
public class MonsterType_Conditional : Conditional
{
    private MonsterView MonsterView;

    private bool isBoss;

    public override void OnAwake()
    {
        MonsterView = Owner.GetComponent<MonsterView>();
    }

    public override void OnStart()
    {
        isBoss = MonsterView.Type == MonsterType.Boss;
    }

    public override TaskStatus OnUpdate()
    {
        if(isBoss) return TaskStatus.Success;
        else return TaskStatus.Failure;
    }
}

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[TaskCategory("BossDie")]
public class Die_Conditional : Conditional
{
    private MonsterView MonsterView;

    [SerializeField] SharedBool isDead;

    public override void OnAwake()
    {
        MonsterView = Owner.GetComponent<MonsterView>();
    }

    public override TaskStatus OnUpdate()
    {
        if (isDead.Value)
        {
            return TaskStatus.Running;
        }
        else
        {
            return TaskStatus.Failure;
        }
    }
}

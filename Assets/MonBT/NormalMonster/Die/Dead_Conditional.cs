using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Monster_Die")]
public class Dead_Conditional : Conditional
{
    private MonsterView monsterView;

    [SerializeField] SharedBool isDead;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
    }

    public override TaskStatus OnUpdate()
    {
        if(isDead.Value) return TaskStatus.Running;
        else return TaskStatus.Failure;
    }
}

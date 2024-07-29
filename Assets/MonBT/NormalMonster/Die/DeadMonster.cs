using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Monster_Die")]
public class DeadMonster : Action
{
    public override void OnStart()
    {
        base.OnStart();
        Owner.gameObject.SetActive(false);
    }
}

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Assassinated")]
public class CompleteAssassinatedAnimation : Action
{
    private MonsterView monsterView;

    [SerializeField] SharedBool isDead;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
    }

    public override TaskStatus OnUpdate()
    {
        if (monsterView.IsAnimationRunning("Assassinated") || monsterView.IsAnimationRunning("AssassinatedUpper")) return TaskStatus.Running;

        isDead.Value = true;
        return TaskStatus.Success;
    }
}

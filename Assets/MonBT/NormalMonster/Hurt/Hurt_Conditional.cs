using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Hurt")]
public class Hurt_Conditional : Conditional
{
    private MonsterView monsterView;
    private Monster.AttackBox_Monster _attackBox;

    [SerializeField] SharedBool isHurt;
    [SerializeField] SharedBool isDead;
    [SerializeField] SharedBool isAssassinated;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        _attackBox = monsterView.attackBox;
    }

    public override TaskStatus OnUpdate()
    {
        if(!isHurt.Value || isDead.Value || isAssassinated.Value) return TaskStatus.Failure;
        if(_attackBox != null) _attackBox.enabled = false;
        return TaskStatus.Running;
    }
}

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using EventEnum;
using Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[TaskCategory("Parried")]
public class Parried_Conditional : Conditional
{
    private MonsterView monsterView;
    private Animator animator;
    //private AttackBox_Monster _attackBox;

    [SerializeField] SharedBool isParried;
    [SerializeField] SharedBool isAssassinated;

    private int hashJump = Animator.StringToHash("Jump");
    private int hashAttack = Animator.StringToHash("Attack");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
        //_attackBox = Owner.GetComponentInChildren<AttackBox_Monster>();
    }

    public override void OnStart()
    {
        //animator.ResetTrigger(hashJump);
        //animator.ResetTrigger(hashAttack);
    }

    public override TaskStatus OnUpdate()
    {
        if(!isParried.Value || isAssassinated.Value) return TaskStatus.Failure;

        //_attackBox.enabled = false;
        return TaskStatus.Running;
    }
}

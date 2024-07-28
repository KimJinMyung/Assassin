using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Idle")]
public class Idle_Conditional : Conditional
{
    [SerializeField] SharedBool isHurt;
    [SerializeField] SharedBool isDead;
    [SerializeField] SharedBool isAttacking;
    [SerializeField] SharedBool isAssassinated;


    MonsterView monsterView;
    Animator animator;

    int hashMoveSpeed = Animator.StringToHash("MoveSpeed");
    int hashRotate = Animator.StringToHash("Rotate");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if(monsterView.vm.TraceTarget !=null || isHurt.Value || isDead.Value || isAttacking.Value || isAssassinated.Value) return TaskStatus.Failure;
        else return TaskStatus.Success;
    }

    public override void OnEnd()
    {
        animator.SetBool(hashRotate, false);
        animator.SetFloat(hashMoveSpeed, 0);
    }
}

using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("BossBackJump")]
public class Backjump_Conditional : Conditional
{
    private MonsterView monsterView;
    private Animator animator;

    [SerializeField] SharedBool isAttacking;

    private int AttackTypeIndex;
    private float _distance;

    private int hashAttackTypeIndex = Animator.StringToHash("AttackTypeIndex");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
    }

    public override void OnStart()
    {
        AttackTypeIndex = animator.GetInteger(hashAttackTypeIndex);

        var target = monsterView.vm.TraceTarget;
        if(target == null)
        {
            _distance = Mathf.Infinity;
        }
        else
        {
            _distance = Vector3.Distance(target.transform.position, Owner.transform.position);
        }
    }

    public override TaskStatus OnUpdate()
    {
        if(_distance >= 6) return TaskStatus.Failure;
        if(isAttacking.Value) return TaskStatus.Failure;

        if(AttackTypeIndex == 1) return TaskStatus.Failure;
        else return TaskStatus.Success;
    }
}

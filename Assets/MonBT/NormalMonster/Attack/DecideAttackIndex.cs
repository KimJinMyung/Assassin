using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Attack")]
public class DecideAttackIndex : Action
{
    private MonsterView monsterView;
    private Animator animator;

    [SerializeField] SharedBool isDead;
    [SerializeField] SharedBool isHurt;

    [SerializeField] SharedFloat AttackIndex;

    private int hashAttackIndex = Animator.StringToHash("AttackIndex");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = monsterView.GetComponent<Animator>();
    }

    public override void OnStart()
    {
        AttackIndex.Value = Random.Range(0, monsterView.AttackMethodCount);
        animator.SetInteger(hashAttackIndex, (int)AttackIndex.Value);
    }

    public override TaskStatus OnUpdate()
    {
        if(isDead.Value || isHurt.Value) return TaskStatus.Failure;
        return TaskStatus.Success;
    }
}

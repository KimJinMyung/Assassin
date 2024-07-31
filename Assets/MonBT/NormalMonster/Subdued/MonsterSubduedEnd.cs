using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Subdued")]
public class MonsterSubduedEnd : Action
{
    private MonsterView monsterView;
    private Animator animator;

    [SerializeField] SharedBool isSubded;
    [SerializeField] SharedBool isAttackAble;

    private int hashIncapacitated = Animator.StringToHash("Incapacitated");

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
        animator = Owner.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        monsterView.vm.RequestMonsterStaminaChanged(monsterView._monsterData.MaxStamina, monsterView.monsterId);

        animator.SetBool(hashIncapacitated, false);
        isSubded.Value = false;
        if (animator.layerCount > 1) animator.SetLayerWeight(1, 1);
        return TaskStatus.Success;
    }

    public override void OnEnd()
    {
        isAttackAble.Value = true;
    }
}

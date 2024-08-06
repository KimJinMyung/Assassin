using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("BossAssassinated")]
public class CompleteAssassinatedAnimation : Action
{
    [SerializeField] SharedBool isAssassinated;

    private MonsterView monsterView;
    private Animator animator;

    public override void OnAwake()
    {
        animator = Owner.GetComponent<Animator>();
        monsterView = Owner.GetComponent<MonsterView>();
    }
}

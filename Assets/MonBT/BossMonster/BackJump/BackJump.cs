using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("BossMonsterBackJump")]
public class BackJump : Action
{
    private MonsterView monsterView;

    [SerializeField] private AnimationCurve JumpPath;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
    }

    public override void OnStart()
    {

    }


}

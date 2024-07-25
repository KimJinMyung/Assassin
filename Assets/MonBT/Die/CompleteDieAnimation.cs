using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Monster_Die")]
public class CompleteDieAnimation : Action
{
    private MonsterView monsterView;
    private Animator _animator;

    public override void OnAwake()
    {
        base.OnAwake();
        monsterView = Owner.GetComponent<MonsterView>();
        _animator = monsterView.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        if (!monsterView.isDead) return TaskStatus.Failure;

        if (IsAnimationRunning("Die"))
        {
            return TaskStatus.Running;
        }

        return TaskStatus.Success;
    }

    private bool IsAnimationRunning(string animationName)
    {
        if (_animator == null) return false;

        bool isRunning = false;
        var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(animationName))
        {
            float normalizedTime = stateInfo.normalizedTime;
            isRunning = normalizedTime >= 0 && normalizedTime < 1.0f;
        }

        return isRunning;
    }
}

using EventEnum;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BossComboAttack : StateMachineBehaviour
{
    private MonsterView monsterView;

    [Serializable]
    public class AttackBoxTime
    {
        public float OnTime;
        public float OffTime;
    }

    [SerializeField] List<AttackBoxTime> attackBoxTimes;
    [SerializeField] float EndTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        monsterView = animator.GetComponent<MonsterView>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float normalizedTime = Mathf.Clamp(stateInfo.normalizedTime, 0f, 1f);

        if (normalizedTime <= EndTime) Rotation();

        bool shouldEnableAttackBox = false;

        foreach (var attackBoxTime in attackBoxTimes)
        {
            if(normalizedTime >= attackBoxTime.OnTime && normalizedTime <= attackBoxTime.OffTime)
            {
                shouldEnableAttackBox = true;
                break;
            }
        }

        EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.Attack, monsterView.monsterId, shouldEnableAttackBox);
    }

    private void Rotation()
    {
        Transform ownerTransform = monsterView.transform;
        Vector3 dir = monsterView.vm.TraceTarget.position - ownerTransform.position;
        dir.y = 0f;
        Quaternion rotation = Quaternion.LookRotation(dir.normalized);
        ownerTransform.rotation = Quaternion.Slerp(ownerTransform.rotation, rotation, 3f * Time.deltaTime);
    }
}

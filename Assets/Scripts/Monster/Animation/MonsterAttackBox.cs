using BehaviorDesigner.Runtime;
using EventEnum;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class MonsterAttackBox : StateMachineBehaviour
{
    private MonsterView MonsterView;
    private BehaviorTree tree;

    private Monster_Attack monsterAttack;

    private Transform throwShurikenPoint;

    [Header("Throw Weapons")]
    [SerializeField] GameObject shuriken;

    [SerializeField] private float AttackBoxOnTime;
    [SerializeField] private float AttackBoxOffTime;

    private bool isAction;
    protected readonly int hashAttackIndex = Animator.StringToHash("AttackIndex");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        MonsterView = animator.GetComponent<MonsterView>();
        tree = MonsterView._behaviorTree;

        monsterAttack = MonsterView.vm.CurrentAttackMethod;

        throwShurikenPoint = FindChildWithTag(MonsterView.transform, "ThrowPoint");

        isAction = false;
    }

    private Transform FindChildWithTag(Transform parent, string tagName)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tagName))
            {
                return child;
            }

            // 재귀적으로 자식의 자식들도 탐색합니다.
            Transform result = FindChildWithTag(child, tagName);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float normalizeTime = Mathf.Clamp(stateInfo.normalizedTime, 0f, 1f);

        if (monsterAttack.DataName == Enum.GetName(typeof(WeaponsType), WeaponsType.ShurikenAttack) && !isAction)
        {
            if ((animator.GetInteger(hashAttackIndex) == 0 && normalizeTime >= 0.34f) || (animator.GetInteger(hashAttackIndex) == 1 && normalizeTime >= 0.297f))
            {
                isAction = true;
                Debug.Log("수리검 던짐");
                Quaternion CreateDir = Quaternion.LookRotation(MonsterView.vm.TraceTarget.position + Vector3.up - throwShurikenPoint.position);
                Shuriken shootShuriken = Instantiate(shuriken, throwShurikenPoint.position, CreateDir).GetComponent<Shuriken>();
                shootShuriken.SetShooterData(MonsterView);
            }
        }
        else
        {            
            if (normalizeTime >= AttackBoxOnTime && normalizeTime <= AttackBoxOffTime)
            {
                EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.Attack, MonsterView.GetInstanceID(), true);
                Debug.Log("몬스터의 공격");
            }
            else EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.Attack, MonsterView.GetInstanceID(), false);
        }        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!(bool)tree.GetVariable("isAttacking").GetValue())
            EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.Attack, MonsterView.GetInstanceID(), false);
    }
}

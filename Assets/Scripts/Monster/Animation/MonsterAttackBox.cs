using BehaviorDesigner.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class MonsterAttackBox : StateMachineBehaviour
{
    private MonsterView monsterVIew;
    private BehaviorTree tree;
    private AttackBox attackBox;

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
        monsterVIew = animator.GetComponent<MonsterView>();
        tree = monsterVIew._behaviorTree;
        attackBox = monsterVIew.attackBox;

        monsterAttack = monsterVIew.vm.CurrentAttackMethod;

        throwShurikenPoint = FindChildWithTag(monsterVIew.transform, "ThrowPoint");

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
                Quaternion CreateDir = Quaternion.LookRotation(monsterVIew.vm.TraceTarget.position + Vector3.up - throwShurikenPoint.position);
                Shuriken shootShuriken = Instantiate(shuriken, throwShurikenPoint.position, CreateDir).GetComponent<Shuriken>();
                shootShuriken.SetShooterData(monsterVIew);
            }
        }
        else
        {            
            if (normalizeTime >= AttackBoxOnTime && normalizeTime <= AttackBoxOffTime)
            {
                attackBox.enabled = true;
            }
            else attackBox.enabled = false;
        }        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(!(bool)tree.GetVariable("isAttacking").GetValue())
            attackBox.enabled = false;
    }
}

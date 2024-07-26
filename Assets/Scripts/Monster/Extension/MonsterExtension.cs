using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public static class MonsterExtension
{
    #region HP
    public static void RegisterMonsterHPChanged(this MonsterViewModel vm, bool isRegister, int monsterId)
    {
        LogicManager.instance.RegisterMonsterHPChangedCallback(monsterId, vm.OnResponseMonsterHPChangedEvent, isRegister);
    }
    public static void RequestMonsterHPChanged(this MonsterViewModel vm, int monsterId, float hp)
    {
        LogicManager.instance.OnMonsterHPChanged(monsterId, hp);
    }
    public static void OnResponseMonsterHPChangedEvent(this MonsterViewModel vm, float HP)
    {
        vm.HP = HP;

    }
    #endregion
    #region Stamina
    public static void RegisterMonsterStaminaChanged(this MonsterViewModel vm, bool isRegister, int monsterId)
    {
        LogicManager.instance.RegisterMonsterStaminaChangedCallback(monsterId, vm.OnResponseMonsterStaminaChangedEvent, isRegister);
    }
    public static void RequestMonsterStaminaChanged(this MonsterViewModel vm, float stamina, int monsterId)
    {
        LogicManager.instance.OnMonsterStaminaChanged(monsterId, stamina);
    }
    public static void OnResponseMonsterStaminaChangedEvent(this MonsterViewModel vm, float stamina)
    {
        vm.Stamina = stamina;

    }
    #endregion
    #region ChangedAttackMethod
    public static void RegisterAttackMethodChanged(this MonsterViewModel vm, int actorId, bool isRegister)
    {
        LogicManager.instance.RegisterAttackMethodChangedCallback(actorId, vm.OnResponseAttackMethodChangedEvent, isRegister);
    }
    public static void RequestAttackMethodChanged(this MonsterViewModel vm, int actorId, List<Monster_Attack> attackList, MonsterView owner)
    {
        LogicManager.instance.OnAttackMethodChanged(actorId, attackList, owner);
    }

    public static void OnResponseAttackMethodChangedEvent(this MonsterViewModel vm, List<Monster_Attack> attackList, MonsterView owner)
    {
        if (vm.TraceTarget == null || vm.CurrentAttackMethod == null)
        {
            vm.CurrentAttackMethod = attackList.Last();
        }
        else
        {
            float distance = Vector3.Distance(vm.TraceTarget.position, owner.transform.position);

            Monster_Attack closestAttackMethod = null;
            float closestDistanceDiff = float.MaxValue;

            foreach (var attackMethod in attackList)
            {
                float distanceDiff = Math.Abs(attackMethod.AttackRange - distance);

                // ��Ÿ� ���� �ְ�, �� ����� ��Ÿ��� ���� ���⸦ ����
                if (distance <= (attackMethod.AttackRange + 2.5f) && distanceDiff < closestDistanceDiff)
                {
                    closestAttackMethod = attackMethod;
                    closestDistanceDiff = distanceDiff;
                }
            }

            // ��Ÿ� ���� �ִ� ���� ����� ��Ÿ��� ���� ������� ����
            // ��Ÿ� ���� ������ ���� ����� ������ ���� �� ��Ÿ��� ���� ����
            vm.CurrentAttackMethod = closestAttackMethod ?? attackList.Last();
        }
    }
    #endregion
    #region TraceTarget
    public static void RegisterTraceTargetChanged(this MonsterViewModel vm, int actorId, bool isRegister)
    {
        LogicManager.instance.RegisterTraceTargetChangedCallback(vm.OnResponseTraceTargetChangedEvent, actorId, isRegister);
    }
    public static void RequestTraceTargetChanged(this MonsterViewModel vm, int actorId, Transform traceTarget)
    {
        LogicManager.instance.OnTraceTarget(actorId, traceTarget);
    }
    public static void OnResponseTraceTargetChangedEvent(this MonsterViewModel vm, Transform traceTarget)
    {
        vm.TraceTarget = traceTarget;
    }
    #endregion
}

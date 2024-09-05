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
        //LogicManager.Instance.RegisterMonsterHPChangedCallback(monsterId, vm.OnResponseMonsterHPChangedEvent, isRegister);
        MonsterManager.Instance.RegisterMonsterHPChangedCallback(monsterId, vm.OnResponseMonsterHPChangedEvent, isRegister);
    }
    public static void RequestMonsterHPChanged(this MonsterViewModel vm, int monsterId, float hp)
    {
        //LogicManager.Instance.OnMonsterHPChanged(monsterId, hp);
        MonsterManager.Instance.SetMonsterHP(monsterId, hp);
    }
    public static void OnResponseMonsterHPChangedEvent(this MonsterViewModel vm, float HP)
    {
        vm.HP = HP;
    }
    #endregion
    #region Stamina
    public static void RegisterMonsterStaminaChanged(this MonsterViewModel vm, bool isRegister, int monsterId)
    {
        //LogicManager.Instance.RegisterMonsterStaminaChangedCallback(monsterId, vm.OnResponseMonsterStaminaChangedEvent, isRegister);
        MonsterManager.Instance.RegisterMonsterStaminaChangedCallback(monsterId, vm.OnResponseMonsterStaminaChangedEvent, isRegister);
    }
    public static void RequestMonsterStaminaChanged(this MonsterViewModel vm, float stamina, int monsterId)
    {
        //LogicManager.Instance.OnMonsterStaminaChanged(monsterId, stamina);
        MonsterManager.Instance.SetMonsterStamina(monsterId, stamina);
    }
    public static void OnResponseMonsterStaminaChangedEvent(this MonsterViewModel vm, float stamina)
    {
        vm.Stamina = stamina;
    }
    #endregion
    #region LifeCount
    public static void RegisterMonsterLifeCountChanged(this MonsterViewModel vm, bool isRegister, int monsterId)
    {
        //LogicManager.Instance.RegisterMonsterLifeCountChangedCallback(monsterId, vm.OnResponseMonsterLifeCountChangedEvent, isRegister);
        MonsterManager.Instance.RegisterMonsterLifeCountChangedCallback(monsterId, vm.OnResponseMonsterLifeCountChangedEvent, isRegister);
    }
    public static void RequestMonsterLifeCountChanged(this MonsterViewModel vm, float LifeCount, int monsterId)
    {
        //LogicManager.Instance.OnMonsterLifeCountChanged(monsterId, LifeCount);
        MonsterManager.Instance.SetMonsterLifeCount(monsterId, LifeCount);
    }
    public static void OnResponseMonsterLifeCountChangedEvent(this MonsterViewModel vm, float lifeCount)
    {
        vm.LifeCount = lifeCount;
    }
    #endregion
    #region ChangedAttackMethod
    public static void RegisterAttackMethodChanged(this MonsterViewModel vm, int actorId, bool isRegister)
    {
        MonsterManager.Instance.RegisterAttackMethodChangedCallback(actorId, vm.OnResponseAttackMethodChangedEvent, isRegister);
    }
    public static void RequestAttackMethodChanged(this MonsterViewModel vm, int actorId, List<Monster_Attack> attackList, MonsterView owner)
    {
        MonsterManager.Instance.OnAttackMethodChanged(actorId, attackList, owner);
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

                // 사거리 내에 있고, 더 가까운 사거리를 가진 무기를 선택
                if (distance <= (attackMethod.AttackRange + 2.5f) && distanceDiff < closestDistanceDiff)
                {
                    closestAttackMethod = attackMethod;
                    closestDistanceDiff = distanceDiff;
                }
            }

            // 사거리 내에 있는 가장 가까운 사거리의 공격 방식으로 설정
            // 사거리 내에 적절한 공격 방식이 없으면 가장 긴 사거리의 무기 선택
            vm.CurrentAttackMethod = closestAttackMethod ?? attackList.Last();
        }
    }
    #endregion
    #region TraceTarget
    public static void RegisterTraceTargetChanged(this MonsterViewModel vm, int actorId, bool isRegister)
    {
        MonsterManager.Instance.RegisterTraceTargetChangedCallback(vm.OnResponseTraceTargetChangedEvent, actorId, isRegister);
    }
    public static void RequestTraceTargetChanged(this MonsterViewModel vm, int actorId, Transform traceTarget)
    {
        MonsterManager.Instance.OnTraceTarget(actorId, traceTarget);
    }
    public static void OnResponseTraceTargetChangedEvent(this MonsterViewModel vm, Transform traceTarget)
    {
        vm.TraceTarget = traceTarget;
    }
    #endregion
}

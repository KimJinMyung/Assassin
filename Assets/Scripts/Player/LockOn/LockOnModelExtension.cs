using System.Collections.Generic;
using UnityEngine;
using Player;
using EventEnum;
using System;

public static class LockOnModelExtension
{
    private static int RopePointLayer = LayerMask.NameToLayer("RopePoint");
    private static int MonsterLayer = LayerMask.NameToLayer("Monster");
    private static int LockOnTargetLayer = LayerMask.NameToLayer("LockOnTarget");
    private static int LockOnAbleLayer = LayerMask.NameToLayer("LockOnAble");
    private static int DeadLayer = LayerMask.NameToLayer("Dead");

    #region LockOnTargetList
    public static void RegisterLockOnTargetListChanged(this LockOnViewModel model, bool isRegister)
    {
        LockOnManager.Instance.RegisterLockOnTargetListChangedCallback(model.OnResponseLockOnTargetListChangedEvent, isRegister);
    }

    public static void RequestLockOnTargetList(this LockOnViewModel model, List<Transform> targetLists)
    {
        LockOnManager.Instance.OnLockOnTargetList(targetLists);
    }

    public static void OnResponseLockOnTargetListChangedEvent(this LockOnViewModel model, List<Transform> tartgetlists)
    {
        List<Transform> newColliders = new List<Transform>();

        foreach (var c in tartgetlists)
        {
            if (!model.HitColliders.Contains(c))
            {
                newColliders.Add(c);
            }
        }

        foreach (var c in model.HitColliders)
        {
            if (tartgetlists.Contains(c))
            {
                newColliders.Add(c);
            }
        }

        foreach (var c in model.HitColliders)
        {
            if (c.CompareTag("RopePoint")) continue;
            if (!newColliders.Contains(c))
            {
                c.gameObject.layer = MonsterLayer;
            }

            MonsterView monster = c.GetComponent<MonsterView>();
            if (monster == null) continue;
        }

        model.HitColliders = newColliders;
        if(newColliders.Count > 0)
        {
            //EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.ChangedLockOnAbleMonsterList, newColliders);
            MonsterManager.Instance.LockOnAbleMonsterListChanged(newColliders);
        }
    }
    #endregion

    #region LockOnAbleTarget
    public static void RegisterLockOnAbleTargetChanged(this LockOnViewModel model, bool isRegister)
    {
        LockOnManager.Instance.RegisterLockOnAbleTargetChangedCallback(model.OnResponseLockOnAbleTargetChangedEvent, isRegister);
    }

    public static void RequestLockOnAbleTarget(this LockOnViewModel model, Transform target)
    {
        LockOnManager.Instance.OnLockOnAbleTarget(target);
    }

    public static void OnResponseLockOnAbleTargetChangedEvent(this LockOnViewModel model, Transform target)
    {
        //만약 지정한 타겟이 기존의 것과 동일하면 return
        if (target == model.LockOnAbleTarget) return;

        //만약 지정한 타겟이 LockOnTarget이면 model.LockOnAbleTarget 으로만 지정
        //그렇지 않다면 지정한 타겟의 레이어를 LockOnAble로 변경.
        //기존의 타겟은 Monster로 변경
        if (target != null)
        {
            if (target.gameObject.layer != LockOnTargetLayer)
            {
                target.gameObject.layer = LockOnAbleLayer;

                if (model.LockOnAbleTarget != null)
                {
                    if (model.LockOnAbleTarget.gameObject.layer != LockOnTargetLayer)
                    {
                        if (model.LockOnAbleTarget.CompareTag("RopePoint"))
                            model.LockOnAbleTarget.gameObject.layer = RopePointLayer;
                        else
                            model.LockOnAbleTarget.gameObject.layer = MonsterLayer;
                    }
                }
            }
            else
            {
                if (model.LockOnAbleTarget.CompareTag("RopePoint"))
                    model.LockOnAbleTarget.gameObject.layer = RopePointLayer;
                else
                    model.LockOnAbleTarget.gameObject.layer = MonsterLayer;
            }
        }
        else
        {
            if (model.LockOnAbleTarget != null)
            {
                if (model.LockOnAbleTarget.gameObject.layer == LockOnTargetLayer) return;
                if (model.LockOnAbleTarget.CompareTag("RopePoint"))
                    model.LockOnAbleTarget.gameObject.layer = RopePointLayer;
                else
                    model.LockOnAbleTarget.gameObject.layer = MonsterLayer;
            }
        }

        model.LockOnAbleTarget = target;
    }
    #endregion

    #region LockOnTarget
    public static void RegisterLockOnViewModel_TargetChanged(this LockOnViewModel model, bool isRegister)
    {
        LockOnManager.Instance.RegisterLockOnViewModel_TargetChangedCallback(model.OnResponseLockOnTargetChangedEvent, isRegister);
        
    }

    public static void RequestLockOnViewModel_Target(this LockOnViewModel model, Transform target, PlayerView player)
    {
        LockOnManager.Instance.OnLockOnTarget_LockOnViewModel(target, player);
    }

    public static void OnResponseLockOnTargetChangedEvent(this LockOnViewModel model, Transform target, PlayerView player)
    {
        if (target == model.LockOnTarget) return;

        if (model.LockOnTarget != null)
        {
            if (model.LockOnTarget == model.LockOnAbleTarget) model.LockOnTarget.gameObject.layer = LockOnAbleLayer;
            else if (model.LockOnTarget.CompareTag("RopePoint")) model.LockOnTarget.gameObject.layer = RopePointLayer;
            else model.LockOnTarget.gameObject.layer = MonsterLayer;
        }

        if (target != null)
            target.gameObject.layer = LockOnTargetLayer;

        player.ViewModel.RequestLockOnTarget(target);

        model.LockOnTarget = target;
    }
    #endregion
}

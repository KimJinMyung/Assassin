using System.Collections.Generic;
using UnityEngine;
using Player;
using EventEnum;

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
        //LogicManager.Instance.RegisterLockOnTargetListChangedCallback(model.OnResponseLockOnTargetListChangedEvent, isRegister);
        EventManager<LockOnEvent>.Binding<List<Transform>>(isRegister, LockOnEvent.UpdateLockOnTargetList, model.OnResponseLockOnTargetListChangedEvent);
    }

    public static void RequestLockOnTargetList(this LockOnViewModel model, List<Transform> targetLists)
    {
        //LogicManager.Instance.OnLockOnTargetList(targetLists);
        var targetList = new List<Transform>(targetLists);
        EventManager<LockOnEvent>.TriggerEvent(LockOnEvent.UpdateLockOnTargetList, targetList);
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
        if(newColliders != null)
            MonsterManager.instance.LockOnAbleMonsterListChanged(newColliders);
    }
    #endregion

    #region LockOnAbleTarget
    public static void RegisterLockOnAbleTargetChanged(this LockOnViewModel model, bool isRegister)
    {
        //LogicManager.Instance.RegisterLockOnAbleTargetChangedCallback(model.OnResponseLockOnAbleTargetChangedEvent, isRegister);
        EventManager<LockOnEvent>.Binding<Transform>(isRegister, LockOnEvent.UpdateLockOnAbleTarget, model.OnResponseLockOnAbleTargetChangedEvent);
    }

    public static void RequestLockOnAbleTarget(this LockOnViewModel model, Transform target)
    {
        //LogicManager.Instance.OnLockOnAbleTarget(target);
        EventManager<LockOnEvent>.TriggerEvent(LockOnEvent.UpdateLockOnAbleTarget, target);
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
        //LogicManager.Instance.RegisterLockOnViewModel_TargetChangedCallback(model.OnResponseLockOnTargetChangedEvent, isRegister);
        EventManager<LockOnEvent>.Binding<Transform, PlayerView>(isRegister,LockOnEvent.UpdateLockOnTarget, model.OnResponseLockOnTargetChangedEvent);
    }

    public static void RequestLockOnViewModel_Target(this LockOnViewModel model, Transform target, PlayerView player)
    {
        //LogicManager.Instance.OnLockOnTarget_LockOnViewModel(target, player);
        EventManager<LockOnEvent>.TriggerEvent(LockOnEvent.UpdateLockOnTarget, target, player);
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

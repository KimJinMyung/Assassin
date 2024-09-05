using Player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnManager : Singleton<LockOnManager>
{
    #region LockOnTargetList
    private Action<List<Transform>> _lockOnTargetListChangedCallback;
    public void RegisterLockOnTargetListChangedCallback(Action<List<Transform>> lockOnTargetListChangedCallback, bool isRegister)
    {
        if (isRegister) _lockOnTargetListChangedCallback += lockOnTargetListChangedCallback;
        else _lockOnTargetListChangedCallback -= lockOnTargetListChangedCallback;
    }
    public void OnLockOnTargetList(List<Transform> lockOnTargetList)
    {
        if (_lockOnTargetListChangedCallback == null) return;
        _lockOnTargetListChangedCallback.Invoke(lockOnTargetList);
    }
    #endregion
    #region LockOnAbleTargetList
    private Action<Transform> _lockOnAbleTargetChangedCallback;
    public void RegisterLockOnAbleTargetChangedCallback(Action<Transform> lockOnAbleTargetChangedCallback, bool isRegister)
    {
        if (isRegister) _lockOnAbleTargetChangedCallback += lockOnAbleTargetChangedCallback;
        else _lockOnAbleTargetChangedCallback -= lockOnAbleTargetChangedCallback;
    }
    public void OnLockOnAbleTarget(Transform target)
    {
        if (_lockOnAbleTargetChangedCallback == null) return;
        _lockOnAbleTargetChangedCallback.Invoke(target);
    }
    #endregion
    #region LockOnTarget
    private Action<Transform, PlayerView> _lockOnViewModel_TargetChangedCallback;
    public void RegisterLockOnViewModel_TargetChangedCallback(Action<Transform, PlayerView> lockOnTargetChangedCallback, bool isRegister)
    {
        if (isRegister) _lockOnViewModel_TargetChangedCallback += lockOnTargetChangedCallback;
        else _lockOnViewModel_TargetChangedCallback -= lockOnTargetChangedCallback;
    }
    public void OnLockOnTarget_LockOnViewModel(Transform target, PlayerView player)
    {
        if (_lockOnViewModel_TargetChangedCallback == null) return;
        _lockOnViewModel_TargetChangedCallback.Invoke(target, player);
    }
    #endregion
}

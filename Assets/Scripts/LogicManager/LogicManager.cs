using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LogicManager : MonoBehaviour
{
    public static LogicManager instance;

    private void Awake()
    {
        if(instance == null) instance = this;
        else if(instance != this) Destroy(instance);

        DontDestroyOnLoad(this.gameObject);
    }

    #region PlayerHP
    private Action<float> _playerHPChangedCallback;

    public void RegisterPlayerHPChangedCallback(Action<float> playerHPChangedCallback, bool isRegister)
    {
        if(isRegister) _playerHPChangedCallback += playerHPChangedCallback;
        else _playerHPChangedCallback -= playerHPChangedCallback;
    }

    public void OnPlayerHPChanged(float playerHP)
    {
        if (_playerHPChangedCallback == null) return;
        _playerHPChangedCallback.Invoke(playerHP);
    }
    #endregion
    #region PlayerStamina
    private Action<float> _playerStaminaChangedCallback;

    public void RegisterPlayerStaminaChangedCallback(Action<float> playerStaminaChangedCallback, bool isRegister)
    {
        if (isRegister) _playerStaminaChangedCallback += playerStaminaChangedCallback;
        else _playerStaminaChangedCallback -= playerStaminaChangedCallback;
    }

    public void OnPlayerStaminaChanged(float playerHP)
    {
        if (_playerStaminaChangedCallback == null) return;
        _playerStaminaChangedCallback.Invoke(playerHP);
    }
    #endregion
    #region Move
    private Action<float, float> _moveVelocityChangedCallback;

    public void RegisterMoveVelocityChangedCallback(Action<float, float> moveVelocityChangedCallback, bool isRegister)
    {
        if (isRegister)
        {
            _moveVelocityChangedCallback += moveVelocityChangedCallback;
        }
        else
        {
            _moveVelocityChangedCallback -= moveVelocityChangedCallback;
        }
    }

    public void OnMoveInput(float x, float y)
    {
        if (_moveVelocityChangedCallback == null) return;
        _moveVelocityChangedCallback.Invoke(x, y);
    }
    #endregion
    #region Rotate
    private Action<float, float, float> _targetAngleChangedCallback;

    public void RegisterActorRotateChangedCallback(Action<float, float, float> targetAngleChangedCallback, bool isRegister)
    {
        if (isRegister)
        {
            _targetAngleChangedCallback += targetAngleChangedCallback;
        }
        else
        {
            _targetAngleChangedCallback -= targetAngleChangedCallback;
        }
    }

    public void OnActorRotate(float x, float y, float z)
    {
        if (_targetAngleChangedCallback == null) return;
        _targetAngleChangedCallback.Invoke(x, y, z);
    }
    #endregion
    #region PlayerViewModelLockOnTargetChanged
    private Action<Transform> _lockOnModelPlayer_TargetChangeCallback;
    public void RegisterLockOnTargetChangedCallback(Action<Transform> lockOnTargetChangedCallback, bool isRegister)
    {
        if (isRegister) _lockOnModelPlayer_TargetChangeCallback += lockOnTargetChangedCallback;
        else _lockOnModelPlayer_TargetChangeCallback -= lockOnTargetChangedCallback;
    }

    public void OnLockOnTarget(Transform target)
    {
        if (_lockOnModelPlayer_TargetChangeCallback == null) return;
        _lockOnModelPlayer_TargetChangeCallback.Invoke(target);
    }
    #endregion

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

    #region MonsterHP
    private Dictionary<int, Action<float>> _monsterHP = new Dictionary<int, Action<float>>();

    public void RegisterMonsterHPChangedCallback(int monsterId, Action<float> monsterHP, bool isRegister)
    {
        if(isRegister)
        {
            if(!_monsterHP.ContainsKey(monsterId)) _monsterHP.Add(monsterId, monsterHP);
            else _monsterHP[monsterId] = monsterHP;
        }else
        {
            if(_monsterHP.ContainsKey(monsterId))
            {
                _monsterHP[monsterId] -= monsterHP;
                if (_monsterHP[monsterId] == null) _monsterHP.Remove(monsterId);
            }

        }
    }
    public void OnMonsterHPChanged(int monsterId, float HP)
    {
        if (_monsterHP.ContainsKey(monsterId)) _monsterHP[monsterId]?.Invoke(HP);
    }
    #endregion
    #region MonsterStamina
    private Dictionary<int, Action<float>> _monsterStamina = new Dictionary<int, Action<float>>();

    public void RegisterMonsterStaminaChangedCallback(int monsterId, Action<float> monsterHP, bool isRegister)
    {
        if (isRegister)
        {
            if (!_monsterStamina.ContainsKey(monsterId)) _monsterStamina.Add(monsterId, monsterHP);
            else _monsterStamina[monsterId] = monsterHP;
        }
        else
        {
            if (_monsterStamina.ContainsKey(monsterId))
            {
                _monsterStamina[monsterId] -= monsterHP;
                if (_monsterStamina[monsterId] == null) _monsterStamina.Remove(monsterId);
            }

        }
    }
    public void OnMonsterStaminaChanged(int monsterId, float HP)
    {
        if (_monsterStamina.ContainsKey(monsterId)) _monsterStamina[monsterId]?.Invoke(HP);
    }
    #endregion
    #region MonsterAttackMethod
    private Dictionary<int, Action<List<Monster_Attack>, MonsterView>> _monsterAttackMethodList = new Dictionary<int, Action<List<Monster_Attack>, MonsterView>>();

    public void RegisterAttackMethodChangedCallback(int monsterId, Action<List<Monster_Attack>, MonsterView> AttackMethodChangedCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (!_monsterAttackMethodList.ContainsKey(monsterId)) _monsterAttackMethodList.Add(monsterId, AttackMethodChangedCallback);
            else _monsterAttackMethodList[monsterId] = AttackMethodChangedCallback;
        }else
        {
            if (_monsterAttackMethodList.ContainsKey(monsterId))
            {
                _monsterAttackMethodList[monsterId] -= AttackMethodChangedCallback;
                if (_monsterAttackMethodList[monsterId] == null) _monsterAttackMethodList.Remove(monsterId);
            }
        }
    }

    public void OnAttackMethodChanged(int actorId, List<Monster_Attack> attackList, MonsterView owner)
    {
        if (_monsterAttackMethodList.ContainsKey(actorId)) _monsterAttackMethodList[actorId]?.Invoke(attackList, owner);
    }
    #endregion
}

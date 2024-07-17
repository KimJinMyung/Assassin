using System;
using System.Collections;
using System.Collections.Generic;
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
}

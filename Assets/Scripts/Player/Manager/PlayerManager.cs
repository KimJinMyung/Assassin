using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    Action<float> HPChangedEvnetHandler;
    Action<float> MaxHPChangedEventHandler;
    Action<float> StaminaUpdateEvnetHandler;
    Action<float> MaxStaminaChangedEventHandler;
    Action<float> LifeCountUpdateEventHandler;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this.gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void BindHPChanged(Action<float> HPChanged, bool isBind)
    {
        if (isBind) HPChangedEvnetHandler += HPChanged;
        else HPChangedEvnetHandler -= HPChanged;
    }
    public void BindMaxHPChanged(Action<float> maxHPChanged, bool isBind)
    {
        if (isBind) MaxHPChangedEventHandler += maxHPChanged;
        else MaxHPChangedEventHandler -= maxHPChanged;
    }

    public void BindStaminaChanged(Action<float> StaminaChanged, bool isBind)
    {
        if (isBind) StaminaUpdateEvnetHandler += StaminaChanged;
        else StaminaUpdateEvnetHandler -= StaminaChanged;
    }
    public void BindMaxStaminaChanged(Action<float> MaxStaminaChanged, bool isBind)
    {
        if (isBind) MaxStaminaChangedEventHandler += MaxStaminaChanged;
        else MaxStaminaChangedEventHandler -= MaxStaminaChanged;
    }
    public void BindLifeCountChanged(Action<float> LifeCountChanged, bool isBind)
    {
        if (isBind) LifeCountUpdateEventHandler += LifeCountChanged;
        else LifeCountUpdateEventHandler -= LifeCountChanged;
    }

    public void SetPlayerHP(float HP)
    {
        HPChangedEvnetHandler?.Invoke(HP);
    }

    public void SetPlayerMaxHP(float  MaxHP)
    {
        MaxHPChangedEventHandler?.Invoke(MaxHP);
    }

    public void SetStamina(float Stamina)
    {
        StaminaUpdateEvnetHandler?.Invoke(Stamina);
    }

    public void SetMaxStamina(float MaxStamina)
    {
        MaxStaminaChangedEventHandler?.Invoke((float)MaxStamina);
    }

    public void SetLifeCount(float LifeCount)
    {
        LifeCountUpdateEventHandler?.Invoke((float)LifeCount);
    }
}

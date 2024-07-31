using System.ComponentModel;
using UnityEngine;

public class MonsterViewModel
{
    private float _hp;
    public float HP
    {
        get { return _hp; }
        set
        {
            if (_hp == value) return;

            _hp = value;
            OnPropertyChanged(nameof(HP));
        }
    }

    private float _stamina;
    public float Stamina
    {
        get { return _stamina; }
        set
        {
            if (_stamina == value) return;
            _stamina = value;
            OnPropertyChanged(nameof(Stamina));
        }
    }

    private float _lifeCount;
    public float LifeCount
    {
        get { return _lifeCount; }
        set
        {
            if (_lifeCount == value) return;
            _lifeCount = value;
            OnPropertyChanged(nameof(LifeCount));
        }
    }

    private Transform _traceTarget;
    public Transform TraceTarget
    {
        get => _traceTarget;
        set
        {
            if (_traceTarget == value) return;

            _traceTarget = value;
            OnPropertyChanged(nameof(TraceTarget));
        }
    }

    private Monster_Attack _currentAttackMethod;
    public Monster_Attack CurrentAttackMethod
    {
        get { return _currentAttackMethod; }
        set
        {
            if (value == _currentAttackMethod) return;
            _currentAttackMethod = value;
            OnPropertyChanged(nameof(CurrentAttackMethod));
        }
    }

    #region propertyEvent
    public event PropertyChangedEventHandler PropertyChanged;

    public virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}

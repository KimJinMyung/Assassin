using System.ComponentModel;
using UnityEngine;

public class PlayerViewModel
{
    private float _hp;
    public float HP
    {
        get { return _hp; }
        set
        {
            if(_hp == value) return;

            _hp = value;
            OnPropertyChanged(nameof(HP));
        }
    }

    private float _maxHp;
    public float MaxHP
    {
        get { return _maxHp; }
        set
        {
            if (_maxHp == value) return;

            _maxHp = value;
            OnPropertyChanged(nameof(MaxHP));
        }
    }

    private float _stamina;
    public float Stamina
    {
        get { return _stamina; }
        set
        {
            if(_stamina == value) return;
            _stamina = value;
            OnPropertyChanged(nameof(Stamina));
        }
    }

    private float _maxStamina;
    public float MaxStamina
    {
        get { return _maxStamina; }
        set
        {
            if (_maxStamina == value) return;
            _maxStamina = value;
            OnPropertyChanged(nameof(MaxStamina));
        }
    }

    private float lifeCount;
    public float LifeCount
    {
        get { return lifeCount; }
        set
        {
            if (lifeCount == value) return;
            lifeCount = value;
            OnPropertyChanged(nameof(LifeCount));
        }
    }

    private float atk;
    public float AttackDamage
    {
        get { return atk; }
        set
        {
            if (atk == value) return;
            atk = value;
            OnPropertyChanged(nameof(AttackDamage));
        }
    }

    private Vector2 _movement;
    public Vector2 Movement
    {
        get { return _movement; }
        set
        {
            if(_movement == value) return;
            _movement = value;
            OnPropertyChanged(nameof(Movement));
        }
    }
    private Quaternion _rotation;
    public Quaternion Rotation
    {
        get { return _rotation; }
        set
        {
            if (_rotation == value) return;
            _rotation = value;
            OnPropertyChanged(nameof(Rotation));
        }
    }

    private Transform _lockOnTarget;
    public Transform LockOnTarget
    {
        get { return _lockOnTarget; }
        set
        {
            if (_lockOnTarget == value) return;
            _lockOnTarget = value;
            OnPropertyChanged(nameof(LockOnTarget));
        }
    }

    private MonsterView _assassinatedMonsters;
    public MonsterView AssassinatedMonsters
    {
        get { return _assassinatedMonsters; }
        set
        {
            if (_assassinatedMonsters == value) return;

            _assassinatedMonsters = value;
            OnPropertyChanged(nameof(AssassinatedMonsters));
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

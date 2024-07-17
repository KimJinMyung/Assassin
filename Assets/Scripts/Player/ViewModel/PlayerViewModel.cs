using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerViewModel : MonoBehaviour
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
    #region propertyEvent
    public event PropertyChangedEventHandler PropertyChanged;

    public virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}

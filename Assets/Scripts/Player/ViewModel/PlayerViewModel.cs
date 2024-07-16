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

    public float _stamina;
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

    #region propertyEvent
    public event PropertyChangedEventHandler PropertyChanged;

    public virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}

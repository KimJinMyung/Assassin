using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerViewModel vm;

    private void OnEnable()
    {
        if (vm == null)
        {
            vm = new PlayerViewModel();
            vm.PropertyChanged += OnPropertyChanged;
            vm.RegisterMoveVelocity(true);
        }
    }
    private void OnDisable()
    {
        if (vm != null)
        {
            vm.RegisterMoveVelocity(false);
            vm.PropertyChanged -= OnPropertyChanged;
            vm = null;
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    { 
    
    }
}

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Cinemachine Setting")]
    [SerializeField] private Cinemachine.AxisState x_Axis;
    [SerializeField] private Cinemachine.AxisState y_Axis;

    [Header("Camera LookAt")]
    [SerializeField] private Transform _lookAt;

    private PlayerViewModel vm;

    private void OnEnable()
    {
        if (vm == null)
        {
            vm = new PlayerViewModel();
            vm.PropertyChanged += OnPropertyChanged;
            vm.RegisterActorRotate(true);
            vm.RegisterMoveVelocity(true);
        }
    }
    private void OnDisable()
    {
        if (vm != null)
        {
            vm.RegisterMoveVelocity(false);
            vm.RegisterActorRotate(false);
            vm.PropertyChanged -= OnPropertyChanged;
            vm = null;
        }
    }

    private void Update()
    {
        CameraRotation_Move();
    }

    private void CameraRotation_Move()
    {
        x_Axis.Update(Time.fixedDeltaTime);
        y_Axis.Update(Time.fixedDeltaTime);

        Quaternion mouseRotation = Quaternion.Euler(y_Axis.Value, x_Axis.Value, 0f);
        _lookAt.rotation = Quaternion.Lerp(_lookAt.rotation, mouseRotation, 1f);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    { 
    
    }
}

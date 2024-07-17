using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.GridLayoutGroup;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Cinemachine Setting")]
    [SerializeField] private Cinemachine.AxisState x_Axis;
    [SerializeField] private Cinemachine.AxisState y_Axis;

    [Header("Camera LookAt")]
    [SerializeField] private Transform _lookAt;

    private PlayerView owner;
    private PlayerViewModel vm;

    private CharacterController playerController;

    private float MoveAngle;
    private float MoveSpeed;
    private bool isRun;
    private Quaternion mouseRotation;

    private void Awake()
    {
        owner = GetComponent<PlayerView>();
        playerController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        if (vm == null)
        {
            vm = new PlayerViewModel();
            vm.PropertyChanged += OnPropertyChanged;
            vm.RegisterActorRotate(true);
            vm.RegisterMoveVelocity(true);
        }

        InitCameraRotation();
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
        DecideMoveSpeed();
        Movement();
    }

    private void InitCameraRotation()
    {
        x_Axis.Value = 0;
        y_Axis.Value = 0;

        Vector3 initEulerAngle = _lookAt.rotation.eulerAngles;
        x_Axis.Value = initEulerAngle.y;
        y_Axis.Value = initEulerAngle.x;

        mouseRotation = _lookAt.rotation;
    }

    private void CameraRotation_Move()
    {
        x_Axis.Update(Time.fixedDeltaTime);
        y_Axis.Update(Time.fixedDeltaTime);

        mouseRotation = Quaternion.Euler(y_Axis.Value, x_Axis.Value, 0f);
        _lookAt.rotation = Quaternion.Lerp(_lookAt.rotation, mouseRotation, 1f);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (vm == null) return;

        vm.RequestMoveOnInput(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y);
    }

    private void Movement()
    {
        Vector3 moveDir = new Vector3(vm.Movement.x, 0, vm.Movement.y).normalized;

        if(moveDir.magnitude >= 0.1f)
        {
            MoveAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            Vector3 dir = Quaternion.Euler(0, MoveAngle, 0) * Vector3.forward;

            playerController.Move(dir.normalized * MoveSpeed * Time.deltaTime);
        }
    }

    private void DecideMoveSpeed()
    {
        if (isRun) MoveSpeed = owner.platerData.RunSpeed;
        else MoveSpeed = owner.platerData.WalkSpeed;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    { 
    
    }
}

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
    private PlayerLockOn playerSight;

    private Animator animator;
    private CharacterController playerController;

    private float MoveAngle;
    private float MoveSpeed;
    private bool isRun;
    private Quaternion mouseRotation;

    private void Awake()
    {
        owner = GetComponent<PlayerView>();
        playerSight = GetComponent<PlayerLockOn>();   
        playerController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
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

    private void FixedUpdate()
    {
        MeshRotation();
    }

    private void InitCameraRotation()
    {
        x_Axis.Value = 0;
        y_Axis.Value = 0.5f;

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

    public void OnSprint(InputAction.CallbackContext context)
    {
        isRun = context.ReadValue<float>() > 0.5f;
    }

    private void Movement()
    {
        if (!animator.GetBool("isMoveAble")) return;
        Vector3 moveDir = new Vector3(vm.Movement.x, 0, vm.Movement.y).normalized;

        if(moveDir.magnitude >= 0.1f)
        {
            MoveAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            Vector3 dir = Quaternion.Euler(0, MoveAngle, 0) * Vector3.forward;

            playerController.Move(dir.normalized * MoveSpeed * Time.deltaTime);
        }

        PlayerMeshAnimation();
    }

    private void MeshRotation()
    {
        if (!animator.GetBool("isMoveAble")) return;

        if (!IsAnimationRunning($"Attack.Attack{animator.GetInteger("AttackIndex") + 1}") && vm.Movement.magnitude >= 0.1f)
        {
            Quaternion cameraDir = Quaternion.Euler(0, MoveAngle, 0);
            Quaternion targetRotate = Quaternion.Lerp(transform.rotation, cameraDir, 100f * Time.fixedDeltaTime);
            vm.RequestActorRotate(targetRotate.x, targetRotate.y, targetRotate.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotate, 10f * Time.fixedDeltaTime);
        }
    }

    private void PlayerMeshAnimation()
    {
        if (!animator.GetBool("LockOn"))
        {
            //float angleValue = Vector3.Dot(owner.transform.forward, owner._moveDir.normalized * 0.3f);

            float MoveValue = Mathf.Abs(vm.Movement.y) < 0.1f && Mathf.Abs(vm.Movement.x) < 0.1f ? 0f : isRun ? 3f : 1f;

            animator.SetFloat("z", Mathf.Lerp(animator.GetFloat("z"), MoveValue, 10f * Time.deltaTime));
        }
        else
        {
            float MoveValue = Mathf.Abs(vm.Movement.y) < 0.1f && Mathf.Abs(vm.Movement.x) < 0.1f ? 0f : isRun ? 3f : 1f;

            animator.SetFloat("z", Mathf.Lerp(animator.GetFloat("z"), vm.Movement.y * MoveValue, 10f * Time.deltaTime));
            animator.SetFloat("x", Mathf.Lerp(animator.GetFloat("x"), vm.Movement.x * MoveValue, 10f * Time.deltaTime));
        }
    }

    private void DecideMoveSpeed()
    {
        if (isRun) MoveSpeed = owner.playerData.RunSpeed;
        else MoveSpeed = owner.playerData.WalkSpeed;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    { 
    
    }

    private bool IsAnimationRunning(string animationName)
    {
        if (animator == null) return false;

        bool isRunning = false;
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(animationName))
        {
            float normalizedTime = stateInfo.normalizedTime;
            isRunning = normalizedTime >= 0 && normalizedTime < 1.0f;
        }

        return isRunning;
    }
}

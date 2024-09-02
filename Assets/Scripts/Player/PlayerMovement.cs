using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using Player;

namespace Temp
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Player Cinemachine Setting")]
        [SerializeField] private Cinemachine.AxisState x_Axis;
        [SerializeField] private Cinemachine.AxisState y_Axis;

        [Header("Camera LookAt")]
        [SerializeField] private Transform _lookAt;


        #region gravity
        //중력 값
        private float gravity = -20;
        public float GravityValue { get { return gravity; } }
        //현재 중력 가속도
        public float _velocity { get; set; }

        public bool isGravityAble { get; set; } = true;

        [Header("그라운드 확인 overlap")]
        [SerializeField]
        private Transform overlapPos;

        [SerializeField]
        private LayerMask gravityLayermask;
        #endregion

        private PlayerView owner;
        public PlayerViewModel vm { get; private set; }
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
            Gravity();

            if (!animator.GetBool("LockOn"))
            {
                CameraRotation_Move();
            }
            else
            {
                CamearaRotation_Target(owner.ViewModel.LockOnTarget);
            }
            DecideMoveSpeed();
            Movement();
        }

        private void FixedUpdate()
        {
            Rotation();
        }

        private void Gravity()
        {
            if (!isGravityAble) return;

            Debug.DrawRay(transform.position, Vector3.down * 0.01f, Color.red);

            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.1f, gravityLayermask) && _velocity <= 0.1f)
            {
                _velocity = 0f;

            }
            else
            {
                _velocity += gravity * 0.5f * Time.deltaTime;

            }

            playerController.Move(new Vector3(0, _velocity, 0) * Time.deltaTime);
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

        private void CamearaRotation_Target(Transform target)
        {
            if (target == null) return;

            // target을 향한 회전 값을 계산
            Quaternion targetRotation = Quaternion.LookRotation(target.position - _lookAt.position);

            // 부드러운 회전을 위해 Slerp 사용
            _lookAt.rotation = Quaternion.Slerp(_lookAt.rotation, targetRotation, Time.deltaTime * 10f);
            InitCameraRotation();
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

        public void OnJump(InputAction.CallbackContext context)
        {

        }

        private void Movement()
        {
            if (!animator.GetBool("isMoveAble")) return;
            Vector3 moveDir = new Vector3(vm.Movement.x, 0, vm.Movement.y).normalized;

            if (moveDir.magnitude >= 0.1f)
            {
                MoveAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                Vector3 dir = Quaternion.Euler(0, MoveAngle, 0) * Vector3.forward;

                playerController.Move(dir.normalized * MoveSpeed * Time.deltaTime);
            }

            PlayerMeshAnimation();
        }

        private void Rotation()
        {
            if (!animator.GetBool("isMoveAble")) return;

            if (animator.GetBool("LockOn"))
            {
                LookAtTargetOnYAxis(owner.ViewModel.LockOnTarget, transform);
            }
            else
            {
                MeshRotation();
            }
        }

        private void MeshRotation()
        {
            if (!IsAnimationRunning($"Attack.Attack{animator.GetInteger("AttackIndex") + 1}") && vm.Movement.magnitude >= 0.1f)
            {
                Quaternion cameraDir = Quaternion.Euler(0, MoveAngle, 0);
                Quaternion targetRotate = Quaternion.Lerp(transform.rotation, cameraDir, 100f * Time.fixedDeltaTime);
                vm.RequestActorRotate(targetRotate.x, targetRotate.y, targetRotate.z);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotate, 10f * Time.fixedDeltaTime);
            }
        }
        private void LookAtTargetOnYAxis(Transform target, Transform playerMesh)
        {
            if (target == null) return;

            Vector3 dirTarget = target.position - playerMesh.position;
            dirTarget.y = 0;
            Quaternion rotation = Quaternion.LookRotation(dirTarget);
            playerMesh.rotation = Quaternion.Lerp(playerMesh.rotation, Quaternion.Euler(0, rotation.eulerAngles.y, 0), 10f * Time.fixedDeltaTime);
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

}
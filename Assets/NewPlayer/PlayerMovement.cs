using UnityEngine;
using UnityEngine.InputSystem;
using EventEnum;
using System.Collections;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float WalkSpeed = 10f;
        [SerializeField] private float RunSpeed = 10f;
        [SerializeField] private float jumpForce = 80f;  // ���� �� �߰�
        [SerializeField] private float fallMultiplier = 2.5f;  // �߰����� �߷� ���ӵ�

        [Header("Camera LookAt")]
        [SerializeField] private Transform CameraArm;

        [Header("Mesh")]
        [SerializeField] private GameObject mesh;

        [Header("Ground Check")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundDistance = 0.4f;

        private Rigidbody rb;
        private Animator animator;

        private Vector2 movement;
        private float MoveAngle;
        private Vector3 dir;
        private float MoveSpeed;

        private Vector3 CameraDistance;

        public bool isRun { get; private set; }
        public bool isLockOn { get; private set; }
        public bool isJumping { get; private set; }
        public bool isGround { get; private set; }

        private bool isNotMove;
        private bool isAttacking;
        private bool isLand;
        private bool isGrapplingRotation;

        //�ӽÿ� 
        private Transform target;

        private void Awake()
        {
            rb = GetComponentInChildren<Rigidbody>();
            animator = GetComponentInChildren<Animator>();
            AddEvent();

            CameraDistance = CameraArm.position - mesh.transform.position;
        }

        private void OnDestroy()
        {
            RemoveEvent();
        }

        private void OnEnable()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            MoveSpeed = WalkSpeed;
            isJumping = false;
            isNotMove = false;
            isAttacking = false;
            isLand = false;
            isGrapplingRotation = false;
        }

        private void Update()
        {
            LockOnLookAround();
            LookAround();
            Rotation();
        }

        private void FixedUpdate()
        {
            CheckIsGrounded();
            Movement();
            PlayerMeshMovementAnimation();
            ApplyExtraGravity();
        }

        private void AddEvent()
        {
            EventManager<PlayerAction>.Binding(true, PlayerAction.Jump, Jump);
            EventManager<PlayerAction>.Binding<float, float>(true, PlayerAction.ChangedSpeed, SetMovementSpeed);
            EventManager<PlayerAction>.Binding<bool>(true, PlayerAction.IsNotMoveAble, SetMovementAble);
            EventManager<PlayerAction>.Binding<bool>(true, PlayerAction.IsAttacking, SetIsAttacking);
            EventManager<CameraEvent>.Binding(true, CameraEvent.UpdateCameraPosition, CameraFollowCharacterMesh);
            EventManager<PlayerAction>.Binding<bool>(true, PlayerAction.IsLockOn, SetLockOnMode);
            EventManager<PlayerAction>.Binding<Transform>(true, PlayerAction.ChangedLockOnTarget, SetLockOnTarget);
            EventManager<PlayerAction>.Binding<bool>(true, PlayerAction.Grappling, SetGrappling);
        }

        private void RemoveEvent()
        {
            EventManager<PlayerAction>.Binding(false, PlayerAction.Jump, Jump);
            EventManager<PlayerAction>.Binding<float, float>(false, PlayerAction.ChangedSpeed, SetMovementSpeed);
            EventManager<PlayerAction>.Binding<bool>(false, PlayerAction.IsNotMoveAble, SetMovementAble);
            EventManager<PlayerAction>.Binding<bool>(false, PlayerAction.IsAttacking, SetIsAttacking);
            EventManager<CameraEvent>.Binding(false, CameraEvent.UpdateCameraPosition, CameraFollowCharacterMesh);
            EventManager<PlayerAction>.Binding<bool>(false, PlayerAction.IsLockOn, SetLockOnMode);
            EventManager<PlayerAction>.Binding<Transform>(false, PlayerAction.ChangedLockOnTarget, SetLockOnTarget);
            EventManager<PlayerAction>.Binding<bool>(false, PlayerAction.Grappling, SetGrappling);
        }

        private void SetMovementSpeed(float walkSpeed, float runSpeed)
        {
            WalkSpeed = walkSpeed;
            RunSpeed = runSpeed;
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            movement = context.ReadValue<Vector2>();
        }

        public void OnRunning(InputAction.CallbackContext context)
        {
            isRun = context.ReadValue<float>() > 0.1f;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started && isGround)
            {
                isJumping = true;
                animator.SetTrigger("isJumping");
            }
        }

        private void Jump()
        {
            if (isJumping) return;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            // ������ y�� �ӵ��� �ʱ�ȭ�Ͽ� �ݺ� ���� �� y�� �ӵ��� �������� �ʵ��� ��
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        private void Movement()
        {
            if (isNotMove)
            {
                return;
            }

            var moveDIr = new Vector3(movement.x, 0, movement.y).normalized;

            if (moveDIr.magnitude >= 0.1f)
            {
                MoveAngle = Mathf.Atan2(moveDIr.x, moveDIr.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                dir = Quaternion.Euler(0, MoveAngle, 0) * Vector3.forward;

                MoveSpeed = !isRun ? Mathf.Lerp(MoveSpeed, WalkSpeed, Time.deltaTime * 10f) : Mathf.Lerp(MoveSpeed, RunSpeed, Time.deltaTime * 10f);

                MoveSpeed = isLand? MoveSpeed * 0.5f : MoveSpeed;

                float speed = 0;

                if (isAttacking)
                {
                    speed = WalkSpeed;
                }
                else
                {
                    speed = MoveSpeed;
                }

                var newPos = rb.position + dir.normalized * speed * Time.fixedDeltaTime;

                rb.MovePosition(newPos);
            }
        }

        private void ApplyExtraGravity()
        {
            if (rb.velocity.y < 0 && !isGround)
            {
                animator.SetBool("isFalling", true); // ���߿��� �������� ���� ��
            }
            else
            {
                animator.SetBool("isFalling", false);
            }

            // �߰� �߷� ����
            if (rb.velocity.y < 0)
            {
                rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }

            if (isJumping && rb.velocity.y <= 0)
            {
                isJumping = false;
                animator.SetBool("isFalling", true); // ���߿��� �������� ������ ��
            }
        }

        private void LockOnLookAround()
        {
            if (!isLockOn || target == null) return;

            Vector3 dir = (target.position - CameraArm.position);
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            CameraArm.rotation = Quaternion.Slerp(CameraArm.rotation, targetRotation, Time.deltaTime * 1f);
        }

        private void LookAround()
        {
            Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            var CameraAngle = CameraArm.rotation.eulerAngles;
            float x = CameraAngle.x - mouseDelta.y;

            if (x < 180f)
                x = Mathf.Clamp(x, -1f, 70f);
            else
                x = Mathf.Clamp(x, 310f, 361f);

            CameraArm.rotation = Quaternion.Euler(x, CameraAngle.y + mouseDelta.x, CameraAngle.z);
        }

        private void Rotation()
        {
            if (isGrapplingRotation) return;
            if (isLockOn)
            {
                // Lock On ����� ��: ĳ���Ͱ� �׻� Ÿ���� ���� ȸ��
                Vector3 directionToTarget = (target.position - mesh.transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToTarget.x, 0, directionToTarget.z));
                mesh.transform.rotation = Quaternion.Slerp(mesh.transform.rotation, lookRotation, Time.deltaTime * 10f);  // �ε巴�� ȸ��
            }
            else
            {
                if (dir == Vector3.zero) return;

                // �Ϲ� ��忡���� �̵� �������� �ε巴�� ȸ��
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                mesh.transform.rotation = Quaternion.Slerp(mesh.transform.rotation, targetRotation, Time.deltaTime * 10f);  // �ε巴�� ȸ��
            }
        }

        private void PlayerMeshMovementAnimation()
        {
            if (!isGround) return;

            if (!animator.GetBool("LockOn"))
            {
                //float angleValue = Vector3.Dot(owner.transform.forward, owner._moveDir.normalized * 0.3f);

                float MoveValue = Mathf.Abs(movement.y) < 0.1f && Mathf.Abs(movement.x) < 0.1f ? 0f : isRun ? 3f : 1f;

                animator.SetFloat("z", Mathf.Lerp(animator.GetFloat("z"), MoveValue, 10f * Time.deltaTime));
            }
            else
            {
                float MoveValue = Mathf.Abs(movement.y) < 0.1f && Mathf.Abs(movement.x) < 0.1f ? 0f : isRun ? 3f : 1f;

                animator.SetFloat("z", Mathf.Lerp(animator.GetFloat("z"), movement.y * MoveValue, 10f * Time.deltaTime));
                animator.SetFloat("x", Mathf.Lerp(animator.GetFloat("x"), movement.x * MoveValue, 10f * Time.deltaTime));
            }
        }

        private void CheckIsGrounded()
        {
            bool wasGrounded = isGround;
            isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGround && !wasGrounded && !isJumping)
            {
                animator.SetTrigger("Land");
            }
        }

        private void SetMovementAble(bool isNotMoveAble)
        {
            isNotMove = isNotMoveAble;
        }

        private void SetIsAttacking(bool isAttacking)
        {
            this.isAttacking = isAttacking;
        }

        private void SetLockOnMode(bool isLockOn)
        {
            this.isLockOn = isLockOn;
        }

        private void SetLockOnTarget(Transform target)
        {
            this.target = target;
        }

        private void SetGrappling(bool grappling)
        {
            this.isGrapplingRotation = grappling;
        }

        private void CameraFollowCharacterMesh()
        {
            var targetPos = CameraDistance + mesh.transform.position;
            CameraArm.position = Vector3.Lerp(CameraArm.position, targetPos, 5 * Time.deltaTime);
        }        

        IEnumerator PlayerLandGround()
        {
            isLand = true;

            yield return new WaitForSeconds(1.5f);

            isLand = false;
        }
    }
}

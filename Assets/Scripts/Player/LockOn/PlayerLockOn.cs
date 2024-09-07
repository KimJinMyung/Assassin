using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using Player;
using EventEnum;

public class PlayerLockOn : MonoBehaviour
{
    [Header("LockOnAble Layer")]
    [SerializeField] private LayerMask _lockOnAbleMask;

    [Header("눈")]
    [SerializeField] private Transform Eye;

    [Header("감지 길이")]
    [SerializeField] private float _viewRange;

    [Header("몬스터 감지 길이")]
    [SerializeField] private float _viewRangeToMonster;

    [Header("시야 각도")]
    [SerializeField] private float _viewAngle;

    private bool isLockOnMode;

    private PlayerView owner;

    private LockOnViewModel _viewModel;
    public LockOnViewModel ViewModel { get { return _viewModel; } }

    private Animator animator;
    private Transform _lockOnAbleObject;

    private readonly int hashLockOn = Animator.StringToHash("LockOn");

    private void Awake()
    {
        owner = GetComponent<PlayerView>();
        animator = GetComponent<Animator>();

        AddEvent();
    }

    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void AddEvent()
    {
        if (_viewModel == null)
        {
            _viewModel = new LockOnViewModel();
            _viewModel.PropertyChanged += OnPropertyChanged;
            _viewModel.RegisterLockOnTargetListChanged(true);
            _viewModel.RegisterLockOnAbleTargetChanged(true);
            _viewModel.RegisterLockOnViewModel_TargetChanged(true);
        }
    }

    private void RemoveEvent()
    {
        if (_viewModel != null)
        {
            _viewModel.RegisterLockOnViewModel_TargetChanged(false);
            _viewModel.RegisterLockOnAbleTargetChanged(false);
            _viewModel.RegisterLockOnTargetListChanged(false);
            _viewModel.PropertyChanged -= OnPropertyChanged;
            _viewModel = null;
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_viewModel.HitColliders):
                break;
            case nameof(_viewModel.LockOnTarget):
                isLockOnMode = (_viewModel.LockOnTarget != null);
                EventManager<PlayerAction>.TriggerEvent(PlayerAction.IsLockOn, isLockOnMode);
                if(isLockOnMode)
                    EventManager<PlayerAction>.TriggerEvent(PlayerAction.ChangedLockOnTarget, _viewModel.LockOnTarget);
                animator.SetBool(hashLockOn, isLockOnMode);
                break;
        }
    }

    private void Update()
    {
        if (owner.ViewModel.LockOnTarget == null || owner.ViewModel.LockOnTarget.gameObject.layer == LayerMask.NameToLayer("Dead"))
        {
            owner.ViewModel.RequestLockOnTarget(null);
        }

        _lockOnAbleObject = DetectingTarget();
        _viewModel.RequestLockOnAbleTarget(_lockOnAbleObject);
    }

    public void OnLockOnMode(InputAction.CallbackContext context)
    {
        if (_viewModel.HitColliders.Count <= 0) return;

        if (context.performed)
        {
            if (_lockOnAbleObject != null && _lockOnAbleObject.CompareTag("RopePoint")) return;
            if (isLockOnMode && _lockOnAbleObject == _viewModel.LockOnTarget)
            {
                _viewModel.RequestLockOnViewModel_Target(null, owner);
            }
            else
            {
                _viewModel.RequestLockOnViewModel_Target(_lockOnAbleObject, owner);
            }
        }
    }

    private Transform DetectingTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(Eye.position, _viewRange, _lockOnAbleMask);

        return ReturnTarget(colliders);
    }

    Transform ReturnTarget(Collider[] colliders)
    {
        Transform closestTarget = null;
        float closestAngle = Mathf.Infinity;

        List<Transform> tempLockOnAbleList = new List<Transform>();

        foreach (var collider in colliders)
        {
            Vector3 dirTarget = (collider.transform.position - Camera.main.transform.position).normalized;
            float angleToTarget = Vector3.Angle(Camera.main.transform.forward, dirTarget);

            if (angleToTarget < _viewAngle)
            {
                float cameraDis = Vector3.Distance(new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z), new Vector3(transform.position.x, 0, transform.position.z));

                float viewRange = 0f;

                if (!collider.CompareTag("RopePoint"))
                {
                    viewRange = _viewRangeToMonster;
                }
                else
                {
                    viewRange = _viewRange;
                }

                if (Physics.Raycast(Camera.main.transform.position, dirTarget, out RaycastHit hit, viewRange + cameraDis, _lockOnAbleMask))
                {
                    if (hit.collider == collider)
                    {
                        if (hit.collider.CompareTag("RopePoint") && !IsGrapplingAblePoint(hit.point)) continue;

                        tempLockOnAbleList.Add(collider.transform);

                        if (angleToTarget < closestAngle)
                        {
                            closestAngle = angleToTarget;
                            closestTarget = hit.transform;
                        }
                    }
                }

            }
        }

        Transform _lockOnTarget = owner.ViewModel.LockOnTarget;

        if (_lockOnTarget != null && !tempLockOnAbleList.Contains(_lockOnTarget) && _lockOnTarget.gameObject.layer != LayerMask.NameToLayer("Dead")) tempLockOnAbleList.Add(_lockOnTarget);

        _viewModel.RequestLockOnTargetList(tempLockOnAbleList);

        return closestTarget;
    }

    private bool IsGrapplingAblePoint(Vector3 grapplingPoint, float checkAngle = 45f)
    {
        Vector3 direcion = grapplingPoint - owner.transform.position;

        direcion.Normalize();

        float dot = Vector3.Dot(direcion, Vector3.up);

        float angleToGrapplingPoint = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (angleToGrapplingPoint <= checkAngle) return false;
        else return true;
    }
}

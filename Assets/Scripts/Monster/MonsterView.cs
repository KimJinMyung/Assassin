using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.GridLayoutGroup;

public enum MonsterType
{
    monster_A,
    monster_B,
    Boss
}

public class MonsterView : MonoBehaviour
{
    [Header("Current Monster Type")]
    [SerializeField] private MonsterType _type;
    public MonsterType Type { get { return _type; } }

    [Serializable]
    public class MonsterMesh
    {
        public MonsterType _monsterType;
        public GameObject mesh;
        public RuntimeAnimatorController animation_controller;
    }

    [Serializable]
    public class WeaponsMesh
    {
        public WeaponsType WeaponsType;
        public GameObject weaponMesh;
    }

    [SerializeField]
    MonsterMesh[] monsterMeshes;

    [SerializeField]
    WeaponsMesh[] monsterWeapons;

    public MonsterViewModel vm { get; private set; }

    private MonsterData _initMonsterData;
    private List<Monster_Attack> monsterAttackMethodList = new List<Monster_Attack>();

    private MonsterBTRunner _monsterBTRunner;

    private NavMeshAgent agent;
    private Animator animator;
    private Rigidbody rb;

    public int monsterId { get; private set; }

    [SerializeField] private LayerMask GroundLayer;
    private float patrolWaitTimer;
    private float patrolWaitTime;
    private float moveSpeed;
    private float distance;
    private float AttackRange;
    private Vector3 patrolPos;
    private float circleDelayTimer;
    private float circlingDir;
    private float circlingTImer;
    private bool isPatrol;
    public bool isAttackAble { get; private set; }
    public bool isCircling { get; private set; }
    private bool isHurt;
    private bool isDead;
    public float CombatMovementTimer { get; private set; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        monsterId = gameObject.GetInstanceID();
        isDead = false;
        patrolWaitTime = UnityEngine.Random.Range(1.5f, 3f);
        patrolWaitTimer = patrolWaitTime;
        circleDelayTimer = UnityEngine.Random.Range(2f, 5f);
        circlingTImer = UnityEngine.Random.Range(3f, 6f);
        gameObject.layer = LayerMask.NameToLayer("Monster");

        if (vm == null)
        {
            vm = new MonsterViewModel();
            vm.PropertyChanged += OnPropertyChanged;
            vm.RegisterMonsterHPChanged(true, monsterId);
            vm.RegisterMonsterStaminaChanged(true, monsterId);
            vm.RegisterAttackMethodChanged(monsterId, true);
        }

        SetMonsterInfo(_type);
        _monsterBTRunner = new MonsterBTRunner(SetMonsterBT());

        //디버깅 용
        vm.TraceTarget = GameObject.FindWithTag("Player").transform;
    }

    private void OnDisable()
    {
        if(vm != null)
        {
            vm.RegisterAttackMethodChanged(monsterId, false);
            vm.RegisterMonsterStaminaChanged(false, monsterId);
            vm.RegisterMonsterHPChanged(false, monsterId);
            vm.PropertyChanged -= OnPropertyChanged;
            vm = null;
        }
    }

    private void SetMonsterInfo(MonsterType type)
    {
        var monster = DataManager.Instance.GetMonsterData((int)type);
        if (monster == null) return;

        _initMonsterData = monster.MonsterDataClone();

        //체력과 스테미너 초기 설정
        vm.RequestMonsterHPChanged(monsterId, _initMonsterData.HP);
        vm.RequestMonsterStaminaChanged(_initMonsterData.Stamina, monsterId);

        ChangedMonsterAnimationController();
        UpdateAttackMethod_Data(monster);

    }

    private void ChangedMonsterAnimationController()
    {
        foreach(var monsterMesh in monsterMeshes)
        {
            if(_type == monsterMesh._monsterType)
            {
                animator.runtimeAnimatorController = monsterMesh.animation_controller;
            }
        }
    }

    private void UpdateAttackMethod_Data(MonsterData monsterData)
    {
        if(monsterAttackMethodList.Count > 0) monsterAttackMethodList.Clear();

        var attackList = monsterData.AttackMethodName;
        if(attackList.Count > 0)
        {
            foreach(var attackName in attackList)
            {
                var attack = DataManager.Instance.GetAttackMethodName(attackName);
                monsterAttackMethodList.Add(attack.Clone());
            }
        }

        monsterAttackMethodList = monsterAttackMethodList.OrderByDescending(e => e.AttackRange).ThenBy(e => e.AttackSpeed).ToList();
        vm.RequestAttackMethodChanged(monsterId, monsterAttackMethodList, this);
        ChangedWeaponsMesh();
    }

    private void ChangedWeaponsMesh()
    {
        GameObject currentWeaponMesh = default;

        foreach (var weapon in monsterWeapons)
        {
            string AttackMethodName = vm.CurrentAttackMethod.DataName.Replace("\"", "");
            string CurrentWeaponsType = Enum.GetName(typeof(WeaponsType), weapon.WeaponsType);

            if (AttackMethodName == CurrentWeaponsType)
            {
                weapon.weaponMesh.SetActive(true);
                currentWeaponMesh = weapon.weaponMesh;
                continue;
            }

            if (weapon.weaponMesh.Equals(currentWeaponMesh)) continue;

            weapon.weaponMesh.SetActive(false);
        }
    }

    private void UpdateAttackMethod()
    {
        //if (vm.TraceTarget == null) return;

        vm.RequestAttackMethodChanged(monsterId, monsterAttackMethodList, this);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    { 
        switch (e.PropertyName)
        {           
            case nameof(vm.CurrentAttackMethod):
                ChangedWeaponsMesh();
                AttackRange = vm.CurrentAttackMethod.AttackRange;
                break;
            case nameof(vm.TraceTarget):
                animator.SetBool("ComBatMode", vm.TraceTarget!= null);
                break;
        }
    }

    private void Update()
    {
        UpdateAttackMethod();

        _monsterBTRunner.Execute();

        animator.SetBool("Circling", isCircling);

        if (vm.TraceTarget != null)
        {
            CombatMovementTimer += Time.deltaTime;
        }

        //디버그 용
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Hurt(50, transform);
        }

        Debug.Log(vm.CurrentAttackMethod.DataName);

        if(vm.TraceTarget != null)
        {
            distance = Vector3.Distance(transform.position, vm.TraceTarget.position);
        }
    }

    private void FixedUpdate()
    {
        if (isCircling)
        {
            var VecToTarget = transform.position - vm.TraceTarget.position;
            var rotatedPos = Quaternion.Euler(0, circlingDir * 20f * Time.fixedDeltaTime, 0) * VecToTarget;

            agent.Move(rotatedPos - VecToTarget);
            transform.rotation = Quaternion.LookRotation(-rotatedPos);
            animator.SetFloat("CirclingDir", circlingDir);
        }
    }

    IBTNode SetMonsterBT()
    {
        var DieNodeList = new List<IBTNode>();
        DieNodeList.Add(new ActionNode(CheckIsDeadOnUpdate));
        DieNodeList.Add(new ActionNode(CompleteDieAnimation));
        var DieSeqNode = new SequenceNode(DieNodeList);

        var HurtNodeList = new List<IBTNode>();
        HurtNodeList.Add(new ActionNode(CheckMonsterHPOnUPdate));
        HurtNodeList.Add(new ActionNode(CompleteHurtAnimation));
        var HurtSeqNode = new SequenceNode(HurtNodeList);

        var IdleNodeList = new List<IBTNode>();
        IdleNodeList.Add(new ActionNode(WaitPatrolDelay));
        IdleNodeList.Add(new ActionNode(RandomPatrolPosOnUpdate));
        IdleNodeList.Add(new ActionNode(CheckMoveDirToTransformForward));
        IdleNodeList.Add(new ActionNode(PatrolMoveOnUpdate));
        var IdleSeqNode = new SequenceNode(IdleNodeList);

        var FollowingNodeList = new List<IBTNode>();
        FollowingNodeList.Add(new ActionNode(CheckFolloewingRangeOnUpdate));
        FollowingNodeList.Add(new ActionNode(WaitCirclingDelay));
        FollowingNodeList.Add(new ActionNode(Circling));
        var FollowingSeqNode = new SequenceNode(FollowingNodeList);

        var rootNodeList = new List<IBTNode>();
        rootNodeList.Add(DieSeqNode);
        rootNodeList.Add(HurtSeqNode);
        rootNodeList.Add(IdleSeqNode);
        rootNodeList.Add(FollowingSeqNode);
        var rootSelectNode = new SelectorNode(rootNodeList);
        return rootSelectNode;
    }

    #region Die

    IBTNode.EBTNodeState CheckIsDeadOnUpdate()
    {
        if (!isDead) return IBTNode.EBTNodeState.Fail;
        if (animator.GetBool("Dead")) return IBTNode.EBTNodeState.Success;

        isHurt = false;
        agent.speed = 0;
        agent.ResetPath();

        if(animator.layerCount > 1) animator.SetLayerWeight(1, 0);

        gameObject.layer = LayerMask.NameToLayer("Die");
        animator.SetBool("Dead", true);
        animator.SetTrigger("Die");
        return IBTNode.EBTNodeState.Success;
    }

    IBTNode.EBTNodeState CompleteDieAnimation()
    {
        if (!isDead) return IBTNode.EBTNodeState.Fail;

        if (IsAnimationRunning("Die"))
        {
            return IBTNode.EBTNodeState.Running;
        }

        //모습을 제거
        StartCoroutine(DeadMonsterActive());

        return IBTNode.EBTNodeState.Success;
    }

    IEnumerator DeadMonsterActive()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    #endregion

    #region Hurt
    public void Hurt(float Damage, Transform attacker)
    {
        vm.RequestMonsterHPChanged(monsterId, vm.HP - Damage);
        Debug.Log(vm.HP);

        if(vm.HP <= 0)
        {
            isDead = true;
        }
        else
        {
            isHurt = true;
            //넉백 구간
        }
    }
    
    IBTNode.EBTNodeState CheckMonsterHPOnUPdate()
    {
        if (!isHurt || isDead) return IBTNode.EBTNodeState.Fail;
        if (animator.GetBool("Hit")) return IBTNode.EBTNodeState.Success;

        animator.SetBool("Hit", true);
        animator.SetTrigger("Hurt");
        //rb.isKinematic = false;
        return IBTNode.EBTNodeState.Success;
    }

    IBTNode.EBTNodeState CompleteHurtAnimation()
    {
        if (!isHurt || isDead) return IBTNode.EBTNodeState.Fail;

        if (IsAnimationRunning("Hurt"))
        {
            return IBTNode.EBTNodeState.Running;
        }

        isHurt = false;
        animator.SetBool("Hit", false);
        return IBTNode.EBTNodeState.Success;
    }
    #endregion

    #region Idle
    IBTNode.EBTNodeState WaitPatrolDelay()
    {
        if (vm.TraceTarget != null) return IBTNode.EBTNodeState.Fail;
        if (isPatrol) return IBTNode.EBTNodeState.Success;

        agent.speed = _initMonsterData.WalkSpeed;

        if(patrolWaitTimer > 0)
        {
            patrolWaitTimer -= Time.deltaTime;
            return IBTNode.EBTNodeState.Running;
        }
        else
        {
            isPatrol = true;
            patrolWaitTime = UnityEngine.Random.Range(1.5f, 3f);
            patrolWaitTimer = patrolWaitTime;
            return IBTNode.EBTNodeState.Success;
        }
    }

    IBTNode.EBTNodeState RandomPatrolPosOnUpdate()
    {
        if (vm.TraceTarget != null) return IBTNode.EBTNodeState.Fail;
        if (Vector3.Distance(transform.position, patrolPos) > 0.1f) return IBTNode.EBTNodeState.Success;

        if(RandomPatrolEndPosition(transform.position, _initMonsterData.ViewRange) != Vector3.zero)
        {
            return IBTNode.EBTNodeState.Success;
        }

        return IBTNode.EBTNodeState.Running;
    }

    IBTNode.EBTNodeState CheckMoveDirToTransformForward()
    {
        if (vm.TraceTarget != null) return IBTNode.EBTNodeState.Fail;

        Vector3 dir = (patrolPos - transform.position);
        dir.y = 0f;
        dir.Normalize();

        float angle = Vector3.Angle(transform.forward, dir);

        float rotationDirection = Vector3.SignedAngle(transform.forward, dir, Vector3.up) > 0 ? 1f : -1f;

        if (angle > 20f)
        {            
            Quaternion rotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, agent.angularSpeed * Time.deltaTime);
            animator.SetBool("Rotate", true);
            animator.SetFloat("Rotation", rotationDirection);
            return IBTNode.EBTNodeState.Running;
        }

        animator.SetBool("Rotate", false);
        animator.SetFloat("Rotation", 0);
        return IBTNode.EBTNodeState.Success;
    }

    IBTNode.EBTNodeState PatrolMoveOnUpdate()
    {
        if (vm.TraceTarget != null) return IBTNode.EBTNodeState.Fail;

        agent.stoppingDistance = 0.1f;
        agent.speed = _initMonsterData.WalkSpeed;

        MoveToTarget(patrolPos);

        moveSpeed = Vector3.Distance(transform.position, patrolPos) > 0.1f ? 1 : 0;

        Debug.Log(moveSpeed);

        animator.SetFloat("MoveSpeed", moveSpeed);

        if (agent.remainingDistance > 0.1f) return IBTNode.EBTNodeState.Running;

        isPatrol = false;
        return IBTNode.EBTNodeState.Success;
    }

    #endregion

    #region Following

    //Follow
    IBTNode.EBTNodeState CheckFolloewingRangeOnUpdate()
    {
        if (vm.TraceTarget == null) return IBTNode.EBTNodeState.Fail;

        agent.speed = _initMonsterData.RunSpeed;

        if (distance > AttackRange + 1.5f)
        {
            isCircling = false;
            isAttackAble = false;
            agent.stoppingDistance = AttackRange + 1.5f;
            animator.SetFloat("MoveSpeed", 1);
            MoveToTarget(vm.TraceTarget.position);
            return IBTNode.EBTNodeState.Running;
        }

        isAttackAble = true;
        animator.SetFloat("MoveSpeed", 0);
        return IBTNode.EBTNodeState.Success;
    }

    //Battle => Circling
    IBTNode.EBTNodeState WaitCirclingDelay()
    {
        if (vm.TraceTarget == null) return IBTNode.EBTNodeState.Fail;
        if (distance > AttackRange + 1.5f) return IBTNode.EBTNodeState.Fail;
        if (isCircling) return IBTNode.EBTNodeState.Success;

        if(circleDelayTimer > 0)
        {
            circleDelayTimer -= Time.deltaTime;
            return IBTNode.EBTNodeState.Running;
        }

        circleDelayTimer = UnityEngine.Random.Range(2f, 5f);

        if (UnityEngine.Random.Range(0,2) == 0)
        {
            circlingDir = UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
            isCircling = true;
            isAttackAble = false;
            return IBTNode.EBTNodeState.Success;
        }

        return IBTNode.EBTNodeState.Running;
    }

    IBTNode.EBTNodeState Circling()
    {
        if (vm.TraceTarget == null) return IBTNode.EBTNodeState.Fail;
        if (distance > AttackRange + 1.5f) return IBTNode.EBTNodeState.Fail;
        if (!isCircling) return IBTNode.EBTNodeState.Fail;

        if (circlingTImer > 0)
        {
            circlingTImer -= Time.deltaTime;
            return IBTNode.EBTNodeState.Running;
        }

        isCircling = false;
        circlingTImer = UnityEngine.Random.Range(3f, 6f);
        return IBTNode.EBTNodeState.Success;
    }
    #endregion

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

    private void MoveToTarget(Vector3 targetPos)
    {
        agent.SetDestination(targetPos);
    }

    private Vector3 RandomPatrolEndPosition(Vector3 originPosition, float distance)
    {
        Vector3 randomPoint = originPosition + UnityEngine.Random.insideUnitSphere * distance;

        if (Physics.Raycast(randomPoint + Vector3.up * (distance + 1f), Vector3.down, out RaycastHit hitInfo, distance + 5f, GroundLayer))
        {
            randomPoint.y = hitInfo.point.y;

            int walkableAreaMask = 1 << NavMesh.GetAreaFromName("Walkable");
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, walkableAreaMask))
            {
                patrolPos = hit.position;
                return patrolPos;
            }
        }

        return Vector3.zero;
    }
}

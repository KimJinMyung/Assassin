using BehaviorDesigner.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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

    public MonsterData _monsterData { get; private set; }
    private List<Monster_Attack> monsterAttackMethodList = new List<Monster_Attack>();

    private MonsterBTRunner _monsterBTRunner;
    public BehaviorTree _behaviorTree { get; private set; }

    private NavMeshAgent agent;
    public Animator animator { get; private set; }
    private Rigidbody rb;
    private CapsuleCollider Collider;

    public int monsterId { get; private set; }
    public float MonsterHeight { get; private set; }

    #region attack
    public float CombatMovementTimer { get; private set; }
    public int AttackMethodCount { get; private set; }
    #endregion
    #region Hurt
    public Vector3 KnockbackDir { get; private set; }
    #endregion

    private bool isPatrol;

    public bool isCircling { get; private set; }
    public bool isParried { get; private set; }
    private bool isParryStart;
    public bool isSubdued { get; private set; }
    private float _subduedTimer;
    public bool isHurt { get; private set; }
    public bool isDead {  get; private set; }
    private bool isHurtAnimationStart;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>(); 
        Collider = GetComponent<CapsuleCollider>();
    }

    private void OnEnable()
    {
        monsterId = gameObject.GetInstanceID();
        MonsterHeight = Collider.height;

        isDead = false;
        KnockbackDir = Vector3.zero;

        gameObject.layer = LayerMask.NameToLayer("Monster");
        Collider.enabled = true;

        if (vm == null)
        {
            vm = new MonsterViewModel();
            vm.PropertyChanged += OnPropertyChanged;
            vm.RegisterMonsterHPChanged(true, monsterId);
            vm.RegisterMonsterStaminaChanged(true, monsterId);
            vm.RegisterAttackMethodChanged(monsterId, true);
            vm.RegisterTraceTargetChanged(monsterId, true);
        }

        SetMonsterInfo(_type);
        _monsterBTRunner = new MonsterBTRunner(SetMonsterBT());
        _behaviorTree = GetComponent<BehaviorTree>();

        MonsterManager.instance.SpawnMonster(this);

        //디버깅 용
        //vm.TraceTarget = GameObject.FindWithTag("Player").transform;
    }

    private void OnDisable()
    {
        if(vm != null)
        {
            vm.RegisterTraceTargetChanged(monsterId, false);
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

        _monsterData = monster.MonsterDataClone();

        //체력과 스테미너 초기 설정
        vm.RequestMonsterHPChanged(monsterId, _monsterData.HP);
        vm.RequestMonsterStaminaChanged(_monsterData.Stamina, monsterId);

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
                GetAttackMethodCount();
                break;
            case nameof(vm.TraceTarget):
                animator.SetBool("ComBatMode", vm.TraceTarget!= null);
                break;
        }
    }

    private void GetAttackMethodCount()
    {
        switch (vm.CurrentAttackMethod.DataName)
        {
            case nameof(WeaponsType.SpearAttack):
                AttackMethodCount = 1;
                break;
        }
    }

    private void Update()
    {
        UpdateAttackMethod();

        //_monsterBTRunner.Execute();

        //디버깅 용
        //isSubdued = true;

        if (vm.TraceTarget != null)
        {
            if (animator.GetBool("Incapacitated")) return;

            CombatMovementTimer += Time.deltaTime;

            MonsterBattleRotation();
        }
    }

    private void MonsterBattleRotation()
    {
        //Player의 방향으로 회전
        Vector3 direction = vm.TraceTarget.position - transform.position;
        direction.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 3f * Time.deltaTime);
    }

    public void MonsterDead()
    {
        Collider.enabled = false;
        agent.speed = 0;
        agent.ResetPath();

        if (animator.layerCount > 1) animator.SetLayerWeight(1, 0);

        gameObject.layer = LayerMask.NameToLayer("Dead");
        MonsterManager.instance.DeadMonster_Update(this);
    }

    IBTNode SetMonsterBT()
    {
        var DieNodeList = new List<IBTNode>();
        DieNodeList.Add(new ActionNode(CheckIsDeadOnUpdate));
        DieNodeList.Add(new ActionNode(CompleteDieAnimation));
        var DieSeqNode = new SequenceNode(DieNodeList);

        var SubduedNodeList = new List<IBTNode>();
        SubduedNodeList.Add(new ActionNode(Subdued));
        SubduedNodeList.Add(new ActionNode(SubduedTimer));
        var SubduedSeqNode = new SequenceNode(SubduedNodeList);

        var ParryNodeList = new List<IBTNode>();
        ParryNodeList.Add(new ActionNode(ParriedStart));
        ParryNodeList.Add(new ActionNode(CompleteParriedAnimation));
        var ParrySeqNode = new SequenceNode(ParryNodeList);

        var rootNodeList = new List<IBTNode>();
        rootNodeList.Add(DieSeqNode);
        rootNodeList.Add(SubduedSeqNode);
        rootNodeList.Add(ParrySeqNode);
        var rootSelectNode = new SelectorNode(rootNodeList);
        return rootSelectNode;
    }

    #region Die
    
    IBTNode.EBTNodeState CheckIsDeadOnUpdate()
    {
        if (animator.GetBool("Dead"))
        {
            if(animator.GetBool("Incapacitated"))
            {
                isDead = true;
                Collider.enabled = false;
                agent.speed = 0;
                agent.ResetPath();

                if (animator.layerCount > 1) animator.SetLayerWeight(1, 0);

                gameObject.layer = LayerMask.NameToLayer("Dead");
                MonsterManager.instance.DeadMonster_Update(this);
            }
            return IBTNode.EBTNodeState.Success;
        }

        if (!isDead) return IBTNode.EBTNodeState.Fail;

        Collider.enabled = false;

        isHurt = false;
        agent.speed = 0;
        agent.ResetPath();

        if(animator.layerCount > 1) animator.SetLayerWeight(1, 0);

        gameObject.layer = LayerMask.NameToLayer("Dead");
        MonsterManager.instance.DeadMonster_Update(this);

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
    public void Hurt(PlayerView attacker, float Damage)
    {
        if (isSubdued)
        {
            vm.RequestMonsterHPChanged(monsterId, 0);
            _behaviorTree.SetVariableValue("isDead", true);

            //암살 동작 실행
            return;
        }

        //방어 성공
        if(UnityEngine.Random.Range(0f, 100f) <= _monsterData.DefencePer)
        {
            Debug.Log("몬스터 방어 성공");

            animator.SetTrigger("Defense");
            vm.RequestMonsterStaminaChanged(vm.Stamina - attacker.playerData.ATK, monsterId);
            return;
        }

        //체력 감소
        vm.RequestMonsterHPChanged(monsterId, vm.HP - Damage);
        Debug.Log(vm.HP);

        if(vm.HP <= 0)
        {
            _behaviorTree.SetVariableValue("isDead", true);
        }
        else
        {
            _behaviorTree.SetVariableValue("isHurt", true);
            KnockbackDir = transform.position - attacker.transform.position;
            //넉백 구간
        }
    }
    
    #endregion

    #region Subdued
    // Subdued Node 추가
    IBTNode.EBTNodeState Subdued()
    {
        if (isHurt || isDead) return IBTNode.EBTNodeState.Fail;
        if (!isSubdued) return IBTNode.EBTNodeState.Fail;
        if (_subduedTimer > 0) return IBTNode.EBTNodeState.Success;

        animator.SetBool("Incapacitated", true);
        animator.SetTrigger("Incapacitate");

        if (IsAnimationRunning("Subdued"))
        {
            _subduedTimer = 5f;
            return IBTNode.EBTNodeState.Success;
        }

        return IBTNode.EBTNodeState.Running;
    }

    IBTNode.EBTNodeState SubduedTimer()
    {
        if (isHurt || isDead) return IBTNode.EBTNodeState.Fail;
        if (!isSubdued) return IBTNode.EBTNodeState.Fail;

        if(_subduedTimer > 0)
        {
            _subduedTimer -= Time.deltaTime;
            return IBTNode.EBTNodeState.Running;
        }

        animator.SetBool("Incapacitated", false);
        isSubdued = false;
        return IBTNode.EBTNodeState.Success;
    }
    #endregion

    #region Parry
    IBTNode.EBTNodeState ParriedStart() 
    {
        if (!isParried) return IBTNode.EBTNodeState.Fail;
        if (isParryStart) return IBTNode.EBTNodeState.Success;

        animator.SetTrigger("Parried");
        if (IsAnimationRunning("Parried"))
        {
            isParryStart = true;
            return IBTNode.EBTNodeState.Success;
        }
        return IBTNode.EBTNodeState.Running;
    }

    IBTNode.EBTNodeState CompleteParriedAnimation()
    {
        if (!isParried) return IBTNode.EBTNodeState.Fail;

        if (IsAnimationRunning("Parried"))
        {
            return IBTNode.EBTNodeState.Running;
        }

        isParryStart = false;
        isParried = false;
        isSubdued = vm.Stamina <= 0;
        return IBTNode.EBTNodeState.Success;
    }

    public void Parried(PlayerView attacker)
    {
        float addParriedPower;

        if (_type != MonsterType.Boss) addParriedPower = attacker.playerData.Strength * 3;
        else addParriedPower = attacker.playerData.Strength * 10;

        vm.RequestMonsterStaminaChanged(vm.Stamina - addParriedPower, monsterId);
        Debug.Log($"Monster Stamina : {vm.Stamina}");
        isParried = true;
    }
    #endregion

    public void Attack()
    {
        if(!(bool)_behaviorTree.GetVariable("isAttackAble").GetValue()) return;
        if (!(bool)_behaviorTree.GetVariable("isAttacking").GetValue()) _behaviorTree.SetVariableValue("isAttacking", true);
    }

    public bool IsAnimationRunning(string animationName)
    {
        if (animator == null) return false;

        bool isRunning = false;
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName(animationName))
        {
            float normalizedTime = stateInfo.normalizedTime;
            isRunning = normalizedTime > 0 && normalizedTime < 1.0f;
        }

        return isRunning;
    }

    public  bool IsHurtAnimationRunning()
    {
        if (animator == null) return false;

        bool isRunning = false;
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // "Hurt" 상태가 현재 상태인지 확인
        if (stateInfo.IsTag("Hurt"))
        {
            float normalizedTime = stateInfo.normalizedTime;
            isRunning = normalizedTime > 0 && normalizedTime < 1.0f;
        }

        return isRunning;
    }

    public void MoveToTarget(Vector3 targetPos)
    {
        agent.SetDestination(targetPos);
    }

}

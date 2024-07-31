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

    public BehaviorTree _behaviorTree { get; private set; }

    private NavMeshAgent agent;
    public Animator animator { get; private set; }
    private Rigidbody rb;
    private CapsuleCollider Collider;

    public int monsterId { get; private set; }
    public float MonsterHeight { get; private set; }

    #region attack
    public AttackBox attackBox { get; private set; }
    public float CombatMovementTimer { get; private set; }
    public int AttackMethodCount { get; private set; }
    #endregion
    #region Hurt
    public Vector3 KnockbackDir { get; private set; }
    #endregion

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
        _behaviorTree = GetComponent<BehaviorTree>();
        _behaviorTree.SetVariableValue("isDead", false);
        _behaviorTree.SetVariableValue("isAssassinated", false);
        _behaviorTree.SetVariableValue("isDisable", false);

        if (animator.layerCount > 1) animator.SetLayerWeight(1, 1);

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

        ChangedCharacterMesh(type);
        ChangedMonsterAnimationController();
        UpdateAttackMethod_Data(monster);

    }

    private void ChangedCharacterMesh(MonsterType type)
    {
        foreach(var child in monsterMeshes)
        {
            if(child._monsterType == type)
            {
                child.mesh.SetActive(true);
                continue;
            }
            child.mesh.SetActive(false);
        }        
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
                attackBox = weapon.weaponMesh.GetComponentInChildren<AttackBox>();
                attackBox.enabled = false;
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
            case nameof(WeaponsType.DaggerAttack):
                AttackMethodCount = 3;
                break;
            case nameof(WeaponsType.ShurikenAttack):
                AttackMethodCount = 2;
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
            CombatMovementTimer += Time.deltaTime;

            if ((bool)_behaviorTree.GetVariable("isAssassinated").GetValue() || (bool)_behaviorTree.GetVariable("isDead").GetValue() || (bool)_behaviorTree.GetVariable("isHurt").GetValue() || (bool)_behaviorTree.GetVariable("isSubded").GetValue()) return;
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

    public void Parried(PlayerView attacker)
    {
        float addParriedPower;

        if (_type != MonsterType.Boss) addParriedPower = attacker.playerData.Strength * 5;
        else addParriedPower = attacker.playerData.Strength * 10;

        vm.RequestMonsterStaminaChanged(vm.Stamina - addParriedPower, monsterId);
        Debug.Log($"Monster Stamina : {vm.Stamina}");
        _behaviorTree.SetVariableValue("isParried", true);
    }

    public void Hurt(PlayerView attacker, float Damage)
    {
        if ((bool)_behaviorTree.GetVariable("isSubded").GetValue())
        {
            vm.RequestMonsterHPChanged(monsterId, 0);
            _behaviorTree.SetVariableValue("isDead", true);
            return;
        }

        if (UnityEngine.Random.Range(0f, 100f) <= _monsterData.DefencePer)
        {
            Debug.Log("Monster Defense");

            animator.SetTrigger("Defense");
            vm.RequestMonsterStaminaChanged(vm.Stamina - attacker.playerData.ATK, monsterId);
            return;
        }

        vm.RequestMonsterHPChanged(monsterId, vm.HP - Damage);

        if (vm.HP <= 0)
        {
            _behaviorTree.SetVariableValue("isDead", true);
        }
        else
        {
            _behaviorTree.SetVariableValue("isHurt", true);
            KnockbackDir = transform.position - attacker.transform.position;
        }
    }

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
            isRunning = normalizedTime >= 0 && normalizedTime < 1.0f;
        }

        return isRunning;
    }
}

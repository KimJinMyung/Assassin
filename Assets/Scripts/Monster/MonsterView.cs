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

    private MonsterViewModel vm;

    private MonsterData _initMonsterData;
    private List<Monster_Attack> monsterAttackMethodList = new List<Monster_Attack>();

    private MonsterBTRunner _monsterBTRunner;

    private NavMeshAgent agent;
    private Animator animator;
    private Rigidbody rb;

    public int monsterId { get; private set; }

    private bool isDead;

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
                break;
        }
    }

    private void Update()
    {
        _monsterBTRunner.Execute();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            isDead = true;
        }

        Debug.Log(vm.CurrentAttackMethod.DataName);

        UpdateAttackMethod();
        //디버그 용
        //animator.SetTrigger(vm.CurrentAttackMethod.DataName.ToString());
    }

    IBTNode SetMonsterBT()
    {
        var DieNodeList = new List<IBTNode>();
        DieNodeList.Add(new ActionNode(CheckIsDeadOnUpdate));
        DieNodeList.Add(new ActionNode(CompleteDieAnimation));
        var DieSeqNode = new SequenceNode(DieNodeList);

        var rootNodeList = new List<IBTNode>();
        rootNodeList.Add(DieSeqNode);
        var rootSelectNode = new SelectorNode(rootNodeList);
        return rootSelectNode;
    }

    IBTNode.EBTNodeState CheckIsDeadOnUpdate()
    {
        if (!isDead) return IBTNode.EBTNodeState.Fail;
        if (animator.GetBool("Dead")) return IBTNode.EBTNodeState.Success;

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

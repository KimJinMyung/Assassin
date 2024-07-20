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

    private NavMeshAgent agent;
    private Animator animator;
    public int monsterId { get; private set; }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        monsterId = gameObject.GetInstanceID();

        if (vm == null)
        {
            vm = new MonsterViewModel();
            vm.PropertyChanged += OnPropertyChanged;
            vm.RegisterMonsterHPChanged(true, monsterId);
            vm.RegisterMonsterStaminaChanged(true, monsterId);
            vm.RegisterAttackMethodChanged(monsterId, true);
        }

        SetMonsterInfo(_type);
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

        UpdateAttackMethod_Data(monster);
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

    private void Update()
    {

    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    { 
    
    }
}

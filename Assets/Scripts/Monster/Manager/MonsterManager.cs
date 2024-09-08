using EventEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterManager : Singleton<MonsterManager>
{
    private Dictionary<int, MonsterView> _monsterLists = new Dictionary<int, MonsterView>();
    private List<Transform> _lockOnAbleMonsterList = new List<Transform>();
    public List<Transform> LockOnAbleMonsterList { get { return _lockOnAbleMonsterList; } }

    private Dictionary<int, Action> monsterAttackMethodsList = new Dictionary<int, Action>();

    private float _attackingTimer;

    [SerializeField] MonsterUI _mainHud;

    Dictionary<int, Action<float>> _hpChangedCallback = new Dictionary<int, Action<float>>();
    Dictionary<int, Action<float>> _maxHpChangedCallback = new Dictionary<int, Action<float>>();
    Dictionary<int, Action<float>> _staminaChangedCallback = new Dictionary<int, Action<float>>();
    Dictionary<int, Action<float>> _maxStaminaChangedCallback = new Dictionary<int, Action<float>>();
    Dictionary<int, Action<float>> _LifeCountChangedCallback = new Dictionary<int, Action<float>>();

    protected new void Awake()
    {
        base.Awake();

        AddEvent();
    }
     
    private void OnDestroy()
    {
        RemoveEvent();
    }

    private void OnEnable()
    {
        _monsterLists.Clear();
        _lockOnAbleMonsterList.Clear();
        monsterAttackMethodsList.Clear();
        _hpChangedCallback.Clear();
        _maxHpChangedCallback.Clear();
        _staminaChangedCallback.Clear();
        _maxStaminaChangedCallback.Clear();
        _LifeCountChangedCallback.Clear();

        _attackingTimer = UnityEngine.Random.Range(2f, 4f);
    }

    private void AddEvent()
    {
        EventManager<MonsterEvent>.Binding<MonsterView>(true, MonsterEvent.SpawnMonster, SpawnMonster);
        //EventManager<MonsterEvent>.Binding<List<Transform>>(true, MonsterEvent.ChangedLockOnAbleMonsterList, LockOnAbleMonsterListChanged);
    }

    private void RemoveEvent()
    {
        EventManager<MonsterEvent>.Binding<MonsterView>(false, MonsterEvent.SpawnMonster, SpawnMonster);
        //EventManager<MonsterEvent>.Binding<List<Transform>>(false, MonsterEvent.ChangedLockOnAbleMonsterList, LockOnAbleMonsterListChanged);
    }

    public void SetHUD(MonsterUI uI)
    {
        this._mainHud = uI;
    }

    #region MonsterList
    public void SpawnMonster(MonsterView monster)
    {
        if (_monsterLists.ContainsKey(monster.monsterId)) return;

        _monsterLists.Add(monster.monsterId, monster);

        if (monster.Type != MonsterType.Boss)
        {
            CreateMonsterHUD(monster);
        }
        else CreateBossMonsterHUD(monster);

        RegisterMonsgterAttackMethod(monster.monsterId, monster.Attack, true);
    }

    public void LockOnAbleMonsterListChanged(List<Transform> monster)
    {
        if (_lockOnAbleMonsterList.Equals(monster)) return;

        _lockOnAbleMonsterList = monster;

        EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.ChangedLockOnIconEnableList, _lockOnAbleMonsterList);
    }

    public void DeadMonster_Update(MonsterView monster)
    {
        if (_monsterLists.ContainsKey(monster.monsterId))
        {
            if (monster.Type == MonsterType.Boss)  ShowBossMonsterHUD_OnOff(false);
            else _mainHud.OffMonsterHUD(monster);
            
            _monsterLists.Remove(monster.monsterId);
            RegisterMonsgterAttackMethod(monster.monsterId, monster.Attack, false);
        }
    }

    public void ShowBossMonsterHUD_OnOff(bool onoff)
    {
        if (_mainHud == null) return;

        _mainHud.BossMonsterHud_OnOff(onoff);
    }

    private void CreateBossMonsterHUD(MonsterView boss)
    {
        if (_mainHud == null) return;

        _mainHud.BindBossMonster(boss);
        ShowBossMonsterHUD_OnOff(true);
    }

    private void CreateMonsterHUD(MonsterView mob)
    {
        if (_mainHud == null)
            return;

        _mainHud.CreateMonsterHUD(mob);
    }

    public bool CheckMonsterList(MonsterView monster)
    {
        return _monsterLists.ContainsKey(monster.monsterId);
    }

    private void Update()
    {
        if (_monsterLists.Count <= 0) return;

        if (IsAttackAble())
        {
            if (_attackingTimer > 0) _attackingTimer -= Time.deltaTime;
            else
            {
                //한마리만 Attack
                var attackMonster = SelectMonsterForAttack();

                if (attackMonster == null) return;

                _attackingTimer = UnityEngine.Random.Range(2f, 4f);
                //몬스터 공격 수행
                if (monsterAttackMethodsList.ContainsKey(attackMonster.monsterId))
                {
                    monsterAttackMethodsList[attackMonster.monsterId]?.Invoke();
                }
            }
        }
    }

    private bool IsAttackAble()
    {
        if (_monsterLists.Count <= 0) return false;

        bool hasTarget = false;

        foreach (var monster in _monsterLists.Values)
        {
            if (monster.Type == MonsterType.Boss) continue;
            if (monster != null && monster.vm.TraceTarget != null)
            {
                hasTarget = true;

                //현재 공격 중인지 확인
                var isAttackAbleVeriable = monster._behaviorTree.GetVariable("isAttackAble");
                if (!(bool)isAttackAbleVeriable.GetValue()) return false;
            }

        }

        if (!hasTarget) return false;

        return true;
    }

    MonsterView SelectMonsterForAttack()
    {
        List<MonsterView> monsterList = new List<MonsterView>();

        foreach (var monster in _monsterLists.Values)
        {
            MonsterView newMonster = monster.GetComponent<MonsterView>();
            if (newMonster.Type == MonsterType.Boss) continue;

            //isAttackAble 이 아니면 continue
            var monsterBT = newMonster._behaviorTree;
            if (!(bool)monsterBT.GetVariable("isAttackAble").GetValue() || newMonster.animator.GetBool("Incapacitated") || (bool)monsterBT.GetVariable("isHurt").GetValue()) continue;

            Transform target = newMonster.vm.TraceTarget;
            if (target != null)
            {
                Vector3 targetDir = (target.position - newMonster.transform.position).normalized;
                float Angle = Vector3.Angle(newMonster.transform.forward, targetDir);
                if (Angle <= 45f) monsterList.Add(newMonster);
            }
        }

        return monsterList.OrderByDescending(e => e.CombatMovementTimer).FirstOrDefault();
    }
    #endregion
    #region MonsterAttackMethod
    private Dictionary<int, Action<List<Monster_Attack>, MonsterView>> _monsterAttackMethodList = new Dictionary<int, Action<List<Monster_Attack>, MonsterView>>();

    public void RegisterAttackMethodChangedCallback(int monsterId, Action<List<Monster_Attack>, MonsterView> AttackMethodChangedCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (!_monsterAttackMethodList.ContainsKey(monsterId)) _monsterAttackMethodList.Add(monsterId, AttackMethodChangedCallback);
            else _monsterAttackMethodList[monsterId] = AttackMethodChangedCallback;
        }
        else
        {
            if (_monsterAttackMethodList.ContainsKey(monsterId))
            {
                _monsterAttackMethodList[monsterId] -= AttackMethodChangedCallback;
                if (_monsterAttackMethodList[monsterId] == null) _monsterAttackMethodList.Remove(monsterId);
            }
        }
    }

    public void OnAttackMethodChanged(int actorId, List<Monster_Attack> attackList, MonsterView owner)
    {
        if (_monsterAttackMethodList.ContainsKey(actorId)) _monsterAttackMethodList[actorId]?.Invoke(attackList, owner);
    }
    #endregion
    #region MonsterTraceTarget
    private Dictionary<int, Action<Transform>> _traceTargetChangedCallback = new Dictionary<int, Action<Transform>>();
    public void RegisterTraceTargetChangedCallback(Action<Transform> TraceTargetChangedCallback, int actorId, bool isRegister)
    {
        if (isRegister)
        {
            if (isRegister)
            {
                if (_traceTargetChangedCallback.ContainsKey(actorId)) _traceTargetChangedCallback[actorId] = TraceTargetChangedCallback;
                else _traceTargetChangedCallback.Add(actorId, TraceTargetChangedCallback);
            }
            else
            {
                if (_traceTargetChangedCallback.ContainsKey(actorId))
                {
                    _traceTargetChangedCallback[actorId] -= TraceTargetChangedCallback;
                    if (_traceTargetChangedCallback[actorId] == null) _traceTargetChangedCallback.Remove(actorId);
                }
            }
        }
    }

    public void OnTraceTarget(int actorId, Transform target)
    {
        if (_traceTargetChangedCallback.ContainsKey(actorId)) _traceTargetChangedCallback[actorId]?.Invoke(target);
    }
    #endregion

    public void RegisterMonsgterAttackMethod(int monsterId, Action attackMethod, bool isRegister)
    {
        if(isRegister)
        {
            if(monsterAttackMethodsList.ContainsKey(monsterId)) monsterAttackMethodsList[monsterId] = attackMethod;
            else monsterAttackMethodsList.Add(monsterId, attackMethod);
        }
        else
        {
            if (monsterAttackMethodsList.ContainsKey(monsterId))
            {
                monsterAttackMethodsList[monsterId] -= attackMethod;
                if (monsterAttackMethodsList[monsterId] == null) monsterAttackMethodsList.Remove(monsterId);
            }
        }
    }

    public void RegisterMonsterHPChangedCallback(int monsterId, Action<float> monsterHpCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (_hpChangedCallback.ContainsKey(monsterId)) _hpChangedCallback[monsterId] = monsterHpCallback;
            else _hpChangedCallback.Add(monsterId, monsterHpCallback);
        }
        else
        {
            if (_hpChangedCallback.ContainsKey(monsterId))
            {
                _hpChangedCallback[monsterId] -= monsterHpCallback;
                if (_hpChangedCallback[monsterId] == null) _hpChangedCallback.Remove(monsterId);
            }
        }
    }

    public void RegisterMonsterMaxHPChangedCallback(int monsterId, Action<float> monstermaxHpCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (_maxHpChangedCallback.ContainsKey(monsterId)) _maxHpChangedCallback[monsterId] = monstermaxHpCallback;
            else _maxHpChangedCallback.Add(monsterId, monstermaxHpCallback);
        }
        else
        {
            if (_maxHpChangedCallback.ContainsKey(monsterId))
            {
                _maxHpChangedCallback[monsterId] -= monstermaxHpCallback;
                if (_maxHpChangedCallback[monsterId] == null) _maxHpChangedCallback.Remove(monsterId);
            }
        }
    }

    public void RegisterMonsterStaminaChangedCallback(int monsterId, Action<float> monsterStaminaCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (_staminaChangedCallback.ContainsKey(monsterId)) _staminaChangedCallback[monsterId] = monsterStaminaCallback;
            else _staminaChangedCallback.Add(monsterId, monsterStaminaCallback);
        }
        else
        {
            if (_staminaChangedCallback.ContainsKey(monsterId))
            {
                _staminaChangedCallback[monsterId] -= monsterStaminaCallback;
                if (_staminaChangedCallback[monsterId] == null) _staminaChangedCallback.Remove(monsterId);
            }
        }
    }
    public void RegisterMonsterMaxStaminaChangedCallback(int monsterId, Action<float> monsterMaxStaminaCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (_maxStaminaChangedCallback.ContainsKey(monsterId)) _maxStaminaChangedCallback[monsterId] = monsterMaxStaminaCallback;
            else _maxStaminaChangedCallback.Add(monsterId, monsterMaxStaminaCallback);
        }
        else
        {
            if (_maxStaminaChangedCallback.ContainsKey(monsterId))
            {
                _maxStaminaChangedCallback[monsterId] -= monsterMaxStaminaCallback;
                if (_maxStaminaChangedCallback[monsterId] == null) _maxStaminaChangedCallback.Remove(monsterId);
            }
        }
    }

    public void RegisterMonsterLifeCountChangedCallback(int monsterId, Action<float> monsterMaxStaminaCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (_LifeCountChangedCallback.ContainsKey(monsterId)) _LifeCountChangedCallback[monsterId] = monsterMaxStaminaCallback;
            else _LifeCountChangedCallback.Add(monsterId, monsterMaxStaminaCallback);
        }
        else
        {
            if (_LifeCountChangedCallback.ContainsKey(monsterId))
            {
                _LifeCountChangedCallback[monsterId] -= monsterMaxStaminaCallback;
                if (_LifeCountChangedCallback[monsterId] == null) _LifeCountChangedCallback.Remove(monsterId);
            }
        }
    }

    public void SetMonsterHP(int monsterId, float HP)
    {
        _hpChangedCallback[monsterId]?.Invoke(HP);
    }
    public void SetMonsterMaxHP(int monsterId, float MaxHP)
    {
        _maxHpChangedCallback[monsterId]?.Invoke(MaxHP);
    }
    public void SetMonsterStamina(int monsterId, float Stamina)
    {
        _staminaChangedCallback[monsterId]?.Invoke(Stamina);
    }
    public void SetMonsterMaxStamina(int monsterId, float MaxStamina)
    {
        _maxStaminaChangedCallback[monsterId]?.Invoke(MaxStamina);
    }
    public void SetMonsterLifeCount(int monsterId, float Life)
    {
        _LifeCountChangedCallback[monsterId]?.Invoke(Life);
    }
}

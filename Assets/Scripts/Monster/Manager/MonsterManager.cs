using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public static MonsterManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this.gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private Dictionary<int, MonsterView> _monsterLists = new Dictionary<int, MonsterView>();
    private List<Transform> _lockOnAbleMonsterList = new List<Transform>();
    public List<Transform> LockOnAbleMonsterList { get { return _lockOnAbleMonsterList; } }

    private Dictionary<int, Action> monsterAttackMethodsList = new Dictionary<int, Action>();

    private float _attackingTimer;

    #region MonsterList
    public void SpawnMonster(MonsterView monster)
    {
        if (_monsterLists.ContainsKey(monster.monsterId)) return;

        _monsterLists.Add(monster.monsterId, monster);
        RegisterMonsgterAttackMethod(monster.monsterId, monster.Attack, true);
    }

    public void LockOnAbleMonsterListChanged(List<Transform> monster)
    {
        if (_lockOnAbleMonsterList.Equals(monster)) return;

        _lockOnAbleMonsterList = monster;
    }

    public void DeadMonster_Update(MonsterView monster)
    {
        if (_monsterLists.ContainsKey(monster.monsterId))
        {
            _monsterLists.Remove(monster.monsterId);
            RegisterMonsgterAttackMethod(monster.monsterId, monster.Attack, false);
        }
    }

    public bool CheckMonsterList(MonsterView monster)
    {
        return _monsterLists.ContainsKey(monster.monsterId);
    }

    private void OnEnable()
    {
        _attackingTimer = UnityEngine.Random.Range(2f, 4f);
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
}

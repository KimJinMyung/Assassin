using EventEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] List<Transform> monstersList;

    private Coroutine hashCoroutine;

    private void Awake()
    {
        EventManager<MonsterEvent>.Binding(true, MonsterEvent.SpawnBossMonster, BossSpawn);
    }

    private void Start()
    {
        hashCoroutine = StartCoroutine(spawnStart());
    }

    private void OnDestroy()
    {
        StopCoroutine(hashCoroutine);

        EventManager<MonsterEvent>.Binding(false, MonsterEvent.SpawnBossMonster, BossSpawn);
    }

    private IEnumerator spawnStart()
    {
        while (true)
        {
            foreach (var monster in monstersList)
            {
                var monsterView = monster.GetComponent<MonsterView>();
                if (monsterView == null) continue;
                if (monsterView.isDead) continue;

                var distance = Vector3.Distance(player.position, monster.position);

                if (distance <= 20f)
                {
                    if (monster.gameObject.activeSelf) continue;
                    monster.gameObject.SetActive(true);
                    monsterView.Recovery();
                }
                else
                {
                    if (!monster.gameObject.activeSelf) continue;
                    monster.gameObject.SetActive(false);
                    monsterView.MonsterDead();
                }
            }

            yield return null;
        }
    }

    private void BossSpawn()
    {
        foreach(var monster in monstersList)
        {
            var monsterView = monster.GetComponent<MonsterView>();
            if (monsterView == null) continue;
            if (monsterView.isDead) continue;

            if (!monster.gameObject.activeSelf) continue;
            monster.gameObject.SetActive(false);
            monsterView.MonsterDead();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] List<Transform> monstersList;

    private void Update()
    {
        foreach(var monster in monstersList)
        {
            var monsterView = monster.GetComponent<MonsterView>();
            if (monsterView == null) continue;
            if (monsterView.isDead) continue;

            var distance = Vector3.Distance(player.position, monster.position);
            if(distance <= 20f)
            {
                if (monster.gameObject.activeSelf) return;
                monster.gameObject.SetActive(true);
            }
            else
            {
                if (!monster.gameObject.activeSelf) return;
                monster.gameObject.SetActive(false);
            }
        }
    }
}

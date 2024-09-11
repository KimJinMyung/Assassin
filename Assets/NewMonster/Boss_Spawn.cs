using EventEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Spawn : MonoBehaviour
{
    [SerializeField] private GameObject BossMonster;

    [SerializeField] private List<Collider> BossZoneWalls;
    [SerializeField] private List<GameObject> RopePoint;

    private Collider collider;

    private bool isSpawn;

    private void Awake()
    {
        collider = GetComponent<Collider>();
    }

    private void Start()
    {
        isSpawn = false;

        foreach (var wall in BossZoneWalls)
        {
            wall.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isSpawn)
        {
            isSpawn = true;
            collider.enabled = false;

            foreach (var wall in  BossZoneWalls)
            {
                wall.enabled = true;
            }

            foreach (var wall in RopePoint)
            {
                wall.SetActive(false);
            }

            BossMonster.SetActive(true);
            EventManager<MonsterEvent>.TriggerEvent(MonsterEvent.SpawnBossMonster);
        }
    }
}

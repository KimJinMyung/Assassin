using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("Idle")]
public class SearchPatrolEndPosition : Action
{
    private MonsterView view;

    [SerializeField] SharedVector3 patrolPos;
    [SerializeField] LayerMask GroundLayer;

    public override void OnAwake()
    {
        view = Owner.GetComponent<MonsterView>();
    }

    public override TaskStatus OnUpdate()
    {
        patrolPos.Value = RandomPatrolEndPosition(Owner.transform.position, Random.Range(0f, view._monsterData.ViewRange));

        if (patrolPos.Value != Vector3.zero)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    private Vector3 RandomPatrolEndPosition(Vector3 originPosition, float distance)
    {
        Vector3 randomPoint = originPosition + UnityEngine.Random.insideUnitSphere * distance;
        Vector3 patrolPos = Vector3.zero;

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

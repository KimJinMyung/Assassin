using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[TaskCategory("Monster_Patrol")]
public class RandomPatrolPositionOnUpdate : Action
{
    private MonsterView monsterView;
    private Animator _animator;

    private Vector3 patrolPos;

    private LayerMask GroundLayer;

    public override void OnAwake()
    {
        base.OnAwake();
        monsterView = Owner.GetComponent<MonsterView>();
        _animator = monsterView.GetComponent<Animator>();

        GroundLayer = LayerMask.GetMask("Default", "MonsterWalkAble");
    }

    public override TaskStatus OnUpdate()
    {
        if (monsterView.isDead || monsterView.isHurt) return TaskStatus.Failure;
        if (monsterView.vm.TraceTarget != null) return TaskStatus.Failure;
        if (monsterView.isAttacking) return TaskStatus.Failure;

        if (Vector3.Distance(transform.position, patrolPos) > 0.1f) return TaskStatus.Success;

        if (RandomPatrolEndPosition(transform.position, monsterView._monsterData.ViewRange) != Vector3.zero)
        {
            return TaskStatus.Success;
        }

        return TaskStatus.Running;
    }

    private Vector3 RandomPatrolEndPosition(Vector3 originPosition, float distance)
    {
        Vector3 randomPoint = originPosition + UnityEngine.Random.insideUnitSphere * distance;

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

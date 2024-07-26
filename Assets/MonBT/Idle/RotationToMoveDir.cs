using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TaskCategory("Idle")]
public class RotationToMoveDir : Action
{
    private Animator animator;

    [SerializeField] SharedVector3 patrolPos;

    public override void OnAwake()
    {
        animator = Owner.GetComponent<Animator>();
    }

    public override TaskStatus OnUpdate()
    {
        Vector3 dir = (patrolPos.Value - Owner.transform.position);
        dir.y = 0f;
        dir.Normalize();

        Quaternion targetRotation = Quaternion.LookRotation(dir);

        float angle = Vector3.Angle(Owner.transform.forward, dir);

        float rotationDirection = Vector3.SignedAngle(Owner.transform.forward, dir, Vector3.up) > 0 ? 1f : -1f;

        if (angle > 20f)
        {
            //patrolPos 방향으로 회전
            Owner.transform.rotation = Quaternion.RotateTowards(Owner.transform.rotation, targetRotation, 120f * Time.deltaTime);

            animator.SetBool("Rotate", true);
            animator.SetFloat("Rotation", rotationDirection);
            return TaskStatus.Running;
        }

        animator.SetBool("Rotate", false);
        animator.SetFloat("Rotation", 0);
        return TaskStatus.Success;
    }
}

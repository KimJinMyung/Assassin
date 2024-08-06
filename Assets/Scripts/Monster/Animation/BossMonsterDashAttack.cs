using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonsterDashAttack : StateMachineBehaviour
{
    private Transform Owner;
    private Rigidbody rb;

    [SerializeField] private float dashPower;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Owner = animator.transform;
        rb = animator.GetComponent<Rigidbody>();

        rb.AddForce(Owner.transform.forward * dashPower, ForceMode.Impulse);
    }   
}

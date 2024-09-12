using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("BossAttack")]
public class BossMonsterAttackDelay : Action
{
    MonsterView monsterView;

    [SerializeField] SharedFloat AttackTimer;   
    [SerializeField] SharedBool isAttacking;

    public override void OnAwake()
    {
        monsterView = Owner.GetComponent<MonsterView>();
    }

    public override void OnStart()
    {
        var target = monsterView.vm.TraceTarget;
        if (target != null)
        {
            var distance = Vector3.Distance(Owner.transform.position, target.transform.position);   
            if(distance < 3f)
            {
                AttackTimer.Value = Random.Range(1f, 2.5f);
                return;
            }
        }

        AttackTimer.Value = Random.Range(1f, 10f);
    }

    public override TaskStatus OnUpdate()
    {
        isAttacking.Value = false;
        return TaskStatus.Success;
    }
}

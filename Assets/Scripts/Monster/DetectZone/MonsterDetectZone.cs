using EventEnum;
using UnityEngine;

public class MonsterDetectZone : MonoBehaviour
{
    private Transform player;
    private MonsterView owner;

    [SerializeField] private Transform Eyes;

    private SphereCollider collider;
    private void Awake()
    {
        owner = transform.parent.GetComponent<MonsterView>();
        collider = GetComponent<SphereCollider>();

        EventManager<MonsterEvent>.Binding(true, MonsterEvent.SetDetectRange, DefaultDetectRange);
    }

    private void OnDestroy()
    {
        EventManager<MonsterEvent>.Binding(false, MonsterEvent.SetDetectRange, DefaultDetectRange);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
    }

    public void DefaultDetectRange()
    {
        collider.radius = owner._monsterData.ViewRange;
    }

    private void OnTriggerExit(Collider other)
    {
        if (owner.Type == MonsterType.Boss) return;

        if (other.CompareTag("Player"))
        {
            if (Vector3.Distance(other.transform.position, transform.position) > collider.radius)
            {
                player = null;
                owner.vm.RequestTraceTargetChanged(owner.monsterId, player);
            }
        }
    }

    private void Update()
    {
        if (collider.transform.position != Eyes.position)
            collider.transform.position = Eyes.position;

        if (owner.vm.TraceTarget != null) collider.radius = owner._monsterData.ViewRange + 3f;
        else if (!collider.radius.Equals(owner._monsterData.ViewRange)) DefaultDetectRange();

    }

    private void FixedUpdate()
    {
        Detecting();
    }

    private void Detecting()
    {
        if (player == null) return;

        Vector3 playerDir = (player.transform.position - transform.position).normalized;
        float angleMonAndPlayer = Vector3.Angle(Eyes.forward, playerDir);

        float viewAngle = owner._monsterData.ViewAngel;

        if (owner.vm.TraceTarget == null)
        {
            if (angleMonAndPlayer < viewAngle / 2f)
            {
                owner.vm.RequestTraceTargetChanged(owner.monsterId, player);
            }
        }
    }
}

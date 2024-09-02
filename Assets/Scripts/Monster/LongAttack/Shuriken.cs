using UnityEngine;
using Player;

public class Shuriken : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float AddForce;

    public MonsterView owner { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        rb.AddForce(transform.forward * AddForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner.gameObject) return;
        //데미지 부여

        if (other.CompareTag("Player"))
        {
            PlayerView hitTarget = other.GetComponent<PlayerView>();
            hitTarget.Hurt(owner, owner._monsterData.ATK);
            Destroy(this.gameObject);
        }        
    }

    public void SetShooterData(MonsterView monster)
    {
        owner = monster;
    }
}

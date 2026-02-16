using Pathfinding.BehaviorTrees;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{

    private NavMeshAgent agent;

    protected BehaviorTree bTreeRoot;
    protected RandomSelector randS;

    protected Rigidbody rb;

    Animator EnemyAnimator;

    [SerializeField] private float viewDistance = 10f;

    [SerializeField] private Transform target;
    [SerializeField] private int _HP = 5;
    [SerializeField] private GameObject corpse;


    protected virtual void Attacking() {}

    protected void Death()
    {
        // Handle enemy death logic here
        if (corpse != null)
        {
            Instantiate(corpse, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }

    void Awake()
    {
        EnemyAnimator = GetComponentInChildren<Animator>();
        agent = GetComponentInChildren<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) target = playerObj.transform;


        bTreeRoot = new BehaviorTree("TreeRoot");
        randS = new RandomSelector("RandomSelector");

        bTreeRoot.AddChild(randS);


        Sequence SQ_Death = new Sequence("SQ_Death");
        SQ_Death.AddChild(new Leaf("L_DeathCheck", new ConditionNode(() => _HP <= 0)));
        SQ_Death.AddChild(new Leaf("L_DeathAction", new NodeAction(() => Death())));
        randS.AddChild(SQ_Death);

        Sequence SQ_PlayerDetected = new Sequence("SQ_PlayerDetected",30);
        SQ_PlayerDetected.AddChild(new Leaf("L_PlayerDetectionCheck", new ConditionNode(() => CanSeePlayer())));
        randS.AddChild(SQ_PlayerDetected);


        Leaf L_MoveToPlayer = new Leaf("L_MoveToPlayer", new NodeAction(() => Moving(target.transform.position)));


    }

    void Update()
    {


    }

    protected bool CanSeePlayer()
    {
        

        return true;

    }

    void Moving(Vector3 dest)
    {
        

    }

    void ShootProjectile(float range)
    {
        //Instantiate(EnemySO.Projectile, 생성할 좌표, 회전 값 등등...)


    }

    void ShootRay(float range)
    {
        


    }

}

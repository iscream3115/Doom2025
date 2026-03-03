using Pathfinding.BehaviorTrees;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{

    private NavMeshAgent agent;
    private Transform target;

    protected BehaviorTree bTreeRoot;
    protected PrioritySelector ps;

    protected RandomSelector rs;

    protected Rigidbody rb;

    Animator EnemyAnimator;

    [Header("Detection")]
    [SerializeField] private float viewDistance = 10f;

    [Space(8)]
    [Header("Movement")]
    [SerializeField] private float lookRotateSpeed = 8f;
    [SerializeField] private float moveDecisionInterval = 0.8f;
    [SerializeField] private float moveStepDistance = 2.5f;

    [Space(8)]
    [Header("Combat")]
    [SerializeField] private float meleeAttackRange = 2f;
    [SerializeField] private float rangedAttackRange = 7f;
    [SerializeField] private string attackStateTag = "Attack";

    [Space(8)]
    [Header("Debug")]
    [SerializeField] private bool enableProcessNodeDebugLog = false;
    [SerializeField] private bool drawSightGizmos = true;

    [Space(8)]
    [Header("Stats")]
    [SerializeField] private int _HP = 5;
    [SerializeField] private GameObject corpse;

    private string lastLoggedProcessNodeName;

    private enum MoveRoute
    {
        Forward,
        StrafeLeft,
        StrafeRight
    }

    private MoveRoute currentMoveRoute;
    private float nextMoveRouteChangeTime;

    protected NavMeshAgent Agent => agent;
    protected Transform Target => target;
    protected float MeleeAttackRange => meleeAttackRange;
    protected float RangedAttackRange => rangedAttackRange;

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
        ps = new PrioritySelector("PS_PrioritySelector",60);
        bTreeRoot.AddChild(ps);


        Sequence SQ_Death = new Sequence("SQ_Death",50);
        SQ_Death.AddChild(new Leaf("L_DeathCheck", new ConditionNode(() => _HP <= 0)));
        SQ_Death.AddChild(new Leaf("L_DeathAction", new NodeAction(() => Death())));
        ps.AddChild(SQ_Death);

        Sequence SQ_PlayerDetected = new Sequence("SQ_PlayerDetected",40);
        SQ_PlayerDetected.AddChild(new Leaf("L_PlayerDetectionCheck", new ConditionNode(() => CanSeePlayer())));
        ps.AddChild(SQ_PlayerDetected);

        Sequence SQ_Chase = new Sequence("SQ_Chase");
        SQ_Chase.AddChild(new Leaf("L_MoveToPlayer", new NodeAction(() => Moving())));

        Sequence SQ_AttackPlayer = new Sequence("SQ_AttackPlayer");
        SQ_AttackPlayer.AddChild(new Leaf("L_AttackCheck", new ConditionNode(() => CanAttackPlayer())));
        SQ_AttackPlayer.AddChild(new Leaf("L_Attack", new NodeAction(() => Attack())));

        RandomSelector rs = new RandomSelector("rs");
        rs.AddChild(SQ_Chase);
        rs.AddChild(SQ_AttackPlayer);
        SQ_PlayerDetected.AddChild(rs);



    }

    void Update()
    {
        if (bTreeRoot == null) return;

        bTreeRoot.Process();

        if (!enableProcessNodeDebugLog) return;

        Node currentNode = Node.LastProcessingNode;
        if (currentNode == null) return;

        if (lastLoggedProcessNodeName == currentNode.name) return;

        lastLoggedProcessNodeName = currentNode.name;
        Debug.Log($"[{name}] Process Node: {currentNode.name}", this);

    }

    protected bool CanSeePlayer()
    {
        if (target == null) return false;

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 toTarget = target.position - origin;
        float distance = toTarget.magnitude;

        if (distance > viewDistance) return false;

        Vector3 direction = toTarget.normalized;
        if (Physics.Raycast(origin, direction, out RaycastHit hit, viewDistance))
        {
            return hit.transform == target || hit.transform.IsChildOf(target);
        }

        return false;

    }

    protected bool CanAttackPlayer()
    {
        //원거리 공격, 근거리 공격을 위한 사거리를 지정하여 그 안에 플레이어가 들어오면 공격 가능 체크
        //추가적으로 이미 공격을 하고 있는 경우 공격을 할 수 없음. (공격 판단은 행동 사이클 하나가 끝나고 할 수 있다는 소리)
        if (target == null || agent == null) return false;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;
        float distance = toTarget.magnitude;

        bool inMeleeRange = distance <= meleeAttackRange;
        bool inRangedRange = distance <= rangedAttackRange;
        if (!inMeleeRange && !inRangedRange) return false;

        if (EnemyAnimator != null)
        {
            AnimatorStateInfo stateInfo = EnemyAnimator.GetCurrentAnimatorStateInfo(0);
            bool isAttackState = !string.IsNullOrEmpty(attackStateTag) && stateInfo.IsTag(attackStateTag);
            if (isAttackState) return false;
        }

        return true;
    }

    void Moving()
    {
        //이동 매커니즘
        //플레이어를 바라보는 상태에서 플레이어를 향해 가기(전방),측면으로 이동(좌측or 우측) 세가지 선택지 중 하나가 있음
        //적 AI는 이 세가지 루트 중 랜덤한 하나의 루트를 골라서 행동
        if (target == null || agent == null) return;

        Vector3 toTarget = target.position - transform.position;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude <= 0.0001f) return;

        Quaternion lookRotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * lookRotateSpeed);

        if (Time.time >= nextMoveRouteChangeTime)
        {
            currentMoveRoute = (MoveRoute)Random.Range(0, 3);
            nextMoveRouteChangeTime = Time.time + moveDecisionInterval;
        }

        Vector3 forward = toTarget.normalized;
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

        Vector3 moveDir = currentMoveRoute switch
        {
            MoveRoute.Forward => forward,
            MoveRoute.StrafeLeft => -right,
            _ => right
        };

        Vector3 desiredPos = transform.position + (moveDir * moveStepDistance);
        agent.SetDestination(desiredPos);

    }

    protected virtual void Attack() {}

    void OnDrawGizmosSelected()
    {
        if (!drawSightGizmos) return;

        Vector3 origin = transform.position + Vector3.up * 0.5f;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(origin, viewDistance);

        if (target == null) return;

        Vector3 toTarget = target.position - origin;
        float targetDistance = toTarget.magnitude;

        if (targetDistance <= 0.0001f)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(origin, 0.1f);
            return;
        }

        Vector3 direction = toTarget / targetDistance;
        float rayDistance = Mathf.Min(viewDistance, targetDistance);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance))
        {
            bool hitPlayer = hit.transform == target || hit.transform.IsChildOf(target);

            Gizmos.color = hitPlayer ? Color.green : Color.red;
            Gizmos.DrawLine(origin, hit.point);
            Gizmos.DrawWireSphere(hit.point, 0.08f);
        }
        else
        {
            Gizmos.color = targetDistance <= viewDistance ? Color.yellow : new Color(1f, 0.5f, 0f);
            Gizmos.DrawLine(origin, origin + direction * rayDistance);
        }
    }

}

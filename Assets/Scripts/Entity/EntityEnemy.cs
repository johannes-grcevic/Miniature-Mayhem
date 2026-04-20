using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class EntityEnemy : Entity
{
    public static readonly int IsDeadHash = Animator.StringToHash("isDead");
    public static readonly int IsSearchingHash = Animator.StringToHash("isSearching");
    public static readonly int AttackHash = Animator.StringToHash("attack");
    public static readonly int MoveSpeedHash = Animator.StringToHash("moveSpeed");
    public static readonly int TauntHash = Animator.StringToHash("taunt");

    public static readonly string HitFrontStateName = "GetHitFront";
    public static readonly string HitBackStateName = "GetHitBack";

    [SerializeField]
    private int damage = 20;

    [SerializeField]
    private float chaseRange = 10f;

    [SerializeField]
    private float despawnDelay = 10f;

    private Animator animator;
    private NavMeshAgent agent;

    private bool isSearching = false;
    private bool isAttacking = false;

    public override void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        base.Awake();
    }

    void Update()
    {
        isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");

        if (PathComplete() && !isAttacking)
        {
            animator.SetTrigger(AttackHash);
        }
    }

    void OnAnimatorMove()
    {
        SearchForTarget(agent.transform.position);

        animator.SetFloat(MoveSpeedHash, agent.velocity.magnitude);
        animator.SetBool(IsSearchingHash, isSearching);
    }

    // called by an animation event on the enemy
    public void AttackTarget()
    {
        GameManager.Instance.GetPlayer().TakeDamage(damage);
    }

    public void PlayAnimation(string stateName, int layer)
    {
        if (animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName))
        {
            return;
        }

        animator.Play(stateName, layer);
    }

    public override void Die()
    {        
        agent.isStopped = true;
        agent.speed = 0f;

        // trigger death animation
        animator.SetBool(IsDeadHash, true);

        Destroy(gameObject, despawnDelay);

        base.Die();
    }

    public void SearchForTarget(Vector3 targetPosition)
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // lose focus if the target is far away
        isSearching = distanceToTarget > chaseRange;
    }

    public bool PathComplete()
    {
        if (!agent.hasPath) return false;

        return Vector3.Distance(agent.destination, agent.transform.position) <= agent.stoppingDistance;
    }
}

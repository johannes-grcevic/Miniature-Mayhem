using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AgentDestinationController))]
[RequireComponent(typeof(LookAtTarget))]
public class EntityEnemy : Entity
{
    public static readonly int MotionSpeedHash = Animator.StringToHash("MotionSpeed");
    public static readonly int IsDeadHash = Animator.StringToHash("IsDead");
    public static readonly int AttackHash = Animator.StringToHash("Attack");

    public const string ATTACK_STATE_TAG = "Attack";
    public const string HIT_FRONT_STATE_NAME = "GetHitBack";
    public const string HIT_BACK_STATE_NAME = "GetHitFront";

    public bool CanAttack => canAttackTarget;
    public float CurrentSpeed => currentSpeed;
    public float CurrentPlayerDistance => currentPlayerDistance;
    public NavMeshAgent Agent => agent;
    public AgentDestinationController DestinationController => destinationController;
    public LookAtTarget LookAtTarget => lookAtTarget;

    [SerializeField]
    private int damage = 20;

    [SerializeField]
    private float attackRange = 1f;

    [SerializeField]
    private float despawnDelay = 5f;

    [SerializeField]
    private float massOnDeath = 50f;

    [SerializeField, Header("Audio")]
    private AudioClip[] idleClips;

    [SerializeField, Tooltip("Idle sound repeat rate in seconds")]
    private float idleRepeatRate = 30f;

    [SerializeField]
    private AudioClip[] painClips;

    [SerializeField]
    private AudioClip[] attackClips;

    [SerializeField]
    private AudioClip[] deathClips;

    [SerializeField, Range(0f, 1f)]
    private float volume = 1f;

    private NavMeshAgent agent;
    private AgentDestinationController destinationController;
    private LookAtTarget lookAtTarget;

    private bool canAttackTarget = false;
    private float currentSpeed;
    private float currentPlayerDistance;

    protected override void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        destinationController = GetComponent<AgentDestinationController>();
        lookAtTarget = GetComponent<LookAtTarget>();

        base.Awake();
    }

    protected override void Start()
    {
        if (attackRange < agent.stoppingDistance)
        {
            Debug.LogWarning($"[{this}] Attack range should not be less than the agent stopping distance. Setting attack range to {agent.stoppingDistance}.");
            attackRange = agent.stoppingDistance;
        }
        
        base.Start();
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(PlayIdleClip), 0f, Mathf.Max(0.1f, idleRepeatRate));
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(PlayIdleClip));
    }

    private void Update()
    {
        if (IsDead) return;

        // Make the target look in the direction its heading
        lookAtTarget.SetLookTarget(agent.destination);

        // Cache the magnitude once per frame to avoid calculating it twice
        currentSpeed = agent.velocity.magnitude;
        currentPlayerDistance = Vector3.Distance(transform.position, GameManager.Instance.Player.transform.position);

        // Verify the agent is in range of the player
        canAttackTarget = currentPlayerDistance <= attackRange && currentSpeed < 0.1f;

        if (!agent.pathPending && agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (canAttackTarget && !IsAnimationPlaying(0, ATTACK_STATE_TAG))
            {
                // Trigger attack animation
                Animator.SetTrigger(AttackHash);
            }
        }

        // reset attack trigger if the target is out of range
        if (!canAttackTarget)
        {
            Animator.ResetTrigger(AttackHash);
        }

        // Set the movement speed for animation blend
        Animator.SetFloat(MotionSpeedHash, currentSpeed);
    }

    public override void TakeDamage(int value, DamageType type)
    {
        PlayAudioClip(GetRandomClip(painClips), volume);

        base.TakeDamage(value, type);
    }

    public override void Die()
    {
        // stop agent from moving
        agent.isStopped = true;
        agent.enabled = false;
        lookAtTarget.enabled = false;

        // trigger death animation
        Animator.SetBool(IsDeadHash, true);

        // fall to the ground when dead
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = massOnDeath;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.freezeRotation = true;
        rb.useGravity = true;

        // play death sound clip
        PlayAudioClip(GetRandomClip(deathClips), volume);

        // despawn cleanup
        Destroy(gameObject, despawnDelay);

        base.Die();
    }

    // called by an animation event on the attack animation
    public void OnAttackHit()
    {   
        GameManager.Instance.Player.TakeDamage(damage, DamageType.Entity);
        PlayAudioClip(GetRandomClip(attackClips), volume);
    }

    public void PlayHitAnimation(Transform target)
    {
        Vector3 targetDirection = transform.position - target.position;

        // 1 for front facing, 0 for back facing
        float directionFacingTarget = Vector3.Dot(target.forward, targetDirection);

        PlayAnimation(directionFacingTarget > 0 ? HIT_FRONT_STATE_NAME : HIT_BACK_STATE_NAME, 0);
    }

    public void PlayIdleClip()
    {
        PlayAudioClip(GetRandomClip(idleClips), volume);
    }

    public AudioClip GetRandomClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length)];
    }
}

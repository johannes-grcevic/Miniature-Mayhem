using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EntityEnemy : Entity
{
    public static readonly int MotionSpeedHash = Animator.StringToHash("MotionSpeed");
    public static readonly int IsDeadHash = Animator.StringToHash("IsDead");
    public static readonly int AttackHash = Animator.StringToHash("Attack");
    public static readonly int AttackSpeedHash = Animator.StringToHash("AttackSpeed");

    public const string AttackStateTag = "Attack";
    public const string HitFrontStateName = "GetHitBack";
    public const string HitBackStateName = "GetHitFront";

    public bool CanAttack => canAttack;
    public float CurrentSpeed => currentSpeed;
    public float CurrentPlayerDistance => currentPlayerDistance;
    public NavMeshAgent Agent => agent;

    [Header("Attack")]
    [SerializeField]
    private int damage = 20;

    [SerializeField]
    private float attackRange = 1f;

    [SerializeField]
    private float attackSpeed = 1f;

    [Header("Death")]
    [SerializeField]
    private float despawnDelay = 5f;

    [SerializeField]
    private float massOnDeath = 50f;

    [Header("Audio")]
    [SerializeField]
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

    [Header("Animation")]
    [SerializeField]
    private bool hasHitAnimation = false;

    private NavMeshAgent agent;
    private EntityPlayer player;

    private float currentSpeed;
    private float currentPlayerDistance;
    private bool canAttack = false;

    protected override void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        base.Awake();
    }

    protected override void Start()
    {        
        if (attackRange < agent.stoppingDistance)
        {
            Debug.LogWarning($"[{name}] Attack range should not be less than the agent stopping distance. Setting attack range to {agent.stoppingDistance}.");
            attackRange = agent.stoppingDistance;
        }

        player = GameManager.Instance.Player;

        Animator.SetFloat(AttackSpeedHash, attackSpeed);

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

        // Cache the magnitude to avoid calculating it more than once per frame
        currentSpeed = agent.velocity.magnitude;

        // Convert to local space
        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
        float forwardSpeed = localVelocity.z;

        // Normalize the speed for the blendtree
        Animator.SetFloat(MotionSpeedHash, Mathf.InverseLerp(0f, 1f, forwardSpeed));

        // The current distance from the player
        currentPlayerDistance = Vector3.Distance(transform.position, player.transform.position);

        // Verify the agent is in range of the player
        canAttack = currentPlayerDistance <= attackRange && currentSpeed < 0.1f;

        if (!agent.pathPending && agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
        {
            // If we can attack the target and the attack animation is not already playing
            if (canAttack && !IsAnimationPlaying(0, AttackStateTag))
            {
                // Trigger attack animation
                Animator.SetTrigger(AttackHash);
            }
        }

        // Reset attack trigger if the target is out of range
        if (!canAttack)
        {
            Animator.ResetTrigger(AttackHash);
        }
    }

    public override void TakeDamage(int value, DamageType type)
    {
        base.TakeDamage(value, type);

        if (hasHitAnimation)
        {
            PlayHitAnimation(0);
        }

        // stop healing if being attacked
        IsHealing = false;

        PlayAudioClip(GetRandomClip(painClips), volume);
    }

    public override void Die()
    {
        base.Die();

        // stop agent from moving
        agent.isStopped = true;
        agent.enabled = false;

        // stop looking at the target
        if (TryGetComponent(out LookAt look))
        {
            look.Target = null;
        }

        // trigger death animation
        Animator.SetBool(IsDeadHash, true);

        // fall to the ground when dead
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = massOnDeath;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.freezeRotation = true;
        rb.useGravity = true;

        // stop idle sound loop
        CancelInvoke(nameof(PlayIdleClip));

        // play death sound clip
        PlayAudioClip(GetRandomClip(deathClips), volume);

        // despawn cleanup
        Destroy(gameObject, despawnDelay);
    }

    // should be called from an animation event on the attack animation
    public virtual void OnAttackHit()
    {
        player.TakeDamage(damage, DamageType.Entity);
        PlayAudioClip(GetRandomClip(attackClips), volume);
    }

    private void PlayHitAnimation(int layer)
    {
        // Get the direction from the player to the target
        Vector3 directionToPlayer = transform.position - player.transform.position;
        directionToPlayer.y = 0; // Flatten the height difference

        // A positive value means facing toward, negative means facing away
        float cosOfAngleToPlayer = Vector3.Dot(transform.forward, directionToPlayer.normalized);

        // If positive, the hit came from the front. If negative, the hit came from behind.
        PlayAnimation(cosOfAngleToPlayer > 0 ? HitFrontStateName : HitBackStateName, layer);
    }

    private void PlayIdleClip()
    {
        PlayAudioClip(GetRandomClip(idleClips), volume);
    }

    private AudioClip GetRandomClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length)];
    }
}

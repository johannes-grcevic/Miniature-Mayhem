using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(LookAtTarget))]
public class EntityEnemy : Entity
{
    public static readonly int IsDeadHash = Animator.StringToHash("IsDead");
    public static readonly int IsSearchingHash = Animator.StringToHash("IsSearching");
    public static readonly int AttackHash = Animator.StringToHash("Attack");
    public static readonly int SpeedHash = Animator.StringToHash("Speed");
    public static readonly int TauntHash = Animator.StringToHash("Taunt");

    public const string ATTACK_STATE_TAG = "Attack";
    public const string HIT_FRONT_STATE_NAME = "GetHitBack";
    public const string HIT_BACK_STATE_NAME = "GetHitFront";

    [SerializeField]
    private int damage = 20;

    [SerializeField]
    private float chaseRange = 10f;

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

    [SerializeField]
    private AudioClip[] tauntClips;

    [SerializeField, Range(0f, 1f)]
    private float volume = 1f;

    private NavMeshAgent agent;
    private LookAtTarget targetLook;

    private bool isSearchingTarget = false;
    private bool canAttackTarget = false;

    protected override void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        targetLook = GetComponent<LookAtTarget>();

        base.Awake();
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
        
        if (!agent.pathPending && agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Cache the magnitude once per frame to avoid calculating it twice
            float currentSpeed = agent.velocity.magnitude;

            // Verify the agent has a valid path and is in range of the player
            canAttackTarget = Vector3.Distance(agent.transform.position, agent.destination) <= attackRange && currentSpeed < 0.1f;

            if (canAttackTarget && !Animator.GetCurrentAnimatorStateInfo(0).IsTag(ATTACK_STATE_TAG))
            {
                // Trigger attack animation
                Animator.SetTrigger(AttackHash);
            }

            // Set the movement speed for animation blend
            Animator.SetFloat(SpeedHash, currentSpeed);
        }

        // todo: not fully implemented yet
        //TrackTarget();
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
        targetLook.enabled = false;

        // trigger death animation
        Animator.SetBool(IsDeadHash, true);

        // fall to the ground when dead
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.mass = massOnDeath;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.freezeRotation = true;
        rb.isKinematic = false;
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

    public void OnTargetLost()
    {
        PlayAudioClip(GetRandomClip(tauntClips), volume);

        Animator.SetBool(IsSearchingHash, isSearchingTarget);
    }

    public void TrackTarget()
    {
        float distanceToTarget = agent.remainingDistance;

        // Start searching if the target is out of the chase range
        isSearchingTarget = distanceToTarget > 0f && distanceToTarget > chaseRange;

        if (isSearchingTarget)
        {
            OnTargetLost();
        }
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

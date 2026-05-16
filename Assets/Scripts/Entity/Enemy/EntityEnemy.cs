using System.IO;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class EntityEnemy : Entity
{
    public static readonly int IsDeadHash = Animator.StringToHash("IsDead");
    public static readonly int IsSearchingHash = Animator.StringToHash("IsSearching");
    public static readonly int AttackHash = Animator.StringToHash("Attack");
    public static readonly int SpeedHash = Animator.StringToHash("Speed");
    public static readonly int TauntHash = Animator.StringToHash("Taunt");

    public const string ATTACK_STATE_TAG = "Attack";
    public const string HIT_FRONT_STATE_TAG = "HitFront";
    public const string HIT_BACK_STATE_TAG = "HitBack";

    [SerializeField]
    private int damage = 20;

    [SerializeField]
    private float attackRange = 2f;

    [SerializeField]
    private float chaseRange = 10f;

    [SerializeField]
    private float despawnDelay = 5f;

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

    private AudioSource enemySource;

    private Animator animator;
    private NavMeshAgent agent;
    private Rigidbody rb;

    private bool isSearchingTarget = false;
    private bool isTargetLost = false;
    private bool canAttackTarget = false;

    protected override void Awake()
    {
        enemySource = GetComponent<AudioSource>();

        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        base.Awake();
    }

    private void Start()
    {
        enemySource.playOnAwake = false;

        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // Let the agent handle position changes natively
        agent.updatePosition = true;
        agent.updateRotation = true;

        agent.stoppingDistance = attackRange;
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
        // Cache the magnitude once per frame to avoid calculating it twice
        float currentSpeed = agent.velocity.magnitude;

        // Verify the agent has a valid path and is in range of the player
        canAttackTarget = !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && currentSpeed < 0.1f;

        if (canAttackTarget && !animator.GetCurrentAnimatorStateInfo(0).IsTag(ATTACK_STATE_TAG))
        {
            // Trigger attack animation
            animator.SetTrigger(AttackHash);
        }

        // Set the movement speed for animation blend
        animator.SetFloat(SpeedHash, currentSpeed);

        // todo: not fully implemented yet
        TrackTarget();
        //animator.SetBool(IsSearchingHash, isSearching);
    }

    public override void TakeDamage(int value, DamageType type)
    {
        PlayAudioClip(GetRandomClip(painClips), volume);

        base.TakeDamage(value, type);
    }

    public override void Die()
    {
        agent.isStopped = true;
        agent.speed = 0f;

        // trigger death animation
        animator.SetBool(IsDeadHash, true);

        PlayAudioClip(GetRandomClip(deathClips), volume);
        Destroy(gameObject, despawnDelay);

        base.Die();
    }

    // called by an animation event on the enemy
    public void OnAttackHit()
    {
        // only allow a hit if the target is in range of the player
        if (Vector3.Distance(agent.destination, transform.position) > attackRange) return;

        GameManager.Instance.Player.TakeDamage(damage, DamageType.Entity);
        PlayAudioClip(GetRandomClip(attackClips), volume);

        isTargetLost = false;
    }

    public void OnTargetLost(float distanceFromTarget)
    {
        PlayAudioClip(GetRandomClip(tauntClips), volume * Mathf.InverseLerp(0f, 1f, distanceFromTarget));
        isTargetLost = true;
    }

    public void PlayAnimation(string stateName, int layer)
    {
        // if its not already playing
        if (!animator.GetCurrentAnimatorStateInfo(layer).IsTag(stateName))
        {
            animator.Play(stateName, layer);
        }
    }

    public void PlayAudioClip(AudioClip clip, float volume)
    {
        if (clip == null) return;

        enemySource.PlayOneShot(clip, volume);
    }

    public void PlayIdleClip()
    {
        PlayAudioClip(GetRandomClip(idleClips), volume);
    }

    public AudioClip GetRandomClip(AudioClip[] clips)
    {
        return clips[Random.Range(0, clips.Length)];
    }

    public void TrackTarget()
    {
        float distanceToTarget = agent.remainingDistance;

        // Start searching if the target is out of the chase range
        isSearchingTarget = distanceToTarget > chaseRange;

        if (isSearchingTarget && !isTargetLost)
        {
            OnTargetLost(distanceToTarget);
        }
    }
}

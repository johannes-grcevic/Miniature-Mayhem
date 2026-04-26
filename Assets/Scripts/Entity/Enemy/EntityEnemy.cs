using Unity.Entities;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
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
    private float attackRange = 2f;

    [SerializeField]
    private float chaseRange = 10f;

    [SerializeField]
    private float despawnDelay = 10f;

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

    private bool isSearching = false;
    private bool isAttacking = false;
    private bool isTargetLost = false;

    public override void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        enemySource = GetComponent<AudioSource>();
        enemySource.playOnAwake = false;

        base.Awake();
    }

    public void Start()
    {
        if (attackRange == 0f)
        {
            attackRange = agent.stoppingDistance;
        }
    }

    public void OnEnable()
    {
        InvokeRepeating(nameof(PlayIdleClip), 0f, Mathf.Max(0.1f, idleRepeatRate));
    }

    public void OnDisable()
    {
        CancelInvoke(nameof(PlayIdleClip));
    }

    void Update()
    {
        isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack");

        if (IsPathComplete() && !isAttacking)
        {
            animator.SetTrigger(AttackHash);
        }
    }

    void OnAnimatorMove()
    {
        SearchForTarget(agent.destination);

        animator.SetFloat(MoveSpeedHash, agent.velocity.magnitude);

        // not fully implemented yet
        //animator.SetBool(IsSearchingHash, isSearching);
    }

    public override void TakeDamage(int value)
    {
        PlayAudioClip(GetRandomClip(painClips), volume);
        base.TakeDamage(value);
    }

    // called by an animation event on the enemy
    public void OnAttackHit()
    {
        // only allow a hit if the target is in range of the player
        if (Vector3.Distance(agent.destination, transform.position) > attackRange) return;

        GameManager.Instance.GetPlayer().TakeDamage(damage);
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
        if (animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName))
        {
            return;
        }

        animator.Play(stateName, layer);
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

    public void SearchForTarget(Vector3 targetPosition)
    {
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // lose focus if the target is far away
        isSearching = distanceToTarget > chaseRange;

        if (isSearching && !isTargetLost)
        {
            OnTargetLost(distanceToTarget);
        }
    }

    public bool IsSearching()
    {
        return isSearching;
    }

    public bool IsPathComplete()
    {
        if (!agent.hasPath) return false;

        return Vector3.Distance(agent.destination, agent.transform.position) <= agent.stoppingDistance;
    }
}

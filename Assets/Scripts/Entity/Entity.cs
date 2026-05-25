using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class Entity : MonoBehaviour
{
    public UnityAction<int, DamageType> OnDamageTaken;
    public UnityAction<int> OnHeal;
    public UnityAction OnDeath;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public float CurrentHealthPerc => currentHealthPerc;
    public bool IsDead => !isAlive;
    public bool IsHealing { get; set; }
    public bool CanHeal => canHeal;
    public float MaxHealAmountPerc => maxHealAmountPerc;
    public Animator Animator => animator;
    public bool HasAnimator => hasAnimator;
    public Vector3 SpawnPoint => spawnPoint;

    public EntityType Type => entityType;

    [Header("Base Stats")]
    [SerializeField]
    private EntityType entityType;

    [SerializeField]
    private int maxHealth = 50;

    [SerializeField, Range(0f, 100f)]
    private float maxHealAmountPerc = 100f;

    [SerializeField, Space(10)]
    private UnityEvent<float> OnHealthChanged;

    private int currentHealth;
    private float currentHealthPerc;
    private bool isAlive = false;
    private bool canHeal = false;

    private AudioSource source;
    private Animator animator;
    private bool hasAnimator = false;
    private Vector3 spawnPoint;

    protected virtual void Awake()
    {
        source = GetComponent<AudioSource>();
        hasAnimator = TryGetComponent(out animator);
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        isAlive = true;
        canHeal = false;

        UpdateHealthInternal();
    }

    public virtual void TakeDamage(int value, DamageType type)
    {
        if (currentHealth - value < 0)
        {
            currentHealth = 0;
        }
        else
        {
            currentHealth -= value;
        }

        UpdateHealthInternal();

        OnDamageTaken?.Invoke(value, type);
        OnHealthChanged.Invoke(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(int value)
    {
        if (currentHealth + value > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += value;
        }

        UpdateHealthInternal();

        OnHeal?.Invoke(value);
        OnHealthChanged.Invoke(currentHealth);
    }

    public virtual void Die()
    {
        isAlive = false;
        OnDeath?.Invoke();
    }

    public virtual void PlayAnimation(string stateName, int layer)
    {
        if (!HasAnimator) return;
        
        // if its not already playing, play it
        if (!animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName))
        {
            animator.Play(stateName, layer);
        }
    }

    public virtual bool IsAnimationPlaying(int layer, string stateTag)
    {
        return animator.GetCurrentAnimatorStateInfo(layer).IsTag(stateTag);
    }

    public virtual void PlayAudioClip(AudioClip clip, float volumeScale)
    {
        if (clip != null && volumeScale > 0f)
        {
            source.PlayOneShot(clip, volumeScale);
        }
    }

    public virtual void SetMaxHealth(int value)
    {
        maxHealth = value;
    }

    public virtual void SetSpawnPoint(Vector3 position)
    {
        if (position != spawnPoint)
        {
            spawnPoint = position;
        }
    }

    private void UpdateHealthInternal()
    {
        currentHealthPerc = (float)currentHealth / maxHealth * 100f;
        canHeal = currentHealthPerc < maxHealAmountPerc;
    }
}

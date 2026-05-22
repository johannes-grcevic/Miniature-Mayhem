using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class Entity : MonoBehaviour
{
    public UnityAction<int, DamageType> OnDamageTaken;
    public UnityAction<int> OnHeal;
    public UnityAction<int> OnHealthChanged;
    public UnityAction<GameState> OnDeath;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public float CurrentHealthPerc => currentHealthPerc;
    public bool IsDead => !isAlive;
    public Animator Animator => animator;
    public bool HasAnimator => hasAnimator;
    public Vector3 SpawnPoint => spawnPoint;

    public EntityType EntityType => entityType;

    [SerializeField, Header("Base Stats")]
    private EntityType entityType;

    [SerializeField]
    private GameState gameStateOnDeath;

    [SerializeField]
    private int maxHealth = 50;

    private int currentHealth;
    private float currentHealthPerc;
    private bool isAlive = false;

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

        UpdateCurrentHealthPerc();
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

        UpdateCurrentHealthPerc();

        OnDamageTaken?.Invoke(value, type);
        OnHealthChanged?.Invoke(currentHealth);

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

        UpdateCurrentHealthPerc();

        OnHeal?.Invoke(value);
        OnHealthChanged?.Invoke(currentHealth);
    }

    public virtual void Die()
    {
        isAlive = false;
        OnDeath?.Invoke(gameStateOnDeath);
    }

    public virtual void PlayAnimation(string stateName, int layer)
    {
        if (!HasAnimator || !TargetStateHasClip(stateName)) return;
        
        // if its not already playing, play it
        if (!animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName))
        {
            animator.Play(stateName, layer);
        }
    }

    public virtual bool IsAnimationPlaying(int layerIndex, string stateTag)
    {
        return animator.GetCurrentAnimatorStateInfo(layerIndex).IsTag(stateTag);
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
        if (position != Vector3.zero)
        {
            spawnPoint = position;
        }
    }

    public void SetDead()
    {
        Die();
    }

    private void UpdateCurrentHealthPerc()
    {
        currentHealthPerc = (float)currentHealth / maxHealth * 100f;
    }

    private bool TargetStateHasClip(string stateName)
    {
        // Ensure the animator is actually running an Override Controller
        AnimatorOverrideController overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
        if (overrideController == null) return true; // Fallback for standard controllers

        AnimationClip currentOverrideClip = overrideController[stateName];

        // If the indexer returns null, or the clip is a dummy/empty asset, block it
        if (currentOverrideClip == null || currentOverrideClip.name == "None" || currentOverrideClip.name == "")
        {
            return false;
        }

        return true;
    }
}

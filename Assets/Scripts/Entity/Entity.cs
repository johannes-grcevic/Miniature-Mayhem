using UnityEngine;
using UnityEngine.Events;

public class Entity : MonoBehaviour
{
    public UnityEvent<int, DamageType> OnDamageTaken { get; private set; } = new();
    public UnityEvent<int> OnHeal { get; private set; } = new();
    public UnityEvent<int> OnHealthChanged { get; private set; } = new();
    public UnityEvent<GameState> OnDeath { get; private set; } = new();

    [SerializeField, Header("Health")]
    private int maxHealth = 50;

    private int currentHealth;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
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

        OnDamageTaken.Invoke(value, type);
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

        OnHeal.Invoke(value);
        OnHealthChanged.Invoke(currentHealth);
    }

    public virtual void Die()
    {
        OnDeath.Invoke(GameState.Over);
    }

    public void SetMaxHealth(int value)
    {
        maxHealth = value;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}

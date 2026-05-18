using UnityEngine;

[RequireComponent(typeof(Entity))]
public class DynamicBloodEffect : MonoBehaviour
{
    [SerializeField, Tooltip("mount of blood when taking damage (relative to damage taken and health remaining)")]
    private float damageBloodAmount = 3f;

    [SerializeField, Tooltip("max amount of blood when not taking damage (relative to health lost)")]
    private float maxBloodIndication = 0.5f;

    private Entity target;

    private void Awake()
    {
        target = GetComponent<Entity>();
    }

    private void OnEnable()
    {
        target.OnDamageTaken += ApplyBloodDamage;
    }

    private void OnDisable()
    {
        target.OnDamageTaken -= ApplyBloodDamage;
    }

    private void Update()
    {
        BleedBehavior.minBloodAmount = maxBloodIndication * (target.MaxHealth - target.CurrentHealth) / target.MaxHealth;
    }

    public void ApplyBloodDamage(int amount, DamageType type)
    {
        BleedBehavior.BloodAmount += Mathf.Clamp01(damageBloodAmount * amount / target.CurrentHealth);
        BleedBehavior.minBloodAmount = maxBloodIndication * (target.MaxHealth - target.CurrentHealth) / target.MaxHealth;
    }
}

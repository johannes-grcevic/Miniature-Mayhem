using System.Collections;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField, Header("Target")]
    private EntityType targetType = EntityType.Player;

    [SerializeField, Header("Damage")]
    private DamageType damageType = DamageType.Hazard;

    [SerializeField] 
    private int damage = 5;

    [SerializeField, Tooltip("The rate of damage in seconds")] 
    private float damageRate = 1.0f;

    private WaitForSeconds waitForDamageDelay;
    private Coroutine damageLoopRoutine;

    private void Awake()
    {
        waitForDamageDelay = new WaitForSeconds(damageRate);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Entity collidingEntity) && collidingEntity.Type == targetType)
        {
            SetDamageState(collidingEntity, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Entity collidingEntity) && collidingEntity.Type == targetType)
        {
            SetDamageState(collidingEntity, false);
        }
    }

    public void SetDamageState(Entity target, bool value)
    {
        // if enabled start the routine otherwise stop it
        if (value)
        {
            damageLoopRoutine = StartCoroutine(ApplyDamageOT(target, damage, damageType, waitForDamageDelay));
        }
        else
        {
            StopCoroutine(damageLoopRoutine);
        }
    }

    private IEnumerator ApplyDamageOT(Entity target, int amount, DamageType type, WaitForSeconds delay)
    {        
        while (true)
        {
            target.TakeDamage(amount, type);

            yield return delay;
        }
    }
}
using System.Collections;
using UnityEngine;

public class DamageOnStay : MonoBehaviour
{
    [SerializeField] 
    private Entity target;

    [SerializeField] 
    private int damage = 5;

    [SerializeField, Tooltip("The rate of damage in seconds")] 
    private float damageRate = 1.0f;

    private Coroutine damageLoop;
    private WaitForSeconds waitForDamageRateDelay;

    private void Awake()
    {
        waitForDamageRateDelay = new WaitForSeconds(damageRate);
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (target == null || !other.TryGetComponent(out Entity entity) || !entity.Equals(target)) return;

        damageLoop = StartCoroutine(DamageLoop());
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Entity entity) && entity.Equals(target))
        {
            StopCoroutine(damageLoop);
        }
    }

    private IEnumerator DamageLoop()
    {
        while (true)
        {
            target.TakeDamage(damage, DamageType.Hazard);
            yield return waitForDamageRateDelay;
        }
    }
}
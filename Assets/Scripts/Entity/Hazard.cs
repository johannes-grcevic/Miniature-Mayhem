using System.Collections;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] 
    private Entity target;

    [SerializeField] 
    private int damage = 5;

    [SerializeField]
    private DamageType damageType = DamageType.Hazard;

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
        SetDamageState(true);
    }

    private void OnTriggerExit(Collider other)
    {
        SetDamageState(false);
    }

    public void SetDamageState(bool value)
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
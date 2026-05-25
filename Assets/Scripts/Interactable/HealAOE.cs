using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class HealAOE : MonoBehaviour
{
    [SerializeField]
    private EntityType targetType;

    [SerializeField]
    private int healAmount = 1;

    [SerializeField, Tooltip("The delay in seconds between each heal")]
    private float healInterval = 1f;

    [SerializeField, Header("VFX")]
    private ParticleSystem healParticle;

    [SerializeField]
    private ParticleSystemStopBehavior healStopBehavior;

    [SerializeField]
    private ParticleSystemStopAction healStopAction;

    [SerializeField, Header("Audio")]
    private AudioClip healClip;

    [SerializeField]
    private float volumeScale = 1.0f;

    private AudioSource healSource;

    private ParticleSystem currentHealParticle;
    private float healTimer;

    private void Awake()
    {
        healSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        healSource.clip = healClip;
        healSource.volume = volumeScale;

        healTimer = 0f;

        InitHealParticle();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsValidTarget(other, out Entity target)) return;

        if (target.CanHeal)
        {
            OnHealStart(target);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsValidTarget(other, out Entity target)) return;

        if (target.CanHeal)
        {
            OnHealStay(target);
        }

        if (!target.CanHeal && target.IsHealing)
        {
            OnHealStop(target);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsValidTarget(other, out Entity target)) return;

        OnHealStop(target);
    }

    public void OnHealStart(Entity other)
    {
        healSource.Play();

        other.IsHealing = true;
        
        if (healParticle)
        {
            currentHealParticle = Instantiate(healParticle, transform);
            currentHealParticle.Play(currentHealParticle.transform.childCount > 0);
        }
    }

    public void OnHealStay(Entity other)
    {
        healTimer += Time.deltaTime;
        if (healTimer > healInterval)
        {
            other.Heal(healAmount);

            // reset the heal timer
            healTimer = 0f;
        }
    }

    public void OnHealStop(Entity other)
    {        
        healSource.Stop();

        other.IsHealing = false;

        // reset the heal timer
        healTimer = 0f;

        // stop playing particles
        if (currentHealParticle)
        {
            currentHealParticle.Stop(currentHealParticle.transform.childCount > 0, healStopBehavior);
        }
    }

    private bool IsValidTarget(Collider other, out Entity target)
    {
        return TryGetCollidingEntity(other, out target) && target.Type == targetType;
    }

    private bool TryGetCollidingEntity(Collider other, out Entity colliding)
    {
        return other.TryGetComponent(out colliding);
    }

    private void InitHealParticle()
    {
        // set up particle stop action
        var mainModule = healParticle.main;
        mainModule.stopAction = healStopAction;

        // set up child particles if they exist
        if (healParticle.transform.childCount <= 0) return;

        foreach (Transform child in healParticle.transform)
        {
            if (child.TryGetComponent(out ParticleSystem childParticle))
            {
                var childMainModule = childParticle.main;
                childMainModule.stopAction = healStopAction;
            }
        }
    }
}
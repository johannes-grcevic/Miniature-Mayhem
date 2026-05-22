using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class HealAOE : MonoBehaviour
{
    [SerializeField]
    private EntityType[] targetTypes;

    [SerializeField]
    private int healAmount = 1;

    [SerializeField, Tooltip("The delay in seconds between each heal")]
    private float healInterval = 1f;

    [SerializeField, Range(0, 100), Tooltip("Heal the target up to a percentage of their max health")]
    private int maxHealAmountPerc = 50;

    [SerializeField, Header("VFX")]
    private ParticleSystem healingParticle;

    [SerializeField, Header("Audio")]
    private AudioClip healingClip;

    [SerializeField]
    private float volumeScale = 1.0f;

    private AudioSource healSource;
    private ParticleSystem attachedParticle;
    private float healTimer;

    private void Awake()
    {
        healSource = GetComponent<AudioSource>();
    }

    private void Start()
    {      
        healSource.clip = healingClip;
        healSource.volume = volumeScale;
        healTimer = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Entity collidingEntity)) return;
        
        if (IsValidTarget(collidingEntity))
        {
            OnHealStart(other.transform);
        }
    }

    private void OnTriggerStay(Collider other)
    {   
        if (!other.TryGetComponent(out Entity collidingEntity)) return;

        if (!IsValidTarget(collidingEntity))
        {
            OnHealStop(ParticleSystemStopBehavior.StopEmittingAndClear);
            return;
        }

        healTimer += Time.deltaTime;
        if (healTimer > healInterval)
        {
            collidingEntity.Heal(healAmount);
            healTimer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!attachedParticle) return;

        OnHealStop(ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void OnHealStart(Transform attachTransform)
    {
        healSource.Play();
        
        if (healingParticle)
        {
            attachedParticle = Instantiate(healingParticle, attachTransform);
            attachedParticle.Play(attachedParticle.transform.childCount > 0);
        }
    }

    public void OnHealStop(ParticleSystemStopBehavior stopBehavior)
    {
        healSource.Stop();

        if (attachedParticle.isPlaying)
        {
            attachedParticle.Stop(attachedParticle.transform.childCount > 0, stopBehavior);
        }

        // clean up particles
        Destroy(attachedParticle, healingParticle.main.duration);
    }

    private bool IsValidTarget(Entity target)
    {
        return targetTypes.Any(type => type == target.EntityType) && target.CurrentHealthPerc < maxHealAmountPerc;
    }
}
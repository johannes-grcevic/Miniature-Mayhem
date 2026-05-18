using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class HealAOE : MonoBehaviour
{
    [SerializeField]
    private EntityType targetType = EntityType.Player;

    [SerializeField]
    private int healAmount = 1;

    [SerializeField, Tooltip("The speed that the target can heal")]
    private float healInterval = 1f;

    [SerializeField, Range(0, 100), Tooltip("Heal the target up to a percentage of their max health")]
    private int maxHealAmountPerc = 50;

    [SerializeField, Header("VFX")]
    private ParticleSystem healingParticle;

    [SerializeField]
    private Transform attachTransform;

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
        if (other.TryGetComponent(out Entity target) && target.CurrentHealthPerc < maxHealAmountPerc)
        {
            OnHealStart(target, attachTransform, true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!attachedParticle) return;

        OnHealStop(ParticleSystemStopBehavior.StopEmittingAndClear, true);
    }

    private void OnTriggerStay(Collider other)
    {   
        if (!other.TryGetComponent(out Entity collidingEntity) || collidingEntity.EntityType != targetType) return;

        if (collidingEntity.CurrentHealthPerc < maxHealAmountPerc)
        {
            healTimer += Time.deltaTime;

            if (healTimer > healInterval)
            {
                collidingEntity.Heal(healAmount);
                healTimer = 0f;
            }
        }
        else
        {
            OnHealStop(ParticleSystemStopBehavior.StopEmittingAndClear, true);
        }
    }

    public void OnHealStart(Entity target, Transform attachTransform, bool playWithChildren = false)
    {
        if (!target || !healingParticle) return;

        healSource.Play();

        attachedParticle = Instantiate(healingParticle, attachTransform);
        attachedParticle.Play(playWithChildren);
    }

    public void OnHealStop(ParticleSystemStopBehavior particleStopBehavior, bool stopWithChildren = false)
    {        
        if (attachedParticle && attachedParticle.isPlaying)
        {
            attachedParticle.Stop(stopWithChildren, particleStopBehavior);
        }

        healSource.Stop();

        // clean up particles
        Destroy(attachedParticle, healingParticle.main.duration);
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ParticleSystem))]
public class Projectile : MonoBehaviour 
{
    public UnityAction<GameObject> OnParticleFirstCollision;

    [Header("Projectile")]
    [SerializeField]
    private float speed = 50f;

    [SerializeField]
    private DamageType damageType;

    [SerializeField]
    private int damage = 10;

    [SerializeField, Tooltip("The particle is destroyed after this time if it does not collide with anything")]
    private float lifetime = 30f;

    [Header("VFX")]
    [SerializeField]
	private ParticleSystem hitEffect;

    private ParticleSystem projectileParticle;
    private bool hasCollided;

    private readonly List<ParticleCollisionEvent> collisionEvents = new();

    private void Awake()
    {
        projectileParticle = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (speed != 0)
        {
            transform.position += speed * Time.deltaTime * transform.forward;
        }
    }

    private void OnParticleCollision(GameObject other)
    {   
        if (hasCollided || !other || other.CompareTag("Player")) return;

        hasCollided = true;
        OnParticleFirstCollision?.Invoke(other);

        // store the particle collision events in a list
        projectileParticle.GetCollisionEvents(other, collisionEvents);

        // Exact world location where particle hit
        Vector3 hitPoint = collisionEvents[0].intersection;

        // Calculate rotation and position strictly based on World Space intersection
        Quaternion hitRotation = Quaternion.LookRotation(collisionEvents[0].normal);

        // Damage the entity that we collided with
        if (other.TryGetComponent(out Entity collidedEntity))
        {
            DoDamage(collidedEntity, damage, damageType);
        }

        if (hitEffect)
        {
            // Spawn impact particle effect at the collision point
            ParticleSystem hitVFX = Instantiate(hitEffect, hitPoint, hitRotation);
            Destroy(hitVFX.gameObject, hitVFX.main.duration);
        }

        // Destroy the projectile on impact
        Destroy(gameObject);
    }

    private void DoDamage(Entity target, int amount, DamageType type)
    {
        if (!target.IsDead)
        {
            target.TakeDamage(amount, type);
        }
    }
}

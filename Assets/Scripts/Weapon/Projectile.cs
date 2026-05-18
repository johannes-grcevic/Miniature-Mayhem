using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class Projectile : MonoBehaviour 
{
    public event System.Action<GameObject> OnCollision;

    [SerializeField]
    private float speed = 50f;

    [Header("VFX")]
    [SerializeField]
	private ParticleSystem hit;

    private ParticleSystem projectileSystem;
    private bool collided;

    private readonly List<ParticleCollisionEvent> collisionEvents = new();

    private void Awake()
    {
        projectileSystem = GetComponent<ParticleSystem>();
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
        if (collided || !other || other.CompareTag("Player")) return;

        collided = true;
        OnCollision?.Invoke(other);

        // store the particle collision events
        projectileSystem.GetCollisionEvents(other, collisionEvents);

        foreach (var collision in collisionEvents)
        {
            // Calculate rotation and position strictly based on World Space intersection
            Quaternion hitRotationNormal = Quaternion.LookRotation(collision.normal);

            // Exact world spot where particle hit
            Vector3 hitPoint = collision.intersection;

            if (hit != null)
            {
                ParticleSystem hitVFX = Instantiate(hit, hitPoint, hitRotationNormal);
                Destroy(hitVFX.gameObject, hitVFX.main.duration);
            }
        }

        // destroy the projectile on impact
        Destroy(projectileSystem.gameObject);
    }
}

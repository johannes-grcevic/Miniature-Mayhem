using ConditionalField;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileMove : MonoBehaviour 
{
    [SerializeField]
    private bool rotate = false;

    [SerializeField, ConditionalField(nameof(rotate))]
    private float rotateAmount = 45;

    [SerializeField]
    private float speed;

    [SerializeField, Tooltip("From 0% to 100%")]
	private float accuracy;

    [SerializeField]
	private float fireRate;

    [Header("Particle Effects")]
    [SerializeField]
	private GameObject muzzlePrefab;

    [SerializeField]
	private GameObject hitPrefab;

    [SerializeField]
	private List<GameObject> trails;

    public UnityEvent<GameObject> OnCollision { get; private set; } = new UnityEvent<GameObject>();

    private Vector3 offset;
	private bool collided;
	private Rigidbody rb;

    [Header("Collision")]
    [SerializeField]
    private ParticleSystem collisionParticle;

    private readonly List<ParticleCollisionEvent> collisionEvents = new();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (!collisionParticle && transform.GetChild(0).TryGetComponent(out ParticleSystem ps))
        {
            collisionParticle = ps;

            // add relay so child can call back to parent
            var collisionRelay = collisionParticle.gameObject.AddComponent<ParticleCollisionRelay>();
            collisionRelay.SetParent(this);
        }
    }

    void Start()
    {
        // create a radius for the accuracy
        if (accuracy != 100)
        {
            accuracy = 1 - (accuracy / 100);

            for (int i = 0; i < 2; i++)
            {
                var val = 1 * Random.Range(-accuracy, accuracy);
                var index = Random.Range(0, 2);

                if (i == 0)
                {
                    offset = index == 0 ? new Vector3(0, -val, 0) : new Vector3(0, val, 0);
                }
                else
                {
                    offset = index == 0 ? new Vector3(0, offset.y, -val) : new Vector3(0, offset.y, val);
                }
            }
        }

        if (muzzlePrefab == null) return;

        GameObject muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
        muzzleVFX.transform.forward = gameObject.transform.forward + offset;

        if (muzzleVFX.TryGetComponent(out ParticleSystem ps))
        {
            Destroy(muzzleVFX, ps.main.duration);
        }
        else
        {
            Transform psChildTransform = muzzleVFX.transform.GetChild(0);

            if (psChildTransform.TryGetComponent(out ParticleSystem psChild)) {
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }
    }

    void FixedUpdate() 
    {
        if (rotate)
        {
            transform.Rotate(0, 0, rotateAmount, Space.Self);
        }

        if (speed != 0 && rb != null)
        {
            rb.position += (transform.forward + offset) * (speed * Time.deltaTime);
        }
    }

    public void HandleParticleCollision(GameObject other)
    {   
        if (collided || !other) return;

        collided = true;
        OnCollision.Invoke(other);

        if (trails.Count > 0)
        {
            foreach (GameObject trail in trails)
            {
                trail.transform.parent = null;
                if (trail.TryGetComponent(out ParticleSystem ps))
                {
                    ps.Stop();
                    Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
                }
            }
        }

        speed = 0;
        rb.isKinematic = true;

        collisionParticle.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < collisionEvents.Count; i++)
        {
            ParticleCollisionEvent collision = collisionEvents[i];
            Quaternion hitRotationNormal = Quaternion.FromToRotation(Vector3.up, collision.normal);
            Vector3 hitPoint = collision.intersection;

            if (hitPrefab != null)
            {
                GameObject hitVFX = Instantiate(hitPrefab, hitPoint, hitRotationNormal);

                if (hitVFX.TryGetComponent(out ParticleSystem ps))
                {
                    Destroy(hitVFX, ps.main.duration);
                }
                else
                {
                    Transform psChildTransform = hitVFX.transform.GetChild(0);

                    if (psChildTransform.TryGetComponent(out ParticleSystem psChild))
                    {
                        Destroy(hitVFX, psChild.main.duration);
                    }
                }
            }
        }

        Destroy(gameObject);
    }
}

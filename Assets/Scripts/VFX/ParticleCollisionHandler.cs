using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleCollisionHandler : MonoBehaviour
{
    public UnityEvent<ParticleSystem, GameObject> OnCollision { get; private set; } = new();
    public UnityEvent<ParticleSystem, GameObject> OnFirstCollision { get; private set; } = new();

    private bool collided = false;

    private void OnParticleCollision(GameObject other)
    {
        OnCollision.Invoke(GetComponent<ParticleSystem>(), other);

        if (!collided)
        {
            OnFirstCollision.Invoke(GetComponent<ParticleSystem>(), other);
        }

        collided = true;
    }
}
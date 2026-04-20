using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem particle;

    public ParticleSystem SpawnParticle(Vector3 position, Quaternion rotation)
    {
        ParticleSystem spawnedParticle = Instantiate(particle, position, rotation);

        spawnedParticle.gameObject.AddComponent<ParticleCollisionHandler>();

        Destroy(spawnedParticle.gameObject, spawnedParticle.main.duration);

        return spawnedParticle;
    }
}

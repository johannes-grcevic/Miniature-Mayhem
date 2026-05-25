using ConditionalField;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{  
    [Header("Spawner")]
    [SerializeField]
    private Entity[] entities;

    [SerializeField]
    private float spawnStartDelay = 1.0f;

    [SerializeField]
    private float respawnDelay = 10f;

    [SerializeField, Space(10), ConditionalField(nameof(spawnOnCollison), Conditional.Options.Invert)]
    private bool randomSpawnPoint = false;

    [SerializeField, ConditionalField(nameof(randomSpawnPoint))]
    private Vector2 spawnPointRange = new(-5f, 5f);

    [Header("Particle Spawner")]
    [SerializeField]
    private bool spawnOnCollison;

    [SerializeField, ConditionalField(nameof(spawnOnCollison)), Range(0f, 600f)]
    private float yOffset = 50f;

    private ParticleSpawner particleSpawner;
    private bool hasParticleSpawner = false;

    private void Awake()
    {
        hasParticleSpawner = TryGetComponent(out particleSpawner);
    }

    private void Start()
    {
        if (entities.Length > 0)
        {
            // start spawning enemies
            _ = SpawnLoopAsync(destroyCancellationToken);
        }
    }

    private async Awaitable SpawnLoopAsync(CancellationToken token)
    {
        if (!CanSpawnEntity()) return;

        while (true)
        {
            // Wait for the respawn delay before spawning another entity
            await Awaitable.WaitForSecondsAsync(Time.time > spawnStartDelay ? respawnDelay : spawnStartDelay, token);

            if (spawnOnCollison && hasParticleSpawner)
            {
                ParticleSystem ps = particleSpawner.SpawnParticle(TerrainUtils.GetRandomPosition(Terrain.activeTerrain, yOffset), Quaternion.identity);

                if (ps.TryGetComponent(out ParticleCollisionHandler collisionHandler))
                {
                    collisionHandler.OnFirstCollision.AddListener(OnSpawnParticleFirstCollision);
                }
                else
                {
                    ps.gameObject.AddComponent<ParticleCollisionHandler>().OnFirstCollision.AddListener(OnSpawnParticleFirstCollision);
                }
            }
            else
            {
                Vector3 randomSpawnPosition = transform.position + new Vector3(
                    Random.Range(spawnPointRange.x, spawnPointRange.y), 
                    0f, 
                    Random.Range(spawnPointRange.x, spawnPointRange.y));

                Vector3 spawnPosition = randomSpawnPoint ? randomSpawnPosition : transform.position;

                if (entities.Length == 1)
                {
                    SpawnEntity(entities[0], spawnPosition, entities[0].transform.rotation);
                }
                else
                {
                    Entity random = GetRandomEntity();
                    SpawnEntity(random, spawnPosition, random.transform.rotation);
                }
            }
        }
    }

    public void OnSpawnParticleFirstCollision(ParticleSystem ps, GameObject other)
    {    
        List<ParticleCollisionEvent> collisionEvents = new();
        ps.GetCollisionEvents(other, collisionEvents);

        Entity random = GetRandomEntity();
        SpawnEntity(random, collisionEvents[0].intersection, random.transform.rotation);
    }

    public Entity SpawnEntity(Entity entity, Vector3 position, Quaternion rotation)
    {
        if (hasParticleSpawner)
        {
            // spawn a particle to mask entity spawning
            particleSpawner.SpawnParticle(position, rotation);
        }

        Entity spawnedEntity = Instantiate(entity, position, rotation);

        // set the spawn point without any offset applied
        spawnedEntity.SetSpawnPoint(spawnOnCollison ? position : transform.position);

        // start a dynamic music track based on the entity type
        MusicManager.Instance.StartTrack(spawnedEntity.Type);

        return spawnedEntity;
    }

    public bool CanSpawnEntity() => entities.Length > 0 && Time.timeScale > 0.0f;

    public Entity GetRandomEntity() => entities[Random.Range(0, entities.Length)];
}
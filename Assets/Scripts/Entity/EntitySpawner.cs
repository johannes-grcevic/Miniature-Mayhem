using ConditionalField;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class EntitySpawner : MonoBehaviour
{
    public UnityEvent<EntityType, Vector3, Quaternion> OnEntitySpawned;
    public UnityEvent<EntityType> OnBossEntitySpawned;
    
    [SerializeField]
    private Entity[] entities;

    [Header("Spawner")]
    [SerializeField]
    private float spawnStartDelay = 1.0f;

    [SerializeField]
    private float respawnDelay = 10f;

    [SerializeField, Space(10), ConditionalField(nameof(spawnOnParticleCollison), Conditional.Options.Invert)]
    private bool randomSpawnPoint = false;

    [SerializeField, ConditionalField(nameof(randomSpawnPoint))]
    private Vector2 spawnPointRange = new(-5f, 5f);

    [SerializeField, Header("Behavior")]
    private bool destroyOnSpawn = false;

    [SerializeField, ConditionalField(nameof(destroyOnSpawn))]
    private UnityEvent OnBeforeSpawnerDestroyed;

    [SerializeField, Header("VFX Spawner")]
    private bool spawnOnParticleCollison;

    [SerializeField, ConditionalField(nameof(spawnOnParticleCollison)), Range(0f, 600f)]
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

            if (spawnOnParticleCollison && hasParticleSpawner)
            {
                ParticleSystem ps = particleSpawner.SpawnParticle(TerrainUtils.GetRandomPosition(Terrain.activeTerrain, yOffset), Quaternion.identity);

                if (ps.TryGetComponent(out ParticleCollisionHandler handler))
                {
                    handler.OnFirstCollision.AddListener(HandleParticleCollisonSpawn);
                }
                else
                {
                    ps.gameObject.AddComponent<ParticleCollisionHandler>()
                        .OnFirstCollision.AddListener(HandleParticleCollisonSpawn);
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
                    Entity randomEntity = GetRandomEntity();
                    SpawnEntity(randomEntity, spawnPosition, randomEntity.transform.rotation);
                }
            }

            // Break the loop if the spawner self-destructed
            if (destroyOnSpawn) break;
        }
    }

    public void HandleParticleCollisonSpawn(ParticleSystem ps, GameObject other)
    {    
        List<ParticleCollisionEvent> collisionEvents = new();
        ps.GetCollisionEvents(other, collisionEvents);

        Entity random = GetRandomEntity();
        SpawnEntity(random, collisionEvents[0].intersection, random.transform.rotation);
    }

    public Entity SpawnEntity(Entity entity, Vector3 position, Quaternion rotation)
    {
        if (!spawnOnParticleCollison && hasParticleSpawner)
        {
            // spawn a particle to mask entity spawning
            particleSpawner.SpawnParticle(position, rotation);
        }

        Entity spawnedEntity = Instantiate(entity, position, rotation);
        spawnedEntity.SetSpawnPoint(spawnOnParticleCollison ? position : transform.position);

        // set the spawn point without any offset applied
        OnEntitySpawned.Invoke(spawnedEntity.EntityType, position, rotation);

        if (spawnedEntity.EntityType == EntityType.Boss)
        {
            OnBossEntitySpawned.Invoke(spawnedEntity.EntityType);
        }

        // destroy the spawner to stop more spawns
        if (destroyOnSpawn && hasParticleSpawner)
        {
            OnBeforeSpawnerDestroyed.Invoke();
            Destroy(gameObject, particleSpawner.SpawnParticle(position, rotation).main.duration);
        }

        return spawnedEntity;
    }

    public bool CanSpawnEntity() => entities.Length > 0 && Time.timeScale > 0.0f;

    public Entity GetRandomEntity() => entities[Random.Range(0, entities.Length)];
}
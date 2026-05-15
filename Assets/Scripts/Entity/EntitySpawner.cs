using ConditionalField;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField]
    private Entity[] entities;

    [SerializeField, Header("Spawning")]
    private float respawnDelay = 10f;

    [SerializeField, ConditionalField(nameof(spawnOnCollison), Conditional.Options.Invert)]
    private bool randomSpawn = false;

    [SerializeField, ConditionalField(nameof(randomSpawn))]
    private Vector2 spawnRange = new(-5f, 5f);

    [SerializeField, Space(10)]
    private bool destroyOnSpawn = false;

    [SerializeField, Header("VFX Spawner")]
    private bool spawnOnCollison;

    [SerializeField, ConditionalField(nameof(spawnOnCollison)), Range(0f, 600f)]
    private float yOffset = 50f;

    private ParticleSpawner particleSpawner;
    private bool hasParticleSpawner = false;

    private void Awake()
    {
        if (!TryGetComponent(out particleSpawner))
        {
            foreach (Transform child in transform.root)
            {
                // search all children for a particle spawner
                if (child.TryGetComponent(out particleSpawner))
                {

                    hasParticleSpawner = true;
                    break; // stop searching after the first one is found
                }
            }
        }
        else
        {
            // parent has a particle spawner
            hasParticleSpawner = true;
        }
    }

    private void Start()
    {
        _ = SpawnLoopAsync(destroyCancellationToken);
    }

    private async Awaitable SpawnLoopAsync(CancellationToken token)
    {
        while (true)
        {
            try
            {
                // Wait for the respawn delay before spawning another entity
                await Awaitable.WaitForSecondsAsync(respawnDelay, token);
            }
            catch (System.OperationCanceledException)
            {
                // Smoothly exit the loop if the object is destroyed during the wait period
                break;
            }

            if (!CanSpawn()) break;

            if (spawnOnCollison && hasParticleSpawner)
            {
                ParticleSystem ps = particleSpawner.SpawnParticle(TerrainUtils.GetRandomPosition(Terrain.activeTerrain, yOffset), Quaternion.identity);

                if (ps.TryGetComponent(out ParticleCollisionHandler handler))
                {
                    handler.OnFirstCollision.AddListener(HandleParticleCollison);
                }
                else
                {
                    ps.gameObject.AddComponent<ParticleCollisionHandler>()
                        .OnFirstCollision.AddListener(HandleParticleCollison);
                }
            }
            else
            {
                Vector3 randomSpawnPosition = transform.position + new Vector3(
                    Random.Range(spawnRange.x, spawnRange.y), 
                    0f, 
                    Random.Range(spawnRange.x, spawnRange.y));

                Vector3 spawnPosition = randomSpawn ? randomSpawnPosition : transform.position;

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

            // Break the loop immediately if the spawner self-destructed
            if (destroyOnSpawn) break;
        }
    }

    public void HandleParticleCollison(ParticleSystem particleSystem, GameObject other)
    {    
        if (!CanSpawn()) return;

        List<ParticleCollisionEvent> collisionEvents = new();
        particleSystem.GetCollisionEvents(other, collisionEvents);

        Entity random = GetRandomEntity();
        SpawnEntity(random, collisionEvents[0].intersection, random.transform.rotation);
    }

    public Entity SpawnEntity(Entity entity, Vector3 position, Quaternion rotation)
    {
        if (!spawnOnCollison && hasParticleSpawner)
        {
            // spawn a particle to mask entity spawning
            particleSpawner.SpawnParticle(position, rotation);
        }

        if (destroyOnSpawn)
        {
            Destroy(gameObject);
        }

        return Instantiate(entity, position, rotation);
    }

    public bool CanSpawn()
    {
        return entities.Length > 0 && !GameManager.Instance.Player.IsDead();
    }

    public Entity GetRandomEntity()
    {
        return entities[Random.Range(0, entities.Length)];
    }
}
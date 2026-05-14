using ConditionalField;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    [SerializeField]
    private Entity[] entities;

    [SerializeField]
    private float respawnDelay = 10f;

    [SerializeField]
    private bool destroyOnSpawn = false;

    [SerializeField, ConditionalField(nameof(spawnOnCollison), Conditional.Options.Invert)]
    private bool randomSpawn = false;

    [SerializeField, ConditionalField(nameof(randomSpawn))]
    private Vector2 spawnRange = new(-5f, 5f);

    [Header("VFX Spawner")]
    [SerializeField]
    private ParticleSpawner spawner;

    [SerializeField, ConditionalField(nameof(spawner))]
    private bool spawnOnCollison;

    [SerializeField, ConditionalField(nameof(spawnOnCollison)), Range(0f, 600f)]
    private float yOffset = 50f;

    private WaitForSeconds waitForRespawnDelay;

    void Awake()
    {
        waitForRespawnDelay = new WaitForSeconds(respawnDelay);
    }

    void Start()
    { 
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return waitForRespawnDelay; // wait before spawning another entity

            if (!CanSpawn()) break;

            if (spawnOnCollison)
            {
                ParticleSystem ps = spawner.SpawnParticle(TerrainUtils.GetRandomPosition(Terrain.activeTerrain, yOffset), Quaternion.identity);

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
        if (!spawnOnCollison)
        {
            // spawn a particle to mask entity spawning
            spawner.SpawnParticle(position, rotation);
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
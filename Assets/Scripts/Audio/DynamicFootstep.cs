using AYellowpaper.SerializedCollections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(FirstPersonController))]
public class DynamicFootstep : MonoBehaviour
{
    [SerializeField, SerializedDictionary("Surface Type", "Audio Clips")]
    private SerializedDictionary<SurfaceType, AudioClip[]> surfaceAudioTypes;

    [Header("Footsteps")]
    [SerializeField, Tooltip("Time between steps at normal walking speed")] 
    private float stepInterval = 0.5f;

    [SerializeField, Tooltip("Ignore tiny movements")] 
    private float minimumSpeedThreshold = 0.1f;

    [Header("Speed Scaling")]
    [SerializeField] 
    private float walkSpeedScale = 1f;

    [SerializeField] 
    private float sprintSpeedScale = 1.6f;

    [Header("Audio")]
    [SerializeField]
    private AudioSource footstepSource;

    [SerializeField, Range(0.8f, 1.0f)] 
    private float minPitch = 0.92f;

    [SerializeField, Range(1.0f, 1.2f)]
    private float maxPitch = 1.08f;

    [SerializeField, Range(0f, 1f)]
    private float volumeScale = 0.05f;

    private float footstepTimer = 0f;

    private FirstPersonController playerController;
    private readonly RaycastHit[] groundRayHitResults = new RaycastHit[1];

    private void Awake()
    {
        playerController = GetComponent<FirstPersonController>();
    }

    private void Start()
    {
        footstepTimer = 0f;
    }

    private void Update()
    {
        if (!playerController.IsGrounded) return;

        // the current player speed
        float currentSpeed = playerController.Velocity.magnitude;

        // reset timer when standing still so steps trigger instantly when starting to walk
        if (currentSpeed < minimumSpeedThreshold || !playerController.IsMovingForward)
        {
            footstepTimer = stepInterval;
            return;
        }

        float currentSpeedPerc = Mathf.InverseLerp(playerController.MoveSpeed, playerController.SprintSpeed, currentSpeed);
        float smoothSpeedMultiplier = Mathf.Lerp(walkSpeedScale, sprintSpeedScale, currentSpeedPerc);

        footstepTimer += Time.deltaTime * smoothSpeedMultiplier;
        if (footstepTimer >= stepInterval)
        {
            TriggerFootstep();
            footstepTimer = 0f; // Reset footstep timer
        }
    }

    private void TriggerFootstep()
    {
        // if the ray hit a collider on a ground layer
        if (Physics.RaycastNonAlloc(transform.position, Vector3.down, groundRayHitResults, Mathf.Infinity, playerController.GroundLayers) > 0)
        {
            RaycastHit groundHitInfo = groundRayHitResults[0];

            if (!groundHitInfo.collider || !groundHitInfo.collider.TryGetComponent(out SurfaceMaterial material)) return;

            // play a random footstep sound based on the type of surface stepped on
            if (surfaceAudioTypes.TryGetValue(material.SurfaceType, out AudioClip[] clips) && clips.Length > 0)
            {
                // apply a random pitch variation before playing the clip
                footstepSource.pitch = Random.Range(minPitch, maxPitch);

                // play a random footstep clip
                footstepSource.PlayOneShot(clips[Random.Range(0, clips.Length)], volumeScale);
            }
        }
    }
}

using AYellowpaper.SerializedCollections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DynamicSurfaceFootstep : MonoBehaviour
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
    [SerializeField, Range(0.8f, 1.0f)] 
    private float minPitch = 0.92f;

    [SerializeField, Range(1.0f, 1.2f)]
    private float maxPitch = 1.08f;

    [SerializeField, Range(0f, 1f)]
    private float volumeScale = 0.5f;

    private readonly RaycastHit[] hitRays = new RaycastHit[10];
    private AudioSource playerSource;
    private FirstPersonController playerController;
    private SurfaceMaterial currentSurfaceMaterial;

    private float footstepTimer = 0f;
    private bool hasPlayerController = false;

    private void Awake()
    {
        playerSource = GetComponent<AudioSource>();
        hasPlayerController = TryGetComponent(out playerController);
    }

    private void Update()
    {
        if (!hasPlayerController) return;

        // Get the current speed magnitude
        float currentSpeed = playerController.CurrentVelocity.magnitude;

        // Only progress timer if player is moving fast enough and pushing the forward input
        if (currentSpeed < minimumSpeedThreshold || Mathf.Approximately(playerController.InputController.Move.y, 0f))
        {
            // reset timer when standing still so steps trigger instantly when starting to walk
            footstepTimer = stepInterval;
            return;
        }

        float speedPercentage = Mathf.InverseLerp(playerController.MoveSpeed, playerController.SprintSpeed, currentSpeed);
        float smoothedSpeedMultiplier = Mathf.Lerp(walkSpeedScale, sprintSpeedScale, speedPercentage);

        footstepTimer += Time.deltaTime * smoothedSpeedMultiplier;

        // Trigger the footstep once the threshold is crossed
        if (footstepTimer >= stepInterval)
        {
            TriggerFootstep();
            footstepTimer = 0f; // Reset timer
        }
    }

    private void TriggerFootstep()
    {
        if (Physics.RaycastNonAlloc(transform.position, Vector3.down, hitRays) <= 0)
        {
            return;
        }

        foreach (RaycastHit raycastHit in hitRays)
        {
            if (!raycastHit.collider || !raycastHit.collider.TryGetComponent(out currentSurfaceMaterial))
            {
                continue;
            }

            Debug.Log(currentSurfaceMaterial);

            if (surfaceAudioTypes.TryGetValue(currentSurfaceMaterial.SurfaceType, out AudioClip[] clips) && clips.Length > 0)
            {
                // apply a random pitch variation before playing the sound
                playerSource.pitch = Random.Range(minPitch, maxPitch);

                // play a random clip
                playerSource.PlayOneShot(clips[Random.Range(0, clips.Length)], volumeScale);

                break; // exit loop once clip is played
            }
        }
    }
}

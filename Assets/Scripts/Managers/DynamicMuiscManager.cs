using UnityEngine;

public class DynamicMusicManager : MonoBehaviour
{
    public static DynamicMusicManager Instance { get; private set; }
    
    [SerializeField]
    private AudioSource musicAudioSource;

    [SerializeField]
    private float fadeInVolume = 1f;

    [SerializeField]
    private float fadeOutVolume = 0f;

    [SerializeField]
    private float fadeDuration = 2.0f;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    public async void StartNewTrack(EntityType type)
    {
        // Only support boss music for now
        if (type != EntityType.Boss) return;
        
        // Fade out current music completely
        await AudioFader.FadeVolumeAsync(musicAudioSource, fadeOutVolume, fadeDuration);

        // Change the clip and play it silently
        musicAudioSource.volume = 0f;
        musicAudioSource.Play();

        // Fade the new music in
        await AudioFader.FadeVolumeAsync(musicAudioSource, fadeInVolume, fadeDuration);
    }
}

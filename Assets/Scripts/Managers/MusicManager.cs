using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance { get; private set; }

    [Header("Dynamic Music")]
    [SerializeField]
    private AudioSource musicSource;

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

    public async void StartTrack(EntityType type)
    {
        if (type != EntityType.Boss) return;
        
        // Fade out current music completely
        await AudioFader.FadeVolumeAsync(musicSource, fadeOutVolume, fadeDuration);

        // Change the clip and play it silently
        musicSource.volume = 0f;
        musicSource.Play();

        // Fade the new music in
        await AudioFader.FadeVolumeAsync(musicSource, fadeInVolume, fadeDuration);
    }
}

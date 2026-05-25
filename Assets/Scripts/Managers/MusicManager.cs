using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : Singleton<MusicManager>
{
    [Header("Dynamic Music")]
    [SerializeField]
    private float fadeInVolume = 1f;

    [SerializeField]
    private float fadeOutVolume = 0f;

    [SerializeField]
    private float fadeDuration = 2.0f;

    private AudioSource musicSource;

    protected override void Awake()
    {
        musicSource = GetComponent<AudioSource>();

        base.Awake();
    }

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnload;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnload;
    }

    public void OnSceneUnload(Scene scene)
    {
        // stop the music from playing if we go to the main menu
        musicSource.Stop();
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

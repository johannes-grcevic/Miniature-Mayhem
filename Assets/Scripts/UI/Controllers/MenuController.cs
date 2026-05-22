using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MenuController : MonoBehaviour
{
    [SerializeField]
    private AudioClip musicClip;

    private AudioSource musicSource;

    private const string KeyMusicVolume = "Setting_Menu_Music_Volume";

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
        
        if (PlayerPrefs.HasKey(KeyMusicVolume))
        {
            OnMusicVolumeChanged(PlayerPrefs.GetFloat(KeyMusicVolume, musicSource.volume));
        }
    }

    private void OnEnable()
    {
        Application.quitting += SavePrefs;
    }

    private void OnDisable()
    {
        Application.quitting -= SavePrefs;
    }

    private void Start()
    {
        musicSource.clip = musicClip;

        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    public void OnMusicVolumeChanged(float value)
    {
        musicSource.volume = value;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneNames.MainGame);
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void SavePrefs()
    {
        // save the menu music volume
        PlayerPrefs.SetFloat(KeyMusicVolume, musicSource.volume);
    }
}

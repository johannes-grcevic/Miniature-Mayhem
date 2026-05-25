using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MenuMusicController : MonoBehaviour
{
    [SerializeField]
    private AudioClip musicClip;

    [SerializeField]
    private Slider musicVolumeSlider;

    private AudioSource musicSource;

    private const string KeyMusicVolume = "Setting_Menu_Music_Volume";

    private void Awake()
    {
        musicSource = GetComponent<AudioSource>();
        LoadMusicVolume();
    }

    private void Start()
    {
        musicSource.clip = musicClip;
        musicSource.Play();
    }

    private void OnEnable()
    {
        LoadMusicVolume();
    }

    private void OnDisable()
    {
        SaveMusicVolume(musicSource.volume);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneNames.MainGame);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void OnMusicVolumeChanged(float value)
    {
        musicSource.volume = value;
        SaveMusicVolume(value);
    }

    private void SaveMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(KeyMusicVolume, value);
        PlayerPrefs.Save();
    }

    private void LoadMusicVolume()
    {
        if (PlayerPrefs.HasKey(KeyMusicVolume))
        {
            float volume = PlayerPrefs.GetFloat(KeyMusicVolume, 1f);

            musicSource.volume = volume;
            musicVolumeSlider.value = volume;
        }
    }
}

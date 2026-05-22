using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public EntityPlayer Player => player;

    private EntityPlayer player;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (!player && playerObject)
        {
            if (!playerObject.TryGetComponent(out player))
            {
                Debug.LogWarning($"[{this}] Could not find a player in the scene.");
            }
        }
    }

    public void SetCursor(bool visible, CursorLockMode lockMode)
    {
        Cursor.visible = visible;
        Cursor.lockState = lockMode;
    }

    public void StartGame()
    {
        LoadScene(SceneNames.MainGame);
    }

    public void LoadMainMenu()
    {
        LoadScene(SceneNames.MainMenu);
    }

    public void RestartCurrentLevel()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit(int exitCode = 0)
    {
#if UNITY_EDITOR        
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(exitCode);
#endif
    }

    private void LoadScene(string name, LoadSceneMode mode = LoadSceneMode.Single)
    {
        SceneManager.LoadScene(name, mode);
    }
}

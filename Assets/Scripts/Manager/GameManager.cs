using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public EntityPlayer Player { get; private set; }

    [SerializeField]
    private UnityEvent<GameState> OnLevelLoad = new();

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            Player = FindFirstObjectByType<EntityPlayer>();
        }

        DontDestroyOnLoad(gameObject);
    }

    public void SetCursor(bool visible, CursorLockMode lockMode)
    {
        Cursor.visible = visible;
        Cursor.lockState = lockMode;
    }

    public void RestartCurrentLevel()
    {
        LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
        OnLevelLoad.Invoke(GameState.Running);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
#if UNITY_EDITOR        
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }
}

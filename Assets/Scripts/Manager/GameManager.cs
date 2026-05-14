using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    public EntityPlayer Player { get; private set; }

    [SerializeField]
    private UnityEvent<GameState> OnLevelRestart = new();

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            Player = FindFirstObjectByType<EntityPlayer>();

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
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
        OnLevelRestart.Invoke(GameState.Running);
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

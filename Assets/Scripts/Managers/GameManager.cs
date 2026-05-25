using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public EntityPlayer Player => player;
    public Transform PlayerTransform => playerTransform;

    private EntityPlayer player;
    private Transform playerTransform;

    protected override void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        base.Awake();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        switch (scene.name)
        {
            case SceneNames.MainGame:
                FindPlayerInScene();
                break;
            case SceneNames.MainMenu:
                GameController.Instance.ChangeGameState(GameState.MainMenu);
                break;
            default:
                break;
        }
    }

    public void SetCursor(bool visible, CursorLockMode lockMode)
    {
        Cursor.visible = visible;
        Cursor.lockState = lockMode;
    }

    public void Quit()
    {
#if UNITY_EDITOR        
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void LoadMainMenu()
    {
        SetCursor(true, CursorLockMode.None);
        LoadScene(SceneNames.MainMenu);
    }

    public void ReloadCurrentLevel()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    private void FindPlayerInScene()
    {
        var playerGO = GameObject.FindWithTag("Player");

        if (!playerGO)
        {
            Debug.LogError($"[{this}] Could not find an active player in the scene.");
            return;
        }

        if (playerGO.TryGetComponent(out player))
        {
            playerTransform = player.transform;
        }
    }

    private void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
}
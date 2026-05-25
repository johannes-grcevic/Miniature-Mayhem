using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : Singleton<GameController>
{
    public GameState CurrentState => currentState;

    [SerializeField, Header("Input")]
    private InputActionReference pauseAction;

    [SerializeField, SerializedDictionary("Game State", "UI Elements")]
    private SerializedDictionary<GameState, List<GameObject>> UIElements;

    private GameState currentState;

    protected override void Awake()
    {
        pauseAction.action.performed += OnPauseButtonPressed;
        SceneManager.sceneLoaded += OnSceneLoad;

        base.Awake();
    }

    private void Start()
    {
        GameManager.Instance.Player.OnDeath += () => ChangeGameState(GameState.Over);
        ChangeGameState(GameState.Running);
    }

    private void OnDestroy()
    {
        pauseAction.action.performed -= OnPauseButtonPressed;
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    public void ChangeGameState(GameState newState)
    {
        currentState = newState;

        // call game state changed event
        OnGameStateChanged(newState);
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode sceneMode)
    {
        ChangeGameState(GameState.Running);
    }

    private void OnGameStateChanged(GameState newState)
    {
        OnUIDraw();

        // freeze time when the game is not running
        Time.timeScale = newState == GameState.Running ? 1.0f : 0.0f;

        switch (currentState)
        {
            case GameState.Over:
                GameManager.Instance.SetCursor(true, CursorLockMode.None);
                AudioListener.volume = 0.0f;
                break;
            case GameState.Win:
                GameManager.Instance.SetCursor(true, CursorLockMode.None);
                AudioListener.volume = 0.0f;
                break;
            case GameState.Paused:
                GameManager.Instance.SetCursor(true, CursorLockMode.None);
                AudioListener.volume = 0.0f;
                break;
            case GameState.Running:
                GameManager.Instance.SetCursor(false, CursorLockMode.Locked);
                AudioListener.volume = 1.0f;
                break;
            case GameState.MainMenu:
                GameManager.Instance.SetCursor(true, CursorLockMode.None);
                AudioListener.volume = 1.0f;
                break;
            default:
                break;
        }
    }

    private void OnPauseButtonPressed(InputAction.CallbackContext context)
    {
        if (currentState == GameState.Over || currentState == GameState.Win) return;

        // set pause state
        ChangeGameState(currentState == GameState.Running ? GameState.Paused : GameState.Running);
    }

    private void OnUIDraw()
    {
        if (UIElements == null || UIElements.Count == 0) return;
        
        foreach (var element in UIElements)
        {
            if (!UIElements.TryGetValue(element.Key, out List<GameObject> gameObjects)) continue;

            foreach (GameObject go in gameObjects)
            {
                if (!go) continue;
                
                // enable the ui element if it matches the current state
                go.SetActive(element.Key == currentState);

                if (go.activeSelf && go.CompareTag("UIMenu") && go.TryGetComponent(out Animator animator))
                {
                    animator.SetBool(Animator.StringToHash("IsOpen"), go.activeSelf);
                }
            }
        }
    }
}

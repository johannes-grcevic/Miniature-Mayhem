using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference pauseAction;

    [SerializedDictionary("Game State", "Elements")]
    public SerializedDictionary<GameState, List<GameObject>> UIElements;

    private GameState currentState;
    private GameState previousState;
    private GameManager gameManager;
    
    private void Awake()
    {
        gameManager = GameManager.Instance;
        gameManager.GetPlayer().OnDeath.AddListener(SetGameState);
        gameManager.GetWaveController().OnWaveEnded.AddListener(SetGameState);

        pauseAction.action.performed += OnPauseButtonPressed;

        DontDestroyOnLoad(gameObject);
    }

    protected void OnPauseButtonPressed(InputAction.CallbackContext context)
    {
        if (pauseAction == null) return;
        
        // toggle pause state
        SetGameState(currentState == GameState.Paused ? previousState : GameState.Paused);

        // toggle global volume
        AudioListener.volume = currentState == GameState.Paused ? 0f : 1f;
    }

    protected void OnGameStateChanged(GameState state)
    {
        DrawUI(state);

        // freeze game time when the game is not running
        Time.timeScale = state == GameState.Active ? 1f : 0f;

        switch (currentState)
        {
            case GameState.GameOver:
            case GameState.Success:
            case GameState.Paused:
                gameManager.SetCursor(true, CursorLockMode.None);
                break;
            case GameState.Active:
                gameManager.SetCursor(false, CursorLockMode.Locked);
                break;
            default:
                gameManager.SetCursor(true, CursorLockMode.None);
                break;
        }
    }

    public void DrawUI(GameState state)
    {
        if (UIElements == null || UIElements.Values == null) return;
        
        foreach (var element in UIElements)
        {
            element.Value.ForEach(go => go.SetActive(element.Key == state));
        }
    }

    public void SetGameState(GameState state)
    {
        previousState = currentState;
        currentState = state;
        OnGameStateChanged(state);
    }

    public GameState GetGameState() => currentState;
}

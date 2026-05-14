using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateController : MonoBehaviour
{
    [SerializeField, SerializedDictionary("Game State", "UI Elements")]
    private SerializedDictionary<GameState, List<GameObject>> UIElements;

    [SerializeField]
    private InputActionReference pauseAction;

    private GameState currentState;
    private GameState previousState;

    private void Awake()
    {
        pauseAction.action.performed += OnPauseButtonPressed;
    }

    private void Start()
    {
        GameManager.Instance.Player.OnDeath.AddListener(SetGameState);
        SetGameState(GameState.Running);
    }

    public void SetGameState(GameState state)
    {
        previousState = currentState;
        currentState = state;

        // call game state changed event handler
        OnGameStateChanged(state);
    }

    public GameState GetGameState() => currentState;

    private void OnGameStateChanged(GameState newState)
    {
        OnUIDraw();

        // freeze time when the game is not running
        Time.timeScale = newState == GameState.Running ? 1.0f : 0.0f;

        switch (currentState)
        {
            case GameState.Over:
            case GameState.Win:
            case GameState.Paused:
                GameManager.Instance.SetCursor(true, CursorLockMode.None);
                break;
            case GameState.Running:
                GameManager.Instance.SetCursor(false, CursorLockMode.Locked);
                break;
            case GameState.None:
                break;
            default:
                break;
        }
    }

    private void OnPauseButtonPressed(InputAction.CallbackContext context)
    {
        if (pauseAction == null) return;

        // set pause state
        SetGameState(currentState == GameState.Paused ? previousState : GameState.Paused);

        // set global volume state
        AudioListener.volume = currentState == GameState.Paused ? 0f : 1f;
    }

    private void OnUIDraw()
    {
        if (UIElements == null || UIElements.Count == 0) return;
        
        foreach (var element in UIElements)
        {
            if (!UIElements.TryGetValue(element.Key, out List<GameObject> elements))
            {
                continue;
            }

            foreach (GameObject go in elements)
            {
                go.SetActive(element.Key == currentState);
            }
        }
    }
}

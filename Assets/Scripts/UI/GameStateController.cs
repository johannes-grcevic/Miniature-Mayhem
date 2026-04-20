using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameStateController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference pauseAction;

    [SerializeField]
    private Entity player;
    
    [SerializeField]
    private WaveController waveController;

    [SerializedDictionary("Game State", "Elements")]
    public SerializedDictionary<GameState, List<GameObject>> UIElements;

    private GameState currentState;
    
    void Awake()
    {
        waveController.OnWaveEnded.AddListener(SetGameState);
        player.OnDeath.AddListener(SetGameState);
        pauseAction.action.performed += OnPauseButtonPressed;
    }

    protected void OnPauseButtonPressed(InputAction.CallbackContext context)
    {
        // toggle pause state
        SetGameState(currentState == GameState.Paused ? GameState.Active : GameState.Paused);
    }

    protected void OnGameStateChanged(GameState state)
    {
        DrawUI(state);

        // freeze the game time
        Time.timeScale = state == GameState.Active ? 1.0f : 0.0f;
    }

    public void DrawUI(GameState state)
    {
        foreach (var element in UIElements)
        {
            element.Value.ForEach(go => go.SetActive(element.Key == state));
        }
    }

    public void SetGameState(GameState state)
    {
        currentState = state;
        OnGameStateChanged(state);
    }

    public GameState GetGameState()
    {
        return currentState;
    }
}

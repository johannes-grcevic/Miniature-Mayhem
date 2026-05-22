using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    public UnityEvent<GameState> OnStateChanged = new();
    
    public GameState CurrentState => currentState;
    public GameState PreviousState => previousState;

    [SerializeField, Header("Input")]
    private InputActionReference pauseAction;

    [SerializeField, SerializedDictionary("Game State", "UI Elements")]
    private SerializedDictionary<GameState, List<GameObject>> UIElements;

    private GameState currentState;
    private GameState previousState;

    private static readonly int IsMenuOpenHash = Animator.StringToHash("IsOpen");

    private void Awake()
    {
        pauseAction.action.performed += OnPauseButtonPressed;
    }

    private void Start()
    {
        GameManager.Instance.Player.OnDeath += ChangeGameState;
        ChangeGameState(GameState.Running);
    }

    private void OnDisable()
    {
        ChangeGameState(GameState.None);
    }

    public void ChangeGameState(GameState newState)
    {
        previousState = currentState;
        currentState = newState;

        // call game state changed event handler
        OnGameStateChanged(newState);
    }

    private void OnGameStateChanged(GameState newState)
    {
        OnUIDraw();

        // freeze time when the game is not running
        Time.timeScale = newState == GameState.Running ? 1.0f : 0.0f;

        switch (currentState)
        {
            case GameState.Over:
                break;
            case GameState.Win:
                break;
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

        OnStateChanged.Invoke(newState);
    }

    private void OnPauseButtonPressed(InputAction.CallbackContext context)
    {
        if (pauseAction == null) return;

        // set pause state
        ChangeGameState(currentState == GameState.Paused ? previousState : GameState.Paused);

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
                if (!go) continue;
                
                go.SetActive(element.Key == currentState);

                if (go.activeSelf && go.CompareTag("UIMenu") && go.TryGetComponent(out Animator animator))
                {
                    animator.SetBool(IsMenuOpenHash, go.activeSelf);
                }
            }
        }
    }
}

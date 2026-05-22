using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameTimerController : MonoBehaviour
{
    public UnityEvent<GameState> OnGameTimerStopped = new();

    [Header("Timer")]
    [SerializeField]
    private float gameDuration = 120f;

    [SerializeField]
    private string displayName = "Timer";

    [SerializeField]
    private TMP_Text textLabel;

    private Timer gameTimer;

    private void Awake()
    {
        gameTimer = new Timer(displayName, textLabel);
        gameTimer.OnStopped += OnTimerStopped;
    }

    private void OnDestroy()
    {
        gameTimer.OnStopped -= OnTimerStopped;
    }

    private void Start()
    {
        gameTimer.Start(this, gameDuration);
    }

    protected void OnTimerStopped()
    {
        OnGameTimerStopped.Invoke(GameState.Win);
    }
}

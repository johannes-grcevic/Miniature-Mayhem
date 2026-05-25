using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
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
        gameTimer.OnStopped += OnGameTimerStopped;
    }

    private void OnDestroy()
    {
        gameTimer.OnStopped -= OnGameTimerStopped;
    }

    private void Start()
    {
        gameTimer.Start(this, gameDuration);
    }

    public void OnGameTimerStopped()
    {
        GameController.Instance.ChangeGameState(GameState.Win);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class WaveController : MonoBehaviour
{
    public UnityEvent<GameState> OnWaveTimerStopped = new();
    
    [SerializeField, Tooltip("The duration of each wave in seconds.")]
    private float waveDuration = 120f;

    [Header("Timer")]
    [SerializeField]
    private string displayName = "Timer";

    [SerializeField]
    private TMP_Text label;

    private Timer timer;

    private void Awake()
    {
        timer = new Timer(displayName, label);
        timer.OnStopped.AddListener(OnTimerStopped);
    }

    private void Start()
    {
        timer.Start(this, waveDuration);
    }

    protected void OnTimerStopped()
    {
        OnWaveTimerStopped.Invoke(GameState.Win);
    }
}

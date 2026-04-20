using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Timer
{
    public UnityEvent OnStarted { get; private set; } = new();
    public UnityEvent OnStopped { get; private set; } = new();

    public float Duration => timerDuration;
    public bool IsRunning => isTimerRunning;

    private string timerName = string.Empty;
    private TMP_Text timerLabel;

    private float timerDuration = 0f;
    private bool isTimerRunning = false;

    public Timer(string name, TMP_Text label)
    {
        timerName = name;
        timerLabel = label;
    }

    public Timer(string name, Vector2 position, TextAlignmentOptions alignment = TextAlignmentOptions.Midline)
    {
        CreateUI(name, position, alignment);      
    }

    protected IEnumerator TimerUpdateLoop()
    {
        while (isTimerRunning)
        {            
            if (timerDuration <= 0f)
            {
                Stop();
                yield break;
            }

            timerDuration -= Time.deltaTime;
            DisplayTime(timerName, timerLabel, timerDuration);

            yield return null; // Wait one frame
        }
    }

    protected void CreateUI(string name, Vector2 position, TextAlignmentOptions alignmentMode)
    {
        timerName = name;
        Canvas timerCanvas = new GameObject(nameof(Timer)).AddComponent<Canvas>();
        timerLabel = new GameObject("Label").AddComponent<TextMeshProUGUI>();
        timerLabel.transform.SetParent(timerCanvas.transform);

        timerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        timerCanvas.vertexColorAlwaysGammaSpace = true;
        timerLabel.rectTransform.anchoredPosition = position;
        timerLabel.alignment = alignmentMode;

        timerCanvas.gameObject.AddComponent<GraphicRaycaster>();

        Resolution screenResolution = Screen.currentResolution;
        var canvasScaler = timerCanvas.gameObject.AddComponent<CanvasScaler>();

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(screenResolution.width, screenResolution.height);
    }

    protected void DisplayTime(string timerName, TMP_Text timeLabel, float timeToDisplay)
    {
        timeToDisplay += 1;
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        StringBuilder sb = new();
        sb.AppendWithSpace(timerName);
        sb.AppendFormat("{0:00}:{1:00}", minutes, seconds);

        timeLabel.SetText(sb.ToString());
    }

    public void Start(MonoBehaviour host, float duration)
    {
        if (isTimerRunning) return;
        
        timerDuration = duration;
        isTimerRunning = true;

        OnStarted.Invoke();
        timerLabel.gameObject.SetActive(true);

        host.StartCoroutine(TimerUpdateLoop());
    }

    public void Stop()
    {
        isTimerRunning = false;
        timerDuration = 0f;
        OnStopped.Invoke();
        timerLabel.gameObject.SetActive(false);
    }

    public string GetName()
    {
        return timerName;
    }

    public TMP_Text GetLabel()
    {
        return timerLabel;
    }
}

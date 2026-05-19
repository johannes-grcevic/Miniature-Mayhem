using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Timer
{
    public UnityAction OnStarted;
    public UnityAction OnStopped;

    public float Duration => timerDuration;
    public bool IsRunning => isTimerRunning;
    public string Name => timerName;
    public TMP_Text TextLabel => timerTextLabel;

    private string timerName;
    private TMP_Text timerTextLabel;

    private float timerDuration = 0f;
    private bool isTimerRunning = false;

    public Timer(string name, TMP_Text label)
    {
        timerName = name;
        timerTextLabel = label;
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
            DisplayTime(timerName, timerTextLabel, timerDuration);

            yield return null; // Wait one frame
        }
    }

    protected void CreateUI(string name, Vector2 position, TextAlignmentOptions alignmentMode)
    {
        timerName = name;
        Canvas timerCanvas = new GameObject(nameof(Timer)).AddComponent<Canvas>();
        timerTextLabel = new GameObject("Label").AddComponent<TextMeshProUGUI>();
        timerTextLabel.transform.SetParent(timerCanvas.transform);

        timerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        timerCanvas.vertexColorAlwaysGammaSpace = true;
        timerTextLabel.rectTransform.anchoredPosition = position;
        timerTextLabel.alignment = alignmentMode;

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
        sb.Append(timerName).Append(':').Append(' ');
        sb.AppendFormat("{0:00}:{1:00}", minutes, seconds);

        timeLabel.SetText(sb.ToString());
    }

    public void Start(MonoBehaviour host, float duration)
    {
        if (isTimerRunning) return;
        
        timerDuration = duration;
        isTimerRunning = true;

        OnStarted?.Invoke();
        timerTextLabel.gameObject.SetActive(true);

        host.StartCoroutine(TimerUpdateLoop());
    }

    public void Stop()
    {
        isTimerRunning = false;
        timerDuration = 0f;
        OnStopped?.Invoke();
        timerTextLabel.gameObject.SetActive(false);
    }
}

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(GameStateController))]

[RequireComponent(typeof(WaveController))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    public UnityEvent<int> OnSceneReload { get; private set; } = new();
    public UnityEvent OnGameQuit { get; private set; } = new();

    [SerializeField]
    private CursorLockMode cursorLockMode;

    private EntityPlayer player;
    private WaveController waveController;
    private GameStateController stateController;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);

        player = FindFirstObjectByType<EntityPlayer>();
        stateController = GetComponent<GameStateController>();
        waveController = GetComponent<WaveController>();
    }

    private void Start()
    {
        SetCursor(true, cursorLockMode);
    }

    public void SetCursor(bool visible, CursorLockMode lockMode)
    {
        cursorLockMode = lockMode;

        Cursor.visible = visible;
        Cursor.lockState = lockMode;
    }

    public void ReloadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
        Time.timeScale = 1f;
        stateController.SetGameState(GameState.Active);
    }

    public void ReloadScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;

        OnSceneReload.Invoke(index);
        ReloadScene(index);
    }

    public void Quit()
    {
        OnGameQuit.Invoke();
        Application.Quit();
    }

    public EntityPlayer GetPlayer() => player;

    public Transform GetPlayerTransform() => player.transform;

    public GameStateController GetGameStateController() => stateController;

    public WaveController GetWaveController() => waveController;

    public bool IsPaused() => stateController.GetGameState() != GameState.Active;
}

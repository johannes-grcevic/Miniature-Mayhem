using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }

    public UnityEvent<int> OnSceneReload { get; private set; } = new();
    public UnityEvent OnGameQuit { get; private set; } = new();

    [SerializeField]
    private CursorLockMode cursorLockMode;

    private EntityPlayer player;

    void Awake()
    {
        if (Instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        player = FindFirstObjectByType<EntityPlayer>();
    }

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = cursorLockMode;
    }

    public void ReloadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void ReloadScene()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        OnSceneReload.Invoke(index);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        OnGameQuit.Invoke();
        Application.Quit();
    }

    public EntityPlayer GetPlayer()
    {
        return player;
    }

    public Transform GetPlayerTransform()
    {
        return player.transform;
    }
}

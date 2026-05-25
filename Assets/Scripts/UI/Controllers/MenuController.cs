using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField]
    private Button[] restartButtons;

    [SerializeField]
    private Button[] mainMenuButtons;

    [SerializeField]
    private Button[] quitButtons;

    private void Start()
    {
        if (GameManager.Instance)
        {
            AddButtonListeners(restartButtons, GameManager.Instance.ReloadCurrentLevel);
            AddButtonListeners(mainMenuButtons, GameManager.Instance.LoadMainMenu);
            AddButtonListeners(quitButtons, GameManager.Instance.Quit);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance)
        {
            RemoveButtonListeners(restartButtons, GameManager.Instance.ReloadCurrentLevel);
            RemoveButtonListeners(mainMenuButtons, GameManager.Instance.LoadMainMenu);
            RemoveButtonListeners(quitButtons, GameManager.Instance.Quit);
        }
    }

    private void AddButtonListeners(Button[] buttons, UnityAction call)
    {
        foreach (var button in buttons)
        {
            button.onClick.AddListener(call);
        }
    }

    private void RemoveButtonListeners(Button[] buttons, UnityAction call)
    {
        foreach (var button in buttons)
        {
            button.onClick.RemoveListener(call);
        }
    }
}

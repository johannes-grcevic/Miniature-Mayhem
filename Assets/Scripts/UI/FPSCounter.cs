using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSCounter : MonoBehaviour
{
    [SerializeField]
    private InputActionReference debugModeAction;

    [SerializeField] 
    private TMP_Text displayLabel;

    [SerializeField, Tooltip("Refresh rate in seconds")]
    private float refreshRate = 0.1f;

    private WaitForSeconds waitForRefresh;
    private float frameCount;
    private bool displayToggle;

    private void Awake()
    {
        debugModeAction.action.performed += ToggleDisplay;

        waitForRefresh = new WaitForSeconds(refreshRate);

        // fps counter is off by default, unless in the editor
        displayLabel.gameObject.SetActive(Application.isEditor);
    }

    private IEnumerator Start()
    {
        while (true)
        {        
            frameCount = 1f / Time.unscaledDeltaTime;
            frameCount = MathF.Round(frameCount, 1);
            displayLabel.SetText(frameCount.ToString());

            switch (frameCount)
            {
                case > 30:
                    displayLabel.color = Color.green;
                    break;
                case > 10:
                    displayLabel.color = Color.yellow;
                    break;
                case < 10:
                    displayLabel.color = Color.red;
                    break;
            }

            yield return waitForRefresh;
        }
    }

    private void ToggleDisplay(InputAction.CallbackContext context)
    {
        displayToggle = !displayToggle;
        displayLabel.gameObject.SetActive(displayToggle);
    }
}
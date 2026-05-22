using UnityEngine;
using UnityEngine.UI;

public class ImageFillController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] 
    private Image fillImage;

    [Header("Fill")]
    [SerializeField] 
    private float minValue = 0f;

    [SerializeField] 
    private float maxValue = 100f;

    [Header("Animation")]
    [SerializeField] 
    private float fillSpeed = 5f;

    private float targetFillAmount = 1f;

    private void Start()
    {
        if (fillImage == null) return;

        targetFillAmount = fillImage.fillAmount;
    }

    private void Update()
    {
        if (fillImage == null) return;

        ApplySmoothFill();
    }

    public void SetFillValue(float currentValue)
    {
        if (fillImage == null) return;

        // Calculate the target fill but don't apply it instantly
        float normalizedValue = Mathf.InverseLerp(minValue, maxValue, currentValue);
        targetFillAmount = Mathf.Clamp01(normalizedValue);
    }

    public void SetMaxValue(float newMax)
    {
        maxValue = newMax;
    }

    private void ApplySmoothFill()
    {
        // Skips calculation if the bar has already reached its destination
        if (Mathf.Approximately(fillImage.fillAmount, targetFillAmount)) return;

        // Smoothly move the fill bar toward the target value
        fillImage.fillAmount = Mathf.MoveTowards(fillImage.fillAmount, targetFillAmount, fillSpeed * Time.deltaTime);
    }
}

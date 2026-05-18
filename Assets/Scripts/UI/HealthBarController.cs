using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField]
    private Entity targetEntity;

    [SerializeField]
    private Image fillImage;

    private void Awake()
    {
        targetEntity.OnHealthChanged += SetFillValue;
    }

    private void OnDestroy()
    {
        targetEntity.OnHealthChanged -= SetFillValue;
    }

    public void SetFillValue(int value)
    {
        // normalize health value and set the fill amount
        fillImage.fillAmount = Mathf.InverseLerp(0, targetEntity.MaxHealth, value);
    }
}

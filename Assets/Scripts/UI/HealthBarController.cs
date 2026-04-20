using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [SerializeField]
    private Entity entity;

    [SerializeField]
    private Image fillImage;

    private void Awake()
    {
        entity.OnHealthChanged.AddListener(SetFillValue);
    }

    public void SetFillValue(int value)
    {
        // normalize health value and set the fill amount
        fillImage.fillAmount = Mathf.InverseLerp(0, entity.GetMaxHealth(), value);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaController : MonoBehaviour
{
    [SerializeField]
    private EntityPlayer entityPlayer;

    [SerializeField]
    private Image fillImage;

    private void Awake()
    {
        entityPlayer.OnStaminaChanged.AddListener(SetFillValue);
    }

    public void SetFillValue(float value)
    {
        // normalize health value and set the fill amount
        fillImage.fillAmount = Mathf.InverseLerp(0, entityPlayer.GetMaxStamina(), value);
    }
}

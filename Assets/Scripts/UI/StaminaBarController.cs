using UnityEngine;
using UnityEngine.UI;

public class StaminaBarController : MonoBehaviour
{
    [SerializeField]
    private Image fillImage;

    private EntityPlayer player;

    private void Start()
    {
        player = GameManager.Instance.Player;
        player.OnStaminaChanged.AddListener(SetFillValue);
    }

    public void SetFillValue(float value)
    {
        // normalize health value and set the fill amount
        fillImage.fillAmount = Mathf.InverseLerp(0, player.MaxStamina, value);
    }
}

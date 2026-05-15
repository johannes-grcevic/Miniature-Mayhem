using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponBobbing : MonoBehaviour
{
    [SerializeField]
    private InputActionReference moveAction;

    [SerializeField]
    private FirstPersonController playerController;

    [SerializeField, Header("Bobbing")]
    private float speed = 5f;

    [SerializeField]
    private float intensity = 0.005f;

    [SerializeField, Header("Smoothing")]
    private float smoothness = 5f;

    private float bobTimer;
    private Vector3 originalPosition;

    private void Start()
    {
        // Record the starting position so we can bob relative to it
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        ApplyBobbing();
    }

    public void ApplyBobbing()
    {
        Vector2 playerMovement = moveAction.action.ReadValue<Vector2>();

        Vector3 horizontalVelocity = new(playerController.CurrentVelocity.x, 0, playerController.CurrentVelocity.z);
        float currentSpeed = horizontalVelocity.magnitude;

        float moveSpeed = playerMovement.sqrMagnitude;
        float bobOffset;

        if (moveSpeed > 0.1f && playerController.Grounded)
        {
            bobTimer += Time.deltaTime * currentSpeed * speed;
            bobOffset = Mathf.Sin(bobTimer) * intensity;
        }
        else
        {
            bobTimer = 0f;
            bobOffset = Mathf.Lerp(bobTimer, 0f, Time.deltaTime * smoothness);
        }

        transform.localPosition = new Vector3(
            originalPosition.x,
            originalPosition.y + bobOffset,
            originalPosition.z
        );
    }
}

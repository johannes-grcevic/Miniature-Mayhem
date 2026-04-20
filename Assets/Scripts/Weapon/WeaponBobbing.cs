using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponBobbing : MonoBehaviour
{
    [SerializeField]
    private InputActionReference moveAction;

    [SerializeField]
    private FirstPersonController playerController;

    [SerializeField]
    private float speed = 5f;

    [SerializeField]
    private float amount = 0.005f;

    [SerializeField]
    private float smoothness = 5f;

    private float timer;
    private CharacterController characterController;

    private Vector3 defaultPosition; // Store the original position

    private void Awake()
    {
        characterController = playerController.GetComponent<CharacterController>();
    }

    void Start()
    {
        // Record the starting position so we can bob relative to it
        defaultPosition = transform.localPosition;
    }

    void Update()
    {
        ApplyBobbing();
    }

    public void ApplyBobbing()
    {
        Vector2 movement = moveAction.action.ReadValue<Vector2>();

        Vector3 horizontalVelocity = new(characterController.velocity.x, 0, characterController.velocity.z);
        float currentSpeed = horizontalVelocity.magnitude;

        float moveSpeed = movement.sqrMagnitude;
        float bobOffset;

        if (moveSpeed > 0.1f && playerController.Grounded)
        {
            timer += Time.deltaTime * currentSpeed * speed;
            bobOffset = Mathf.Sin(timer) * amount;
        }
        else
        {
            timer = 0f;
            bobOffset = Mathf.Lerp(timer, 0f, Time.deltaTime * smoothness);
        }

        transform.localPosition = new Vector3(
            defaultPosition.x,
            defaultPosition.y + bobOffset,
            defaultPosition.z
        );
    }
}

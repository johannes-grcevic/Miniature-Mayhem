using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway")]
    public float intensity = 0.02f;
    public float maxRotation = 5f;

    [Header("Smoothing")]
    public float smoothness = 12f;

    private Quaternion targetRotation;

    private void LateUpdate()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // Mouse movement
        float mouseX = mouseDelta.x * intensity;
        float mouseY = mouseDelta.y * intensity;

        // Clamp sway
        mouseX = Mathf.Clamp(mouseX, -maxRotation, maxRotation);
        mouseY = Mathf.Clamp(mouseY, -maxRotation, maxRotation);

        // Target rotation
        targetRotation = Quaternion.Euler(-mouseY, mouseX, 0f);

        // Frame-rate independent smoothing
        float t = 1f - Mathf.Exp(-smoothness * Time.deltaTime);

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, t);
    }
}
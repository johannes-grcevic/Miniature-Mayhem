using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSway : MonoBehaviour
{
    public float smothness = 8f;
    public float multiplier = 0.1f;

    void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * multiplier;

        // calculate target rotation (Sway)
        Quaternion rotationX = Quaternion.AngleAxis(-mouseDelta.y, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseDelta.x, Vector3.up);
        Quaternion targetRotation = rotationX * rotationY;

        // apply smooth transition
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smothness * Time.deltaTime);
    }
}

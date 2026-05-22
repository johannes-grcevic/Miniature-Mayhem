using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LookAtTarget : MonoBehaviour
{
    public Vector3 LookTargetPosition => lookTargetPosition;
    
    [SerializeField] 
    private Transform lookTarget;

    [SerializeField] 
    private float rotationSpeed = 5f;

    [Header("Constraints")]
    [SerializeField] 
    private bool lockXAxis = false;

    [SerializeField] 
    private bool lockZAxis = false;

    private Vector3 lookTargetPosition;

    private void Update()
    {
        if (!lookTarget) return;

        RotateTowardsTarget();
    }

    public void SetLookTarget(Vector3 target)
    {
        lookTargetPosition = target;
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = lookTargetPosition - transform.position;

        if (lockXAxis) direction.y = 0f;
        if (lockZAxis) direction.z = 0f;

        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
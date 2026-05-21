using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LookAtTarget : MonoBehaviour
{
    [SerializeField] 
    private Transform target;

    [SerializeField] 
    private float rotationSpeed = 5f;

    [Header("Constraints")]
    [SerializeField] 
    private bool lockXAxis = false;

    [SerializeField] 
    private bool lockZAxis = false;

    private void Awake()
    {
        if (!target)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player)
            {
                target = player.transform;
            }
        }
    }

    private void Update()
    {
        if (!target) return;

        RotateTowardsTarget();
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = target.position - transform.position;

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
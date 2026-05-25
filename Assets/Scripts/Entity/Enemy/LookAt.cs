using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LookAt : MonoBehaviour
{
    public Transform Target { get; set; }

    [SerializeField] 
    private float rotationSpeed = 5f;

    private void Start()
    {
        Target = GameManager.Instance.PlayerTransform;
    }

    private void Update()
    {
        if (!Target) return;

        RotateTowardsTarget();
    }

    private void RotateTowardsTarget()
    {
        Quaternion targetRotation = Quaternion.LookRotation(Target.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
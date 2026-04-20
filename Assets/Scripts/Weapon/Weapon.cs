using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    private static readonly int IsAimingHash = Animator.StringToHash("isAiming");

    [SerializeField] 
    private InputActionReference fireAction;

    [SerializeField]
    private InputActionReference aimAction;

    [SerializeField]
    private ProjectileMove projectile;

    [SerializeField]
    private Transform projectileJoint;

    [SerializeField]
    private GameObject crosshair;

    [SerializeField]
    private int damage = 10;

    [SerializeField]
    private float zoomSpeed = 2f;

    private bool isAiming = false;

    private void OnEnable()
    {
        fireAction.action.performed += OnFire;
        aimAction.action.performed += OnAim;
    }

    private void OnDisable()
    {
        fireAction.action.performed -= OnFire;
        aimAction.action.performed -= OnAim;
    }

    protected void OnFire(InputAction.CallbackContext context)
    {
        Instantiate(projectile, projectileJoint.position, projectileJoint.rotation).
            OnCollision.AddListener(OnProjectileCollision);
    }

    protected void OnAim(InputAction.CallbackContext context)
    {
        isAiming = !isAiming;
        
        if (crosshair.TryGetComponent(out Animator animator))
        {
            animator.SetBool(IsAimingHash, isAiming);
            animator.speed = zoomSpeed;
        }
    }

    protected void OnProjectileCollision(GameObject other)
    {
        if (other.TryGetComponent(out EntityEnemy entity))
        {
            DoDamage(entity, damage);
        }
    }

    public void DoDamage(EntityEnemy entity, int damage)
    {
        if (entity.IsDead()) return;
        
        entity.TakeDamage(damage);
        PlayHitAnimation(entity);
    }

    public void PlayHitAnimation(EntityEnemy entity)
    {
        Transform playerTransform = GameManager.Instance.GetPlayerTransform();
        Vector3 directionToTarget = entity.transform.position - playerTransform.position;
        float dotProduct = Vector3.Dot(playerTransform.forward, directionToTarget);

        entity.PlayAnimation(dotProduct > 0 ? EntityEnemy.HitFrontStateName : EntityEnemy.HitBackStateName, 0);
    }

    public bool IsAiming()
    {
        return isAiming;
    }
}

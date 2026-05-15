using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    private static readonly int IsAimingHash = Animator.StringToHash("isAiming");

    [SerializeField, Header("Input")] 
    private InputActionReference fireAction;

    [SerializeField]
    private InputActionReference aimAction;

    [SerializeField, Header("Weapon")]
    private ProjectileMove projectile;

    [SerializeField]
    private Transform projectileJoint;

    [SerializeField]
    private int damage = 10;

    [SerializeField, Header("Sights")]
    private GameObject crosshair;

    [SerializeField]
    private float zoomSpeed = 2f;

    [SerializeField, Header("Audio")]
    private AudioClip fireSound;

    [SerializeField, Range(0f, 1f)]
    private float volume = 1f;

    private AudioSource weaponSource;
    private bool isAiming = false;

    private void Awake()
    {
        weaponSource = GetComponent<AudioSource>();
        weaponSource.playOnAwake = false;
    }

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
        // spawn projectile
        Instantiate(projectile, projectileJoint.position, projectileJoint.rotation).
            OnCollision.AddListener(OnProjectileCollision);

        // play fire sound
        weaponSource.PlayOneShot(fireSound, volume);
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
        if (other.TryGetComponent(out Entity entity))
        {
            DoDamage(entity, damage);
        }
    }

    public void DoDamage(Entity entity, int damage)
    {
        if (entity.IsDead()) return;
        
        entity.TakeDamage(damage);
        PlayHitAnimation(entity as EntityEnemy);
    }

    public void PlayHitAnimation(EntityEnemy entity)
    {
        Transform playerTransform = GameManager.Instance.Player.transform;
        Vector3 directionToTarget = entity.transform.position - playerTransform.position;
        float dotProduct = Vector3.Dot(playerTransform.forward, directionToTarget);

        entity.PlayAnimation(dotProduct > 0 ? EntityEnemy.HIT_FRONT_STATE_TAG : EntityEnemy.HIT_BACK_STATE_TAG, 0);
    }

    public bool IsAiming()
    {
        return isAiming;
    }
}

using ConditionalField;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    public static readonly int IsAimingHash = Animator.StringToHash("IsAiming");

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

    [SerializeField, ConditionalField(nameof(crosshair))]
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
        // weapon does not need to have a crosshair
        if (crosshair == null) return;
        
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
        
        entity.TakeDamage(damage, DamageType.Weapon);
        PlayHitAnimation(entity as EntityEnemy);
    }

    public void PlayHitAnimation(EntityEnemy entity)
    {
        Transform playerTransform = GameManager.Instance.Player.transform;
        Vector3 directionToTarget = entity.transform.position - playerTransform.position;

        // 1 for front facing, 0 for back facing
        float directionFacingTarget = Vector3.Dot(playerTransform.forward, directionToTarget);

        entity.PlayAnimation(directionFacingTarget > 0 ? EntityEnemy.HIT_FRONT_STATE_TAG : EntityEnemy.HIT_BACK_STATE_TAG, 0);
    }

    public bool IsAiming()
    {
        return isAiming;
    }
}

using ConditionalField;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    public static readonly int IsAimingHash = Animator.StringToHash("IsAiming");
    public bool IsAiming => isAiming;

    [SerializeField, Header("Input")] 
    private InputActionReference fireAction;

    [SerializeField]
    private InputActionReference aimAction;

    [SerializeField, Header("Weapon")]
    private int damage = 10;

    [SerializeField, Tooltip("How many projectiles the weapon fires per second.")]
    private float fireRate = 5f;

    [SerializeField, Header("Projectile")]
    private Projectile projectile;

    [SerializeField]
    private Transform projectileJoint;

    [SerializeField, Header("Sights")]
    private GameObject crosshair;

    [SerializeField]
    private float crosshairZoomSpeed = 3f;

    [Header("Camera")]
    [SerializeField]
    private float normalFOV = 40f;

    [SerializeField]
    private float zoomFOV = 30f;

    [SerializeField]
    private float zoomSpeed = 3f;

    [SerializeField, Header("Audio")]
    private AudioClip fireSound;

    [SerializeField, Range(0f, 1f)]
    private float volumeScale = 1f;

    private AudioSource weaponSource;
    private bool isAiming = false;
    private float nextTimeToFire = 0f;

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

    private void Update()
    {
        if (CinemachineCore.GetVirtualCamera(0) is CinemachineCamera virtualCam)
        {
            virtualCam.Lens.FieldOfView = Mathf.Lerp(virtualCam.Lens.FieldOfView, isAiming ? zoomFOV : normalFOV, Time.deltaTime * zoomSpeed);
        }
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        // stop firing when the game is paused
        if (Time.timeScale <= 0f) return;

        // only fire if the cooldown timer has finished
        if (Time.time >= nextTimeToFire)
        {
            // set the time for the next allowed shot
            nextTimeToFire = Time.time + (1f / fireRate * Time.deltaTime);

            ExecuteFire();
        }
    }

    private void OnAim(InputAction.CallbackContext context)
    {
        // weapon does not need a crosshair
        if (crosshair == null) return;
        
        isAiming = !isAiming;

        // animate the crosshair size on screen
        if (crosshair.TryGetComponent(out Animator animator))
        {
            animator.SetBool(IsAimingHash, isAiming);
            animator.speed = crosshairZoomSpeed;
        }
    }

    public void DoDamage(Entity target, int amount)
    {
        if (target.IsDead) return;

        target.TakeDamage(amount, DamageType.Weapon);

        if (target is EntityEnemy enemy)
        {
            enemy.PlayHitAnimation(GameManager.Instance.Player.transform);
        }
    }

    private void OnProjectileCollision(GameObject other)
    {
        if (other.TryGetComponent(out Entity entity))
        {
            DoDamage(entity, damage);
        }
    }

    private void ExecuteFire()
    {
        Instantiate(projectile, projectileJoint.position, projectileJoint.rotation)
            .OnCollision += OnProjectileCollision;

        weaponSource.PlayOneShot(fireSound, volumeScale);
    }
}

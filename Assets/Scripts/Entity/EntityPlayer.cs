using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(FirstPersonController))]
public class EntityPlayer : Entity
{
    public UnityEvent<float> OnStaminaChanged { get; private set; } = new();
    
    [SerializeField]
    private bool godMode = false;

    [SerializeField, Header("Stamina")]
    private float maxStamina = 100f;

    [SerializeField]
    private float drainRate = 10f;

    [SerializeField]
    private float jumpCost = 15f;

    [SerializeField, Space(10)]
    private float regenRate = 5f;

    [SerializeField, Tooltip("Movement speed reduction when out of stamina")]
    private float moveReduceRate = 0.5f;

    [SerializeField, Tooltip("Delay before recovery starts after running out of stamina")]
    private float exhaustionDelay = 1.5f;

    private float currentStamina;
    private float currentRegenTimer;
    private bool isExhausted = false;

    [SerializeField, Header("Audio")]
    private AudioClip painClip;

    [SerializeField, Range(0f, 1f)]
    private float volume;

    private FirstPersonController playerController;
    private AudioSource playerSource;

    private float normalMoveSpeed;
    private float normalSprintSpeed;

    protected override void Awake()
    {
        playerSource = GetComponent<AudioSource>();
        playerController = GetComponent<FirstPersonController>();

        base.Awake();
    }

    private void Start()
    {
        normalMoveSpeed = playerController.MoveSpeed;
        normalSprintSpeed = playerController.SprintSpeed;

        currentStamina = maxStamina;
        currentRegenTimer = 0f;
    }

    private void Update()
    {
        if (!isExhausted && currentStamina > 0f)
        {
            if (playerController.IsSprinting && playerController.InputController.Move.y >= 1.0f)
            {
                DrainStamina(drainRate);
            }
        }

        RegenerateStamina(regenRate);
    }

    public void OnJump()
    {
        DecreaseStamina(jumpCost);
    }

    public override void TakeDamage(int amount)
    {
        if (godMode)
        {
            Debug.LogWarning("Can't take damage while in god mode!");
            return;
        }

        playerSource.PlayOneShot(painClip, volume);

        base.TakeDamage(amount);
    }

    public void DrainStamina(float rate)
    {
        DecreaseStamina(rate * Time.deltaTime);

        if (currentStamina <= 0)
        {
            isExhausted = true;
            ApplyReducedSpeed();
        }
    }

    public void RegenerateStamina(float rate)
    {
        if (isExhausted)
        {
            currentRegenTimer += Time.deltaTime;

            // wait for the timer to expire
            if (currentRegenTimer >= exhaustionDelay)
            {
                isExhausted = false; // Recovery can now begin
                currentRegenTimer = 0f; // Reset regen timer
            }
        }
        else
        {
            // Recover continuously up to max
            IncreaseStamina(rate * Time.deltaTime);
            ApplyNormalSpeed();
        }
    }

    public void IncreaseStamina(float value)
    {
        if (currentStamina + value <= maxStamina)
        {
            currentStamina += value;
        }
        else
        {
            currentStamina = maxStamina;
        }

        OnStaminaChanged.Invoke(currentStamina);
    }
    
    public void DecreaseStamina(float value)
    {
        // prevent stamina from going below zero
        if (currentStamina - value >= 0f)
        {
            currentStamina -= value;
        }
        else
        {
            currentStamina = 0f;
        }      

        OnStaminaChanged.Invoke(currentStamina);
    }

    public void SetMaxStamina(float value) => maxStamina = Mathf.Clamp(value, 0f, maxStamina);

    public float GetMaxStamina() => maxStamina;

    public float GetCurrentStamina() => currentStamina;

    private void ApplyNormalSpeed()
    {
        playerController.MoveSpeed = normalMoveSpeed;
        playerController.SprintSpeed = normalSprintSpeed;
    }

    private void ApplyReducedSpeed()
    {
        playerController.MoveSpeed *= moveReduceRate;
        playerController.SprintSpeed *= moveReduceRate;
    }
}

using ConditionalField;
using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(FirstPersonController))]
public class EntityPlayer : Entity
{
    public UnityEvent<float> OnStaminaChanged;

    public float MaxStamina => maxStamina;
    public float CurrentStamina => currentStamina;
    public bool IsStaminaExhausted => isStaminaExhausted;
    public FirstPersonController Controller => controller;

    [SerializeField, Header("Stamina")]
    private float maxStamina = 100f;

    [SerializeField, Tooltip("The speed that stamina drains when moving forward and sprinting")]
    private float drainRate = 10f;

    [SerializeField, Tooltip("The speed that stamina recovers when not sprinting")]
    private float regenRate = 5f;

    [SerializeField, Tooltip("The amount of stamina used to jump")]
    private float jumpCost = 15f;

    [Header("Stamina Penalty")]
    [SerializeField, Range(0f, 1f), Tooltip("Movement speed percentage reduction when out of stamina")]
    private float speedReducePerc = 0.5f;

    [SerializeField, Tooltip("Delay before recovery starts after running out of stamina")]
    private float recoveryDelay = 1f;

    [SerializeField, Header("Audio")]
    private AudioClip painClip;

    [SerializeField]
    private AudioClip hitClip;

    [SerializeField, Range(0f, 1f)]
    private float volumeScale;

    private float currentStamina;
    private float staminaRegenTimer;
    private bool isStaminaExhausted = false;

    private FirstPersonController controller;

    protected override void Awake()
    {
        controller = GetComponent<FirstPersonController>();

        base.Awake();
    }

    protected override void Start()
    {
        currentStamina = maxStamina;
        staminaRegenTimer = 0f;
        
        base.Start();
    }

    private void Update()
    {   
        if (IsDead) return;
        
        // only drain stamina if we have some to use
        if (currentStamina > 0f && !isStaminaExhausted)
        {
            // drain when sprinting and moving forward only
            if (controller.IsSprinting && controller.IsMovingForward)
            {
                DrainStamina(drainRate);
            }
        }

        // Only regen when not sprinting
        if (!controller.IsSprinting)
        {
            RegenStamina(regenRate);
        }
    }

    public void OnJump()
    {
        // Only count the jump if the player is grounded
        if (!controller.IsGrounded) return;

        // prevent stamina from dropping low too quickly
        if (currentStamina > jumpCost)
        {
            DecreaseStaminaImmediate(jumpCost);
        }
    }

    public override void TakeDamage(int value, DamageType type)
    {
        base.TakeDamage(value, type);

        PlayAudioClip(painClip, volumeScale);

        if (type == DamageType.Entity)
        {
            PlayAudioClip(hitClip, volumeScale);
        }
    }

    public void DrainStamina(float rate)
    {
        DecreaseStaminaImmediate(rate * Time.deltaTime);

        if (Mathf.Approximately(currentStamina, 0f))
        {
            isStaminaExhausted = true;

            // reduce movement speed
            controller.ApplyReducedSpeed(speedReducePerc);
        }
    }

    public void RegenStamina(float rate)
    {
        if (isStaminaExhausted)
        {
            staminaRegenTimer += Time.deltaTime;

            // wait for the timer to expire
            if (staminaRegenTimer >= recoveryDelay)
            {
                isStaminaExhausted = false; // Recovery can now begin
                staminaRegenTimer = 0f; // Reset regen timer
            }
        }
        else
        {
            // Recover continuously up to max
            IncreaseStaminaImmediate(rate * Time.deltaTime);

            // apply normal movement speed
            controller.ApplyStartSpeed();
        }
    }

    public void IncreaseStaminaImmediate(float value)
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
    
    public void DecreaseStaminaImmediate(float value)
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

    public void SetMaxStamina(float value)
    {
        maxStamina = Mathf.Clamp(value, 0f, maxStamina);
    }
}

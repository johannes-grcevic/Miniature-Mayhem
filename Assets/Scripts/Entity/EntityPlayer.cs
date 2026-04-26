using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EntityPlayer : Entity
{
    [SerializeField]
    private bool godMode = false;

    [SerializeField, Header("Audio")]
    private AudioClip takeDamageSound;

    [SerializeField, Range(0f, 1f)]
    private float volume;

    private AudioSource playerSource;

    public override void Awake()
    {
        playerSource = GetComponent<AudioSource>();
        playerSource.playOnAwake = false;

        base.Awake();
    }

    public override void TakeDamage(int amount)
    {
        if (godMode)
        {
            Debug.LogWarning("Can't take damage while in god mode!");
            return;
        }

        playerSource.PlayOneShot(takeDamageSound, volume);

        base.TakeDamage(amount);
    }  
}

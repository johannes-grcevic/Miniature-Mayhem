using UnityEngine;

public class EntityPlayer : Entity
{
    [SerializeField]
    private bool godMode = false;

    public override void TakeDamage(int amount)
    {
        if (godMode) return;
        
        base.TakeDamage(amount);
    }  
}

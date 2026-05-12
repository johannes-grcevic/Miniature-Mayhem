using UnityEngine;

public class ParticleCollisionRelay : MonoBehaviour
{
    [SerializeField]
    private ProjectileMove parent;

    void OnParticleCollision(GameObject other)
    {
        parent.HandleParticleCollision(other);
    }

    public ProjectileMove GetParent()
    {
        return parent;
    }

    public void SetParent(ProjectileMove value)
    {
        parent = value;
    }
}
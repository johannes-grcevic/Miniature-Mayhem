using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Condition Chase Player", menuName = "AI/Conditions/Chase Player")]
public class ConditionChasePlayer : AgentCondition
{
    [SerializeField]
    private float maxDistance = 100f;

    [SerializeField]
    private LayerMask areaMask = NavMesh.AllAreas;

    public override bool IsSatisfied(NavMeshAgent agent)
    {
        return NavMesh.SamplePosition(GameManager.Instance.Player.transform.position, out _, maxDistance, areaMask);
    }
}

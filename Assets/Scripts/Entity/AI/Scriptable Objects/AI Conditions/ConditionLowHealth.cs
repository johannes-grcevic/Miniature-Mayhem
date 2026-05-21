using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Condition Low Health", menuName = "AI/Conditions/Low Health")]
public class ConditionLowHealth : AgentCondition
{
    [Range(0f, 1f)]
    [SerializeField] private float healthThresholdPercentage = 0.3f;

    public override bool IsSatisfied(NavMeshAgent agent)
    {
        if (agent.TryGetComponent(out Entity entity))
        {
            return entity.CurrentHealthPerc <= healthThresholdPercentage;
        }

        return false;
    }
}

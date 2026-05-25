using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Condition Low Health", menuName = "AI/Conditions/Low Health")]
public class ConditionLowHealth : AgentCondition
{
    [Range(0f, 100f)]
    [SerializeField] private float healthThresholdPercentage = 30f;

    public override bool IsSatisfied(NavMeshAgent agent)
    {
        return agent.TryGetComponent(out Entity entity) && entity.CurrentHealthPerc <= healthThresholdPercentage;
    }
}

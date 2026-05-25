using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Condition Chase Player", menuName = "AI/Conditions/Chase Player")]
public class ConditionChasePlayer : AgentCondition
{
    public override bool IsSatisfied(NavMeshAgent agent)
    {
        return agent.TryGetComponent(out Entity self) && !self.IsHealing && !self.IsDead && !GameManager.Instance.Player.IsDead;
    }
}

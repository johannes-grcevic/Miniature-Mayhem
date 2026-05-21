using UnityEngine;
using UnityEngine.AI;

public abstract class AgentCondition : ScriptableObject
{
    public abstract bool IsSatisfied(NavMeshAgent agent);
}

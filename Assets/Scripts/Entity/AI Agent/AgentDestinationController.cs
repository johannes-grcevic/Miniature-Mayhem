using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Entity))]
public class AgentDestinationController : MonoBehaviour
{
    public enum DestinationType { None,Custom,Player,SpawnPoint }

    public DestinationType CurrentDestination => currentDestination;
    
    [Serializable]
    public struct ConditionDestinationPair
    {
        public AgentCondition condition;

        [Tooltip("Where the agent should go if the condition is met.")]
        public DestinationType destinationType;

        [Tooltip("Set a custom destination for the agent. Destination Type must be set to Custom to use this.")]
        public Transform customDestination;
    }

    [SerializeField, Header("Agent Conditions")] 
    private ConditionDestinationPair[] conditionalBehaviors;

    private DestinationType currentDestination;

    private NavMeshAgent agent;
    private Entity entity;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        entity = GetComponent<Entity>();
    }

    private void Update()
    {
        if (!agent.enabled || agent.speed == 0) return;

        EvaluateConditionalDestinations();
    }

    private void EvaluateConditionalDestinations()
    {
        foreach (var behavior in conditionalBehaviors)
        {
            if (!behavior.condition || behavior.destinationType == DestinationType.None) continue;

            if (behavior.condition.IsSatisfied(agent))
            {
                // set the current destination type
                currentDestination = behavior.destinationType;

                // Send the agent to its destination
                switch (currentDestination)
                {
                    case DestinationType.Custom:
                        agent.SetDestination(behavior.customDestination.position);
                        break;
                    case DestinationType.Player:
                        agent.SetDestination(GameManager.Instance.PlayerTransform.position);
                        break;
                    case DestinationType.SpawnPoint:
                        agent.SetDestination(entity.SpawnPoint);
                        break;
                    case DestinationType.None:
                        Debug.LogError($"[{name}] Has no destination.");
                        break;
                    default:
                        break;
                }

                // If the first condition is satisfied exit the loop
                // Prioritises conditions in the order they appear in the list
                break;
            }
        }
    }
}

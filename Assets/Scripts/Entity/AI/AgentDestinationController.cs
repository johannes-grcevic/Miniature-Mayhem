using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentDestinationController : MonoBehaviour
{
    public enum DestinationType { Custom,Player,SpawnPoint }
    public DestinationType CurrentDestination => currentDestination;
    
    [Serializable]
    public struct ConditionDestinationPair
    {
        public AgentCondition condition;

        [Tooltip("Where the agent should go if the condition is met.")]
        public DestinationType destinationType;

        [Tooltip("Set a custom destination for the agent.")]
        public Transform destination;
    }

    [SerializeField, Header("Agent Conditions")] 
    private ConditionDestinationPair[] conditionalBehaviors;

    private NavMeshAgent agent;
    private DestinationType currentDestination;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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
            if (behavior.condition == null) continue;

            // Check if the individual asset condition is met
            if (!behavior.condition.IsSatisfied(agent)) continue;

            currentDestination = behavior.destinationType;

            // Send the agent to the closest location
            switch (currentDestination)
            {
                case DestinationType.Custom:
                    agent.SetDestination(behavior.destination.position);
                    break;
                case DestinationType.Player:
                    agent.SetDestination(GameManager.Instance.Player.transform.position);
                    break;
                case DestinationType.SpawnPoint:
                    if (agent.TryGetComponent(out Entity entity))
                    {
                        agent.SetDestination(entity.SpawnPoint);
                    }
                    break;
                default:
                    //Debug.LogWarning($"[{agent}] Has no valid destination.");
                    break;
            }

            //Debug.Log($"[{agent}] Moving to destination: {behavior.destinationType}.");

            // Exit if the first condition is met to give it priority over the ones below it
            break;
        }
    }
}

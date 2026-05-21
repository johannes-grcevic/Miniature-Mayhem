using ConditionalField;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AgentDestinationController : MonoBehaviour
{
    public enum DestinationType { None,Player,Other }
    
    [Serializable]
    public struct ConditionDestinationPair
    {
        public AgentCondition condition;

        public DestinationType destinationType;

        [Tooltip("Where the agent should go if the condition is met.")]
        public Transform[] destinations;

        [HideInInspector]
        public DistanceComparer<Transform> distanceComparer;
    }

    [SerializeField, Space(10)] 
    private List<ConditionDestinationPair> conditionalBehaviors;

    private NavMeshAgent agent;

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
            if (behavior.condition == null || behavior.destinations == null || behavior.destinationType == DestinationType.None) continue;

            // Check if the individual asset condition is met
            if (behavior.condition.IsSatisfied(agent))
            {      
                // Sorts the list from closest to furthest
                if (behavior.destinations.Length > 1)
                {
                    Array.Sort(behavior.destinations, behavior.distanceComparer);
                }

                // Send the agent to the closest location
                agent.SetDestination(behavior.destinations.Length > 0 ? behavior.destinations[0].transform.position : GameManager.Instance.Player.transform.position);

                Debug.Log($"[{gameObject.name}] Moving to destination due to: {behavior.condition.name}.");

                // Prioritizes the top items in the Inspector list over the bottom items
                break;
            }
        }
    }
}

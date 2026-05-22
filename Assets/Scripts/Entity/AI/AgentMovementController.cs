using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class AgentMovementController : MonoBehaviour
{  
    private NavMeshAgent agent;
    private Animator animator;
    private Vector3 currentVelocity;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!agent.enabled || agent.speed <= 0f) return;

        // get the current velocity of the agent
        currentVelocity = agent.velocity;

        // Smooth movement to fix jittering
        transform.position = Vector3.SmoothDamp(transform.position, agent.nextPosition, ref currentVelocity, Time.deltaTime, agent.speed);
    }

    private void OnAnimatorMove()
    {
        if (!animator.applyRootMotion || !agent.enabled || agent.speed <= 0f) return;
        
        Vector3 position = animator.rootPosition;

        // Apply root motion to agent
        position.y = agent.nextPosition.y;
        transform.position = position;
        agent.nextPosition = transform.position;
    }
}

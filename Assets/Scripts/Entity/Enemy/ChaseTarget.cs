using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class ChaseTarget : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    private NavMeshAgent agent;
    private Animator animator;

    private Vector3 currentVelocity = Vector3.zero;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (!target)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        if (!agent.enabled || agent.speed == 0) return;
        
        // set the current agent velocity
        currentVelocity = agent.velocity;

        // set the agent destination to the player
        agent.SetDestination(target.position);

        // Smooth movement to fix jittering
        transform.position = Vector3.SmoothDamp(transform.position, agent.nextPosition, ref currentVelocity, Time.deltaTime, agent.speed);
    }

    private void OnAnimatorMove()
    {
        if (!animator.applyRootMotion || !agent.enabled || agent.speed == 0) return;
        
        Vector3 position = animator.rootPosition;

        // Apply root motion to agent
        position.y = agent.nextPosition.y;
        transform.position = position;
        agent.nextPosition = transform.position;
    }
}

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
    private bool isRootMotion = false;

    private Vector3 currentVelocity = Vector3.zero;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.updatePosition = false;

        isRootMotion = animator.applyRootMotion;

        if (isRootMotion)
        {
            agent.updatePosition = false;
            agent.updateRotation = true;
        }

        if (!target)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (agent.speed == 0) return;
        
        // set the current agent velocity
        currentVelocity = agent.velocity;

        // set the agent destination to the player
        agent.SetDestination(target.position);

        // Smooth movement to fix jittering
        transform.position = Vector3.SmoothDamp(transform.position, agent.nextPosition, ref currentVelocity, Time.deltaTime, agent.speed);
    }

    void OnAnimatorMove()
    {
        if (!isRootMotion) return;
        
        Vector3 position = animator.rootPosition;

        // Apply root motion to agent
        position.y = agent.nextPosition.y;
        transform.position = position;
        agent.nextPosition = transform.position;
    }
}

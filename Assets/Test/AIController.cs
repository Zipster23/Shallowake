using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    private GameObject destination;
    private NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
        destination = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        // Safety check: Is the agent actually on a NavMesh at the start?
        if (!agent.isOnNavMesh)
        {
            Debug.LogError($"{gameObject.name} is not on a NavMesh! Check your Bake or placement.");
        }
    }

    private void Update()
{
    if (agent.isActiveAndEnabled && agent.isOnNavMesh && destination != null)
    {
        agent.SetDestination(destination.transform.position);

        // 1. Get the movement vector in world space
        Vector3 worldVelocity = agent.velocity;

        // 2. Convert it to "Local" space (Relative to the Dummy's front/sides)
        Vector3 localVelocity = transform.InverseTransformDirection(worldVelocity);

        // 3. Map these to your specific Blend Tree parameters
        // localVelocity.z is Forward/Backward
        // localVelocity.x is Left/Right
        anim.SetFloat("Vertical", localVelocity.z);
        anim.SetFloat("Horizontal", localVelocity.x);
    }
}
}
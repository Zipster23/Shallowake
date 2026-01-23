using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{

    // Variables to store references to components and the target destination
    private GameObject destination;
    private NavMeshAgent agent;
    private Animator anim;

    public float attackRange = 3.0f;        // how close the dummy needs to be to hit you
    private float attackCooldown = 1.5f;    // time between attacks
    private float nextAttackTime = 0;
    private int comboCount = 0;
    private Renderer dummyRenderer;              // Mesh component of the dummy to add an "attack indicator"

    private float lastHitTime = 0;
    private float hitCooldown = 0.5f;

    void Start()
    {
        // Find the player in the scene
        destination = GameObject.FindGameObjectWithTag("Player");

        // Variables to store the components attached to the AI
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        dummyRenderer = GetComponentInChildren<Renderer>();

        // Debug Check - checks if dummy is standing on the NavMesh
        if (!agent.isOnNavMesh)
        {
            Debug.LogError($"{gameObject.name} is not on a NavMesh! Check your Bake or placement.");
        }
    }

    private void Update()
    {       
        // Only run logic if the agent is ready and the player is in the scene
        if (agent.isActiveAndEnabled && agent.isOnNavMesh && destination != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, destination.transform.position);

            // Calculates a path to the player's current position
            agent.SetDestination(destination.transform.position);

            if(distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
            {
                comboCount++;
                StartCoroutine(AttackFlash());

                if(comboCount % 3 == 0)
                {
                    nextAttackTime = Time.time + 2.5f;
                } 
                else
                {
                    nextAttackTime = Time.time + 0.6f;
                }
            }
            // Stores the movement vector (how fast the dummy is moving) in a variable
            Vector3 worldVelocity = agent.velocity;

            // Calculates if this movement is happening in the direction that the dummy is facing
            Vector3 localVelocity = transform.InverseTransformDirection(worldVelocity);

            // Map these to the specific Blend Tree parameters
            // localVelocity.z is Forward/Backward
            // localVelocity.x is Left/Right
            anim.SetFloat("Vertical", localVelocity.z);
            anim.SetFloat("Horizontal", localVelocity.x);
        }   
    }

    private void OnTriggerEnter(Collider other)
    {


        if(other.CompareTag("Weapon") && Time.time >= lastHitTime + hitCooldown)
        {
            Debug.Log("Hit! Player slashed the dummy");
            lastHitTime = Time.time;
        }
    }

    System.Collections.IEnumerator AttackFlash()
    {
        dummyRenderer.material.color = Color.yellow;
        yield return new WaitForSeconds(0.3f);
        dummyRenderer.material.color = Color.red;
    }


}

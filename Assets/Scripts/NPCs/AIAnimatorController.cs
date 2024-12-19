using UnityEngine;
using UnityEngine.AI;

public class AIAnimatorController : MonoBehaviour
{
    public Animator animator; // Reference to the Animator
    private NavMeshAgent agent; // Reference to the NavMeshAgent

    void Start()
    {
        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
        {
            Debug.LogError("Animator is not assigned!");
        }

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent is not assigned!");
        }
    }

    void Update()
    {
        // Calculate the AI's current speed
        float speed = agent.velocity.magnitude;
        Debug.Log(speed);
        // Update the Animator's Speed parameter
        animator.SetFloat("velocity", speed);
    }
}

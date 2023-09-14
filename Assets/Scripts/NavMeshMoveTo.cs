using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Author: Tim Deville (2003506)
///
/// Attached to a NavMesh agent, provides it with a target to move to.
/// </summary>
public class NavMeshMoveTo : MonoBehaviour
{
    [SerializeField] private Transform goal;
    private PositionLogger positionLogger; // Allow the positions to be recorded.

    /// <summary>
    /// Called once. Fetches relevant references and sets the target position.
    /// </summary>
    private void Start()
    {
        positionLogger = GetComponent<PositionLogger>();
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = goal.position;
    }

    /// <summary>
    /// When the NavMesh agent collides with its target (a 'finish' box), the array of positions are saved.
    /// </summary>
    /// <param name="other">The object being collided with</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            print("FINISHED");
            positionLogger.SavePathToJson();
        }
    }
}
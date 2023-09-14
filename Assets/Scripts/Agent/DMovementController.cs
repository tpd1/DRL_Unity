using UnityEngine;

/// <summary>
/// Author: Tim Deville (2003506)
///
/// Handles the physical movement of the agent based on its chosen output.
/// </summary>
public class DMovementController : MonoBehaviour
{
    public const float MoveSpeed = 4f;
    private const float RotationSpeed = 160f;
    private Rigidbody rb;

    /// <summary>
    /// Called Once. Fetches a reference to the agents RigidBody for physics calculations.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Public interface for processing agent movement input.
    /// </summary>
    /// <param name="forwardInput"> forwards / backwards movement selected by the agent.</param>
    /// <param name="rotationInput">the agent's rotational input.</param>
    public void ProcessInput(float forwardInput, float rotationInput)
    {
        ProcessMovement(forwardInput);
        ProcessRotation(rotationInput);
    }


    /// <summary>
    /// Processes the forward / backwards motion input, updating the physics engine with the new values. 
    /// </summary>
    /// <param name="forwardInput">forwards / backwards movement selected by the agent.</param>
    private void ProcessMovement(float forwardInput)
    {
        var targetVelocity = transform.forward * forwardInput * MoveSpeed;
        var velocityChange = targetVelocity - rb.velocity;

        // Cap the move speed to a maximum value.
        if (velocityChange.magnitude > MoveSpeed)
        {
            velocityChange = velocityChange.normalized * MoveSpeed;
        }

        // Apply the force to the agent's rigidbody.
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    /// <summary>
    /// Processes the agent's rotational input, applying the the new values direction to its transform component.
    /// </summary>
    /// <param name="rotationInput">the agent's rotational input.</param>
    private void ProcessRotation(float rotationInput)
    {
        if (rotationInput != 0f)
        {
            float angle = Mathf.Clamp(rotationInput, -1f, 1f) * RotationSpeed;
            transform.Rotate(Vector3.up, Time.fixedDeltaTime * angle);
        }
    }
}
using UnityEngine;

/// <summary>
/// Author: Tim Deville (2003506)
///
/// Handles the animation aspect of the agent's movement. Connects to the Unity animation state machine.
/// </summary>
public class AnimatorController : MonoBehaviour
{
    [SerializeField] private float blendTime = 0.1f;
    private Animator animator;
    private int rotation;
    private int vertical;

    /// <summary>
    /// Called Once. creates hash values for animation string types for efficiency.
    /// </summary>
    private void Awake()
    {
        animator = GetComponent<Animator>();
        vertical = Animator.StringToHash("Vertical");
        rotation = Animator.StringToHash("Turn");
    }

    /// <summary>
    /// Sets the animator states based on input values.
    /// </summary>
    /// <param name="verticalVelocity">forwards / backwards agent input.</param>
    /// <param name="rotationVelocity">agent rotational input.</param>
    public void ProcessAnimations(float verticalVelocity, float rotationVelocity)
    {
        animator.SetFloat(vertical, verticalVelocity, blendTime, Time.deltaTime);
        animator.SetFloat(rotation, rotationVelocity, blendTime, Time.deltaTime);
    }
}
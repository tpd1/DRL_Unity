using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

/// <summary>
/// Author: Tim Deville (2003506)
/// 
/// Variation of the main Agent class, designed to operate solely within the testing environment,
/// within static tests that don't rely on the floor generator script.
/// </summary>
public class TestAgent : Agent
{
    private AnimatorController animatorController;
    private DMovementController movementController;
    private Rigidbody rb;
    private TestManager testManager;
    private bool isInSmoke;
    private RayPerceptionSensorComponent3D raysForward;
    private const float smokeRayLength = 5f;
    private float defaultRayLength;
    private PositionLogger positionLogger;
    private StatsLogger statsLogger;
    
    /// <summary>
    /// Initialises components at the start of an episode.
    /// </summary>
    public override void Initialize()
    {
        positionLogger = GetComponent<PositionLogger>();
        raysForward = GetComponentInChildren<RayPerceptionSensorComponent3D>();
        if (raysForward != null) defaultRayLength = raysForward.RayLength;

        animatorController = GetComponent<AnimatorController>();
        rb = GetComponent<Rigidbody>();
        testManager = GetComponentInParent<TestManager>();
        movementController = GetComponent<DMovementController>();
        statsLogger = GetComponent<StatsLogger>();
    }

    /// <summary>
    /// Called when the agent collides with another object in the scene.
    /// </summary>
    /// <param name="collision">A reference to the object being collided with.</param>
    private void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Wall":
                if (statsLogger != null) statsLogger.PrintStats(wallCollision:true);
                AddReward(LevelParameters.Instance.GetWallPenalty());
                testManager.ResetLevel();
                positionLogger.ResetList();
                EndEpisode();
                break;
            case "Obstacle":
                if (statsLogger != null) statsLogger.ObstacleCollisions++;
                AddReward(LevelParameters.Instance.GetObstaclePenalty());
                break;
        }
    }

    /// <summary>
    /// Called when the agent exits a collider tagged as a trigger. 
    /// </summary>
    /// <param name="other">A reference to the object being collided with.</param>
    private void OnTriggerExit(Collider other)
    {
        switch (other.tag)
        {
            case "Unvisited Door":
                if (statsLogger != null) statsLogger.RoomsVisited++;
                AddReward(LevelParameters.Instance.GetUnvisitedDoorReward());
                break;
            case "Smoke":
                isInSmoke = false;
                if (raysForward != null) raysForward.RayLength = defaultRayLength;
                StopCoroutine(TakeSmokeDamage());
                break;
            case "Finish":
                AddReward(LevelParameters.Instance.GetExitReward());
                if (statsLogger != null) statsLogger.PrintStats();
                positionLogger.SavePathToJson();
                testManager.ResetLevel();
                EndEpisode();
                break;
        }
    }

    /// <summary>
    /// Called when the agent first enters a collider tagged as a trigger. 
    /// </summary>
    /// <param name="other">A reference to the object being collided with.</param>
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Smoke":
                isInSmoke = true;
                StartCoroutine(TakeSmokeDamage());
                break;
        }
    }

    /// <summary>
    /// Simulates the effect of smoke damage on the agent.
    /// </summary>
    /// <returns>Coroutine that applies damage over time.</returns>
    private IEnumerator TakeSmokeDamage()
    {
        while (isInSmoke)
        {
            if (raysForward != null) raysForward.RayLength = smokeRayLength;
            AddReward(LevelParameters.Instance.GetSmokePenalty());
            yield return new WaitForSeconds(LevelParameters.Instance.GetSmokePenaltyDelay());
        }
    }


    /// <summary>
    /// Called at the start of every episode. Resets the agent to the starting position.
    /// </summary>
    public override void OnEpisodeBegin()
    {
        if (statsLogger != null) statsLogger.SyncStart();
        testManager.ResetAllDoorways();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }


    /// <summary>
    /// Controls the agent with human input
    /// </summary>
    /// <param name="actionsOut">The actions parsed from keyboard input.</param>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discActionsOut = actionsOut.DiscreteActions;

        if (Input.GetKey(KeyCode.UpArrow)) discActionsOut[0] = 1;
        if (Input.GetKey(KeyCode.DownArrow)) discActionsOut[0] = 2;
        if (Input.GetKey(KeyCode.LeftArrow)) discActionsOut[1] = 2;
        if (Input.GetKey(KeyCode.RightArrow)) discActionsOut[1] = 1;
    }

    /// <summary>
    /// Collect vector observations from environment
    /// </summary>
    /// <param name="sensor">The vector sensor</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        // Normalised rotation on Y-axis.
        float rotationYNorm = transform.rotation.eulerAngles.y / 360.0f;

        // Normalised speed
        Vector3 velocity = transform.InverseTransformDirection(rb.velocity);
        float speed = velocity.magnitude;
        float speedNorm = speed / DMovementController.MoveSpeed;

        // Send observations to ML-agents
        sensor.AddObservation(rotationYNorm);
        sensor.AddObservation(speedNorm);
        sensor.AddObservation(isInSmoke); // Whether the agent is within the smoke box collider.
    }

    /// <summary>
    /// Processes the selected actions.
    /// actions[0] = Forward movement (-1, 0, 1)
    /// actions[1] = Rotation (-1,0,1)
    /// </summary>
    /// <param name="actions">The actions received</param>
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (StepCount >= MaxStep)
        {
            if (statsLogger != null) statsLogger.PrintStats(maxSteps:true);
            positionLogger.ResetList();
        }
        float forwardAction = actions.DiscreteActions[0] <= 1 ? actions.DiscreteActions[0] : -1;
        float rotationAction = actions.DiscreteActions[1] <= 1 ? actions.DiscreteActions[1] : -1;

        // Small penalty for moving backwards.
        if (forwardAction < 0f) AddReward(-1f / MaxStep);

        // Pass to movement controller to handle rigid body movement.
        movementController.ProcessInput(forwardAction, rotationAction);
        animatorController.ProcessAnimations(forwardAction, rotationAction);
    }
}
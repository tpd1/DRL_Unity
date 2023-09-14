using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Author: Tim Deville (2003506)
///
/// Controls all aspects of the training environment and provides relevant dependencies to the DAgent.
/// </summary>
public class AgentManager : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;

    [Header("Test Mode Only")] [SerializeField]
    private GameObject testLevelPrefab;

    [Header("Training Mode Only")] [SerializeField]
    private FloorGenerator floorGenerator;

    private int collisionCounter;
    private bool agentInitialised;
    private Bounds startRoomBox;

    /// <summary>
    /// Called Once. Instantiates the floor generation builder.
    /// </summary>
    private void Start()
    {
        floorGenerator.InitiateBuild();
    }

    /// <summary>
    /// Returns a random starting location within the confines of the starting room.
    /// </summary>
    /// <returns>Position vector for the new starting position</returns>
    public Vector3 GetRandomStartPosition()
    {
        Bounds tempBox = floorGenerator.GetStartRoomBounds().bounds;
        var halfBoxSize = tempBox.size / 2;
        const float offset = 0.5f;
        float minX = -halfBoxSize.x + offset;
        float maxX = halfBoxSize.x - offset;
        float minZ = -halfBoxSize.z + offset;
        float maxZ = halfBoxSize.z - offset;
        var initialX = Random.Range(minX, maxX);
        var initialZ = Random.Range(minZ, maxZ);
        return new Vector3(initialX, 0, initialZ);
    }


    /// <summary>
    /// Returns a random rotation vector that represents the agent's new starting rotation.
    /// </summary>
    /// <returns>A random rotation vector.</returns>
    public static Quaternion GetRandomRotation()
    {
        var yRotation = Random.Range(-180f, 180f);
        Quaternion rotation = Quaternion.Euler(0, yRotation, 0);
        return rotation;
    }

    /// <summary>
    /// Called when the agent collides with a wall. Resets the agent and regenerates the environment if
    /// a threshold value is passed.
    /// </summary>
    public void ProcessWallCollision()
    {
        collisionCounter++;
        if (collisionCounter > LevelParameters.Instance.GetResetAfterFailed())
        {
            collisionCounter = 0;
            floorGenerator.ResetLayout();
        }
        else
        {
            ResetAllDoorways();
        }
    }

    /// <summary>
    /// public interface allowing the floor to be regenerated
    /// </summary>
    /// <param name="exitReached">Whether the final exit door was reached.</param>
    public void ResetLevel(bool exitReached)
    {
        if (exitReached)
        {
            LevelParameters.Instance.IncrementExitCounter();
            Academy.Instance.StatsRecorder.Add("Exit Count", LevelParameters.Instance.GetExitCounter(),
                StatAggregationMethod.MostRecent);
        }

        collisionCounter = 0;
        floorGenerator.ResetLayout();
    }

    /// <summary>
    /// Instantiates an agent prefab into the environment.
    /// </summary>
    public void SpawnAgent()
    {
        if (!agentInitialised && LevelParameters.Instance.InitialiseAgent)
        {
            Instantiate(agentPrefab, GetRandomStartPosition(), GetRandomRotation(), transform);
            agentInitialised = true;
        }
    }

    /// <summary>
    /// Resets all doorways in the test level to their original state.
    /// </summary>
    public void ResetAllDoorways()
    {
        foreach (Doorway doorway in transform.GetComponentsInChildren<Doorway>())
        {
            doorway.ResetDoor();
        }
    }
    
    /// <summary>
    /// Called by the camera controller in order to centre the camera on the environment.
    /// </summary>
    /// <returns>The centre position of the entire layout.</returns>
    public Vector3 GetLevelCenterPoint()
    {
        // Get list of generated rooms
        List<Room> roomList = floorGenerator.GetRoomList();

        // find min an max points for X & Z in world space.
        float minX = roomList.Min(r => r.RoomObject.transform.position.x);
        float minZ = roomList.Min(r => r.RoomObject.transform.position.z);
        float maxX = roomList.Max(r => r.RoomObject.transform.position.x);
        float maxZ = roomList.Max(r => r.RoomObject.transform.position.z);

        //calculate center point to position camera.
        float centerX = (minX + maxX) / 2.0f;
        float centerZ = (minZ + maxZ) / 2.0f;

        return new Vector3(centerX, 0f, centerZ);
    }
}
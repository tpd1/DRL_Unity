using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

/// <summary>
/// Author: Tim Deville (2003506)
///
/// Controls all aspects of the demostration environment and provides relevant dependencies to the DemoAgent.
/// </summary>
public class DemoManager : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] FloorGenerator floorGenerator;
    [SerializeField] private UnityEvent resetEvent;
    private bool agentInitialised;

    /// <summary>
    /// Called Once. Initialises the procedural generation of the level layout.
    /// </summary>
    private void Start()
    {
        Application.targetFrameRate = 60;
        floorGenerator.InitiateBuild();
    }

    /// <summary>
    /// Returns a random position vector within the starting room.
    /// </summary>
    /// <returns>Random position within the starting room.</returns>
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
    /// Public interface that regenerates the procedurally generated environment.
    /// </summary>
    public void RegenTrainingLevel()
    {
        floorGenerator.ResetLayout();
        resetEvent?.Invoke();
    }


    /// <summary>
    /// Public interface for resetting the environment.
    /// </summary>
    public void ResetLevel()
    {
        ResetAllDoorways();
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
}
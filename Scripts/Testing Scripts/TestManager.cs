using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Author: Tim Deville (2003506)
///
/// Controls all aspects of the test environment and provides relevant dependencies to the TestAgent.
/// </summary>
public class TestManager : MonoBehaviour
{
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private bool randomStartPos;
    [SerializeField] private KeyCode screenShotButton;
    [Header("Test Mode Only")] [SerializeField]
    
    private GameObject testLevelPrefab;
    private bool agentInitialised;
    private Bounds startRoomBox;

    private void Start()
    {
        startRoomBox = testLevelPrefab.transform.Find("Start Room").GetComponent<BoxCollider>().bounds;
    }
    
    /// <summary>
    /// Allows for an easy method to take screenshots during gameplay.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(screenShotButton))
        {
            ScreenCapture.CaptureScreenshot("screenshot.png");
            Debug.Log("A screenshot was taken!");
        }
    }

    /// <summary>
    /// Returns a random starting location within the confines of the starting room.
    /// </summary>
    /// <returns>Position vector for the new starting position</returns>
    private Vector3 GetRandomStartPosition()
    {
        var halfBoxSize = startRoomBox.size / 2;
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
    private static Quaternion GetRandomRotation()
    {
        var yRotation = Random.Range(-180f, 180f);
        Quaternion rotation = Quaternion.Euler(0, yRotation, 0);
        return rotation;
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

    /// <summary>
    /// Initiates an agent within the environment at runtime.
    /// </summary>
    public void SpawnAgent()
    {
        if (!agentInitialised)
        {
            if (randomStartPos)
            {
                Instantiate(agentPrefab, GetRandomStartPosition(), GetRandomRotation(), transform);
            }
            else
            {
                Instantiate(agentPrefab, Vector3.zero, Quaternion.identity, transform);
            }

            agentInitialised = true;
        }
    }
}
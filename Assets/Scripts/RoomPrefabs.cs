using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Author: Tim Deville (2003506)
///
/// Singleton class that controls which room prefabs are being used in the procedural generation process.
/// </summary>
public class RoomPrefabs : MonoBehaviour
{
    [Header("Room Types")] [SerializeField]
    private GameObject[] starterRoomPrefabs;

    [SerializeField] private GameObject[] intRoomPrefabs;
    [SerializeField] private GameObject[] deadEndRoomPrefabs;
    [SerializeField] private GameObject[] corridorPrefabs;
    [SerializeField] private GameObject[] finishRoomPrefabs;
    private static RoomPrefabs Instance { get; set; }

    private GameObject[] StarterRoomPrefabs => starterRoomPrefabs;
    private GameObject[] IntRoomPrefabs => intRoomPrefabs;
    private GameObject[] DeadEndRoomPrefabs => deadEndRoomPrefabs;
    private GameObject[] CorridorPrefabs => corridorPrefabs;
    private GameObject[] FinishRoomPrefabs => finishRoomPrefabs;

    /// <summary>
    /// Called once, sets up singleton instance.
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    /// <summary>
    ///     Generates a random starting room out of a defined list of prefabs.
    /// </summary>
    /// <returns>Random starting room prefab</returns>
    public static GameObject GetFinishRoom()
    {
        var randomRoomIndex = Random.Range(0, Instance.FinishRoomPrefabs.Length);
        return Instance.FinishRoomPrefabs[randomRoomIndex];
    }
    
    /// <summary>
    ///     Generates a random starting room out of a defined list of prefabs.
    /// </summary>
    /// <returns>Random starting room prefab</returns>
    public static GameObject GetDeadEndRoom()
    {
        var randomRoomIndex = Random.Range(0, Instance.DeadEndRoomPrefabs.Length);
        return Instance.DeadEndRoomPrefabs[randomRoomIndex];
    }

    /// <summary>
    ///     Generates a random starting room out of a defined list of prefabs.
    /// </summary>
    /// <returns>Random starting room prefab</returns>
    public static GameObject GetStarterRoom()
    {
        var randomRoomIndex = Random.Range(0, Instance.StarterRoomPrefabs.Length);
        return Instance.StarterRoomPrefabs[randomRoomIndex];
    }

    /// <summary>
    ///     Generates a random main path room out of a defined list of prefabs.
    /// </summary>
    /// <returns>Random prefab out of a randomly chosen list of types</returns>
    public static GameObject GetMainPathRoom()
    {
        var randomNum = Random.Range(0, 2);
        var roomType = randomNum switch
        {
            0 => Instance.IntRoomPrefabs,
            1 => Instance.CorridorPrefabs,
            _ => throw new ArgumentOutOfRangeException()
        };

        var randomRoomIndex = Random.Range(0, roomType.Length);

        return roomType[randomRoomIndex];
    }

    /// <summary>
    ///     Generates a random branch path room out of a defined list of prefabs.
    /// </summary>
    /// <returns>Random prefab out of a randomly chosen list of types</returns>
    public static GameObject GetBranchRoom()
    {
        var randomNum = Random.Range(1, 101);

        var roomType = randomNum <= 10 ? Instance.CorridorPrefabs : Instance.IntRoomPrefabs;
        
        var randomRoomIndex = Random.Range(0, roomType.Length);

        return roomType[randomRoomIndex];
    }
}
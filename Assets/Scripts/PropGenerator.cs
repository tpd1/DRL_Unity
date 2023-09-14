using UnityEngine;

/// <summary>
/// Author: Tim Deville (2003506)
///
/// When attached to a room prefab, controls the spawning of objects within it at runtime.
/// </summary>
public class PropGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] propPrefabs; // collections of obstacle prefabs
    private bool isDone;

    /// <summary>
    /// Chooses a random set of props from the array and instantiates them within the room.
    /// </summary>
    public void GenerateRoomProps()
    {
        if (isDone || propPrefabs.Length < 1) return;

        var randomIndex = Random.Range(0, propPrefabs.Length);
        Instantiate(propPrefabs[randomIndex], transform.position, transform.rotation, transform);
        isDone = true;
    }
}
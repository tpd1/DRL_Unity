using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Author: Tim Deville (2003506)
///
/// When attached to a room prefab, instantiates the smoke effect in a random position.
/// </summary>
public class SmokeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] smokeSpawnLocations; // User determined locations smoke can be spawned.

    /// <summary>
    /// Called once. initiates the smoke spawning function
    /// </summary>
    private void Start()
    {
        SpawnSmoke();
    }
    
    /// <summary>
    /// Picks a random location from the array and shows the smoke object at that position.
    /// </summary>
    private void SpawnSmoke()
    {
        var randomNum = Random.Range(0f, 1f);

        // decided by percentage chance of spawning smoke.
        if (randomNum <= LevelParameters.Instance.GetSmokeProbability())
        {
            // pick a spawn location and set it to active.
            var randomIndex = Random.Range(0, smokeSpawnLocations.Length);
            smokeSpawnLocations[randomIndex].SetActive(true);
            
        }
    }
    
}
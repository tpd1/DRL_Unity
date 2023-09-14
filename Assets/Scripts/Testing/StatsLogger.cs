using System;
using UnityEngine;

/***
 * Author: Tim Deville (2003506)
 *
 * StatsLogger is attached to a TestAgent prefab and logs statistics during a test run, printing them to
 * the console for further processing.
 */
public class StatsLogger : MonoBehaviour
{
    public int ObstacleCollisions { get; set; }
    private float runTimer = 0f;
    private static int runCount = 0;
    public int RoomsVisited { get; set; }
    [SerializeField] private String testName = "Test Layout 1";
    private bool isStarted = false;
    
    public void SyncStart()
    {
        isStarted = true;
    }


    // Increment run timer every frame.
    void Update()
    {
        if (isStarted) runTimer += Time.deltaTime;
    }


    /*
     * Resets all variables at the end of a test run.
     */
    private void ResetStats()
    {
        runTimer = 0f;
        ObstacleCollisions = 0;
        RoomsVisited = 1;
    }

    /*
     * Prints the Agent's statistics to the console at the end of every test run.
     */
    public void PrintStats(bool wallCollision=false, bool maxSteps=false)
    {
        runCount++;
        isStarted = false;
        string wall = wallCollision ? "Y" : "N";
        string outOfTime = maxSteps ? "Y" : "N";
        
        Debug.Log($"{testName}, Run {runCount}: Time: {runTimer:F2}, Rooms Visited: {RoomsVisited}," +
                  $" Wall Failure: {wall}, Step Failure: {outOfTime}, Obstacle Collisions: {ObstacleCollisions}.");
        
        ResetStats();
    }
    
}

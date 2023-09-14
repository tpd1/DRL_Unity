using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Author: Tim Deville (2003506)
///
/// When attached to an agent, logs its position across the level. At the level end, positions are saved to
/// a JSON file for later processing.
/// </summary>
public class PositionLogger : MonoBehaviour
{
    [SerializeField] private float sampleRate = 0.2f;
    [SerializeField] private int maxRuns = 10;
    public LinePositions agentPositions = new();
    public string filePath = "/10M_Brain_Run_";
    private static int lineCounter = 1;
    private bool isPaused;

    /// <summary>
    /// Called once at the start. Starts the position logging function.
    /// </summary>
    private void Start()
    {
        InvokeRepeating(nameof(LogPosition), 0.0f, sampleRate);
    }

    /// <summary>
    /// Called every frame. Pauses the editor if the test has gone on too long. Allows for tests to
    /// be carried out AFK.
    /// </summary>
    private void Update()
    {
        if (lineCounter >= maxRuns && !isPaused)
        {
            isPaused = true;
            Debug.Break();
        }
    }

    /// <summary>
    /// Periodically adds the agent's position vector to a list of positions.
    /// </summary>
    private void LogPosition()
    {
        agentPositions.lines.Add(new LinePosition(transform.position));
    }

    /// <summary>
    /// Clears the list of positions at the end of an episode.
    /// </summary>
    public void ResetList()
    {
        agentPositions.lines.Clear();
    }

    /// <summary>
    /// Stores the final list of postions to a JSON file in the specified directory.
    /// </summary>
    public void SavePathToJson()
    {
        if (agentPositions.lines.Count > 0)
        {
            string json = JsonUtility.ToJson(agentPositions);
            string savedFilePath = (Application.dataPath + filePath + lineCounter + ".json");
            File.WriteAllText(savedFilePath, json);
            Debug.Log("File Saved: " + savedFilePath);
            agentPositions.lines.Clear();
            lineCounter++;
        }
    }
}

/// <summary>
/// Helper class to allow C# serialisation to work.
/// </summary>
[Serializable]
public class LinePositions
{
    public List<LinePosition> lines;

    public LinePositions()
    {
        lines = new List<LinePosition>();
    }
}

/// <summary>
/// Helper class to allow C# serialisation to work.
/// Contains the agent's x,y and z positions.
/// </summary>
[Serializable]
public class LinePosition
{
    public float x;
    public float y; // Included in case multiple story levels were used.
    public float z;

    public LinePosition(Vector3 vectorPos)
    {
        x = vectorPos.x;
        z = vectorPos.z;
        y = 0f;
    }
}
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Author: Tim Deville (2003506)
///
/// When given a LineRenderer game object, loads a JSON file with agent
/// vector positions and draws a line over the environment, following the agents path.
/// </summary>
public class LineDrawer : MonoBehaviour
{
    [SerializeField] private float yOffset = 0.1f;
    [SerializeField] private string folderPath;
    [SerializeField] private GameObject linePrefab;

    /// <summary>
    /// Called once. Loads all JSON files in a directory corresponding to single agent paths.
    /// Overlays them onto the environment using a Unity LineRenderer prefab.
    /// </summary>
    private void Start()
    {
        DirectoryInfo dir = new DirectoryInfo(folderPath);
        FileInfo[] pathFiles = dir.GetFiles("*.json");

        foreach (var file in pathFiles)
        {
            string json = File.ReadAllText(file.FullName);
            LinePositions characterPositions = JsonUtility.FromJson<LinePositions>(json);

            var vectorPositions = new List<Vector3>();
            foreach (var pos in characterPositions.lines)
            {
                vectorPositions.Add(new Vector3(pos.x, yOffset, pos.z));
            }

            var line = Instantiate(linePrefab);
            var lineRenderer = line.GetComponent<LineRenderer>();
            lineRenderer.positionCount = vectorPositions.Count;
            lineRenderer.SetPositions(vectorPositions.ToArray());
        }
    }
}
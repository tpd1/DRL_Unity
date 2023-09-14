using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

/// <summary>
/// Author: Tim Deville (2003506)
///
/// Main class responsible for procedurally generating environments at runtime.
/// Parts of this code (primarily collision handling) are modified versions of the scripts
/// presented in the Unity dungeon generation tutorial by Billy McDaniel.
/// </summary>
public class FloorGenerator : MonoBehaviour
{
    [SerializeField] private float roomDelay = .5f;
    [SerializeField] private GameObject blockedDoorPrefab;
    [SerializeField] private List<Room> generatedRooms = new();
    [SerializeField] private UnityEvent finishedGeneratingEvent;

    private readonly List<Doorway> unusedDoorways = new();
    private GameObject room1;
    private GameObject room2;
    private GameObject rootRoom;
    private GameObject branchContainer;
    private IEnumerator buildCoroutine; // Used to stop the coroutine (within reset).
    private int tries;
    private const int maxTries = 50;

    /// <summary>
    /// Called once. Caches the coroutine reference.
    /// </summary>
    private void Awake()
    {
        buildCoroutine = BuildMainPath();
    }

    /// <summary>
    /// Returns the bounds of the starting room.
    /// </summary>
    /// <returns>Start room bounding box.</returns>
    public BoxCollider GetStartRoomBounds()
    {
        GameObject startRoom = generatedRooms[0].RoomObject;
        return startRoom.GetComponent<BoxCollider>();
    }

    public List<Room> GetRoomList() => generatedRooms;

    /// <summary>
    /// Begins the coroutine to build the level.
    /// </summary>
    public void InitiateBuild()
    {
        StartCoroutine(buildCoroutine);
    }

    /// <summary>
    /// Clears the current room layout and Starts a new coroutine.
    /// </summary>
    public void ResetLayout()
    {
        ClearLayout();

        // Fetch curriculum values to update the generation complexity
        if (LevelParameters.Instance.IsTrainingMode())
        {
            LevelParameters.Instance.SetComplexity();
        }

        buildCoroutine = BuildMainPath();
        StartCoroutine(buildCoroutine);
    }

    /// <summary>
    /// Deletes all rooms and associated lists.
    /// </summary>
    private void ClearLayout()
    {
        // Stop Coroutine
        StopCoroutine(buildCoroutine);
        buildCoroutine = null;

        // Clear list of generated rooms & doorways
        unusedDoorways.Clear();
        generatedRooms.Clear();
        tries = 0;

        // Destroy all child objects of FloorGenerator (might have to change this to tags if we use agent as child).
        int numOfChildren = transform.childCount;
        for (int i = numOfChildren - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Adds all unconnected doorways to a list for future use.
    /// </summary>
    private void AddUnusedDoorwaysToList()
    {
        foreach (var doorway in branchContainer.GetComponentsInChildren<Doorway>())
        {
            if (!doorway.IsConnected && !unusedDoorways.Contains(doorway))
            {
                unusedDoorways.Add(doorway);
            }
        }
    }

    /// <summary>
    /// Generates random props in each room (make sure to attach script).
    /// </summary>
    private void AddRoomProps()
    {
        if (!LevelParameters.Instance.GenerateProps) return;

        foreach (Room room in generatedRooms)
        {
            var prop = room.RoomObject.GetComponent<PropGenerator>();

            if (prop != null) prop.GenerateRoomProps();
        }
    }

    /// <summary>
    /// The coroutine that procedurally places rooms to form a layout.
    /// </summary>
    private IEnumerator BuildMainPath()
    {
        // Create a new empty game object to store the main path from start to exit.
        branchContainer = new GameObject("Main Path");
        branchContainer.transform.SetParent(transform);

        // Generate Starter room as the root node.
        rootRoom = GenerateStartRoom();
        room2 = rootRoom;

        // Generate rooms in main path (no dead ends, only corridors & intermediate rooms).
        while (generatedRooms.Count < LevelParameters.Instance.MainPathLength)
        {
            yield return new WaitForSeconds(roomDelay);
            room1 = room2;
            room2 = generatedRooms.Count == LevelParameters.Instance.MainPathLength - 1
                ? GenerateFinishRoom()
                : GenerateMainPathRoom();

            ConnectRooms();
            HandleCollisions();
        }

        // Create a list of all unconnected remaining doorways in main path.
        // This will be used create branches off these doorways.
        AddUnusedDoorwaysToList();

        // For each branch off the main path
        for (int i = 0; i < LevelParameters.Instance.MaxBranches; i++)
        {
            // If we have unused doorways
            if (unusedDoorways.Count > 0)
            {
                // Create an empty game object for the branch
                branchContainer = new GameObject("Branch " + i);
                branchContainer.transform.SetParent(transform);

                // Pick a random unused doorway from the main branch and set it as the root.
                int doorIndex = Random.Range(0, unusedDoorways.Count);
                rootRoom = unusedDoorways[doorIndex].transform.parent.parent.gameObject; //Doorway -> Doorways -> Room
                unusedDoorways.RemoveAt(doorIndex);
                room2 = rootRoom;

                // Create each room in the branch.
                for (int j = 0; j < LevelParameters.Instance.BranchLength; j++)
                {
                    yield return new WaitForSeconds(roomDelay);
                    room1 = room2;
                    room2 = j == LevelParameters.Instance.BranchLength - 1
                        ? GenerateDeadEndRoom()
                        : GenerateBranchRoom();
                    ConnectRooms();
                    HandleCollisions();
                    if (tries >= maxTries) break;
                }
            }
            else break;
        }

        FillEmptyDoors();
        AddRoomProps();
        yield return null; // Wait one frame
        finishedGeneratingEvent?.Invoke(); // Let other features (agent script) know generation is complete. 
    }


    /// <summary>
    /// Iterates over all doorways & fills in unused ones with the blocked door prefab.
    /// </summary>
    private void FillEmptyDoors()
    {
        foreach (Doorway doorway in transform.GetComponentsInChildren<Doorway>())
        {
            if (!doorway.IsConnected)
            {
                // Remove the collider that is seen by the agent.
                doorway.tag = "Obstacle";

                var doorBox = doorway.GetComponent<BoxCollider>();
                if (doorBox != null) Destroy(doorBox);

                //Add the blocked door prefab.
                var transform1 = doorway.transform;
                Vector3 doorPosition = transform1.position;
                GameObject blockedDoor = Instantiate(blockedDoorPrefab, doorPosition, transform1.rotation,
                    transform1);
                blockedDoor.name = blockedDoorPrefab.name;
            }
        }
    }

    /// <summary>
    /// Generates a single finish room.
    /// </summary>
    /// <returns>A new finish room.</returns>
    private GameObject GenerateFinishRoom()
    {
        GameObject newRoom = Instantiate(RoomPrefabs.GetFinishRoom(), transform.position,
            Quaternion.identity);

        newRoom.name = "Finish Room";
        newRoom.transform.SetParent(branchContainer.transform); // Add to main path container.
        var origin = generatedRooms[generatedRooms.FindIndex(x => x.RoomObject == room1)].RoomObject;
        generatedRooms.Add(new Room(newRoom, origin));
        return newRoom;
    }

    /// <summary>
    /// Generates a single main path room.
    /// </summary>
    /// <returns>A new main path room.</returns>
    private GameObject GenerateMainPathRoom()
    {
        GameObject newRoom = Instantiate(RoomPrefabs.GetMainPathRoom(), transform.position,
            Quaternion.identity, branchContainer.transform);

        GameObject origin = generatedRooms[generatedRooms.FindIndex(x => x.RoomObject == room1)].RoomObject;
        generatedRooms.Add(new Room(newRoom, origin));

        return newRoom;
    }

    /// <summary>
    /// Generates a single branch room.
    /// </summary>
    /// <returns>A new branch room.</returns>
    private GameObject GenerateBranchRoom()
    {
        GameObject newRoom = Instantiate(RoomPrefabs.GetBranchRoom(), transform.position, Quaternion.identity,
            branchContainer.transform);

        GameObject origin = generatedRooms[generatedRooms.FindIndex(x => x.RoomObject == room1)].RoomObject;
        generatedRooms.Add(new Room(newRoom, origin));

        return newRoom;
    }

    /// <summary>
    /// Generates the start room
    /// </summary>
    /// <returns>A new start room.</returns>
    private GameObject GenerateStartRoom()
    {
        GameObject newRoom = Instantiate(RoomPrefabs.GetStarterRoom(), transform.position,
            Quaternion.Euler(0, Random.Range(0, 4) * 90, 0));
        newRoom.name = "Start Room";
        newRoom.transform.SetParent(branchContainer.transform); // Add to main path container.
        generatedRooms.Add(new Room(newRoom, null));

        return newRoom;
    }

    /// <summary>
    /// Generates a single dead-end room (server/storeroom/bathroom).
    /// </summary>
    /// <returns>A new dead-end room.</returns>
    private GameObject GenerateDeadEndRoom()
    {
        GameObject newRoom = Instantiate(RoomPrefabs.GetDeadEndRoom(), transform.position,
            Quaternion.identity);

        newRoom.transform.SetParent(branchContainer.transform); // Add to main path container.
        var origin = generatedRooms[generatedRooms.FindIndex(x => x.RoomObject == room1)].RoomObject;
        generatedRooms.Add(new Room(newRoom, origin));
        return newRoom;
    }

    /// <summary>
    /// Handles logic associated with connecting two rooms based on their doorway position/orientation.
    /// </summary>
    private void ConnectRooms()
    {
        GameObject doorway1 = RandomDoorwayFromRoom(room1);
        if (doorway1 == null) return;
        GameObject doorway2 = RandomDoorwayFromRoom(room2);
        if (doorway2 == null) return;

        doorway2.transform.SetParent(doorway1.transform); // Set the 2nd doorway to be a child of the first doorway
        room2.transform.SetParent(doorway2.transform); // Set the 2nd room to be a child of the 2nd room

        // Now doorway 2 is a child of doorway 1, reset its position compared to the parent object.
        doorway2.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        doorway2.transform.Rotate(0, 180f, 0); // Rotate doorway 2 so the doorways are at 180 deg to each other.

        room2.transform.SetParent(branchContainer.transform); // Reset parenting of 2nd room back to this script.
        doorway2.transform.SetParent(room2.transform.Find("Doorways"));

        // Delete one of the door colliders, so we have one per used doorway.
        var doorBox = doorway2.GetComponent<BoxCollider>();
        if (doorBox != null) Destroy(doorBox);

        // Delete one of the green shaded indicators.
        var unvisitedDoor = doorway2.transform.Find("Unvisited Door").gameObject;
        if (unvisitedDoor != null) Destroy(unvisitedDoor);

        doorway1.tag = "Unvisited Door";
        generatedRooms.Last().DoorwayObject = doorway1.GetComponent<Doorway>(); // Add doorway to rooms list.
        Physics.SyncTransforms();
    }

    /// <summary>
    /// Chooses a random doorway in a room prefab.
    /// </summary>
    /// <param name="room">The selected prefab to choose from</param>
    /// <returns>A doorway from the selected room.</returns>
    private static GameObject RandomDoorwayFromRoom(GameObject room)
    {
        if (room == null) return null;
        // Get all doorways on the room game object.
        var doorways = room.GetComponentsInChildren<Doorway>().ToList();

        // Remove already connected doorways in the list.
        doorways.RemoveAll(item => item.IsConnected);

        // If there are unconnected doorways, pick one randomly.
        if (doorways.Count > 0)
        {
            int i = Random.Range(0, doorways.Count);
            doorways[i].IsConnected = true;
            return doorways[i].gameObject;
        }

        return null;
    }

    /// <summary>
    /// Handles collisions when placing one room next to another. Recursively attempts to retry placing rooms
    /// until successfully, otherwise backtracks and chooses another doorway to branch from.
    /// </summary>
    private void HandleCollisions()
    {
        BoxCollider roomBoxCollider = room2.GetComponent<BoxCollider>();

        LayerMask layerMask = LayerMask.GetMask("RoomBox");

        Transform room2Transform = room2.transform;
        var roomBoxCenter = roomBoxCollider.center;
        Vector3 offset = (room2Transform.right * roomBoxCenter.x) +
                         (room2Transform.up * roomBoxCenter.y) +
                         (room2Transform.forward * roomBoxCenter.z);

        // Make a list of rooms that overlap the physics box.
        var hits = Physics
            .OverlapBox(room2Transform.position + offset, roomBoxCollider.size / 2, Quaternion.identity, layerMask)
            .ToList();

        // If a collision is detected
        if (hits.Count <= 0) return;

        if (hits.Exists(col => col != roomBoxCollider))
        {
            tries++;
            int toIndex = generatedRooms.FindIndex(room => room.RoomObject == room2);
            Doorway d = generatedRooms[toIndex].DoorwayObject;
            if (d != null)
            {
                d.IsConnected = false;
            }

            generatedRooms.RemoveAt(toIndex);
            DestroyImmediate(room2);

            // If we have retried placing a room too many times
            if (tries >= maxTries)
            {
                int fromIndex = generatedRooms.FindIndex(room => room.RoomObject == room1);
                Room newRoom1 = generatedRooms[fromIndex];

                if (room1 != rootRoom)
                {
                    if (newRoom1.DoorwayObject != null)
                    {
                        newRoom1.DoorwayObject.IsConnected = false;
                    }

                    unusedDoorways.RemoveAll(door => door.transform.parent.parent.gameObject == room1);
                    generatedRooms.RemoveAt(fromIndex);
                    DestroyImmediate(room1);

                    if (newRoom1.OriginObject != rootRoom)
                    {
                        room1 = newRoom1.OriginObject;
                    }
                    else if (branchContainer.name.Contains("Main"))
                    {
                        if (newRoom1.OriginObject != null)
                        {
                            rootRoom = newRoom1.OriginObject;
                            room1 = rootRoom;
                        }
                    }
                    else if (unusedDoorways.Count > 0)
                    {
                        int index = Random.Range(0, unusedDoorways.Count);
                        rootRoom = unusedDoorways[index].transform.parent.parent.gameObject;
                        unusedDoorways.RemoveAt(index);
                        room1 = rootRoom;
                    }
                    else return;
                }
                else if (branchContainer.name.Contains("Main"))
                {
                    if (newRoom1.OriginObject != null)
                    {
                        rootRoom = newRoom1.OriginObject;
                        room1 = rootRoom;
                    }
                }
                else if (unusedDoorways.Count > 0)
                {
                    int index = Random.Range(0, unusedDoorways.Count);
                    rootRoom = unusedDoorways[index].transform.parent.parent.gameObject;
                    unusedDoorways.RemoveAt(index);
                    room1 = rootRoom;
                }
                else return;
            }
            if (room1 != null)
            {
                if (generatedRooms.Count == LevelParameters.Instance.MainPathLength - 1)
                {
                    room2 = GenerateFinishRoom();
                }
                else
                {
                    room2 = branchContainer.name.Contains("Main") ? GenerateMainPathRoom() : GenerateBranchRoom();
                }

                ConnectRooms();
                HandleCollisions();
            }
        }
        else tries = 0;
    }
}
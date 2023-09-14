using System;
using UnityEngine;

/// <summary>
/// Author: Tim Deville (2003506)
///
/// Models an individual room within the procedurally generated envrionment.
/// </summary>
[Serializable]
public class Room
{
    // Create fields separately to expose in editor.
    [SerializeField] private GameObject roomObject; // The room itself
    [SerializeField] private GameObject originObject; // The originating room
    [SerializeField] private Doorway doorwayObject; // The connecting doorway
    
    public Room(GameObject room, GameObject origin)
    {
        roomObject = room;
        originObject = origin;
    }

    public GameObject RoomObject
    {
        get => roomObject;
        set => roomObject = value;
    }

    public GameObject OriginObject
    {
        get => originObject;
        set => originObject = value;
    }

    public Doorway DoorwayObject
    {
        get => doorwayObject;
        set => doorwayObject = value;
    }
}
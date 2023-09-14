using UnityEngine;

/// <summary>
/// Author: Tim Deville (2003506)
///
/// When attached to a doorway object, controls what happens upon agent interaction events.
/// </summary>
public class Doorway : MonoBehaviour
{
    private GameObject unvisitedDoor;
    private BoxCollider boxCollider;
    public bool IsConnected { get; set; }

    /// <summary>
    /// Called once. Fetches a reference to the unvisited door component.
    /// </summary>
    private void Awake()
    {
        boxCollider = transform.GetComponent<BoxCollider>();

        var unvisitedDoorTransform = transform.Find("Unvisited Door");
        if (unvisitedDoorTransform != null)
        {
            unvisitedDoor = unvisitedDoorTransform.gameObject;
        }
    }

    /// <summary>
    /// When the agent exits the door's collider, the box collider is hidden for the remainder
    /// of the episode.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Hide collider and unvisited door graphic.
            if (boxCollider != null) boxCollider.enabled = false;
            if (unvisitedDoor != null) unvisitedDoor.SetActive(false);
        }
    }

    /// <summary>
    /// Resets the doorway to its original state.
    /// </summary>
    public void ResetDoor()
    {
        transform.tag = "Unvisited Door";
        if (boxCollider != null) boxCollider.enabled = true;
        if (unvisitedDoor != null) unvisitedDoor.SetActive(true);
    }
}
using Cinemachine;
using UnityEngine;

/// <summary>
/// Author: Tim Deville (2003506)
///
/// Controls the camera during testing. Allows for zoom and rotation.
/// </summary>
public class TestingCamera : MonoBehaviour
{
    [SerializeField] private float zoomAmount = 10f;
    [SerializeField] private float rotationSpeed = 100f;
    private Vector3 followOffset;
    private CinemachineFramingTransposer framingTransposer;

    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 100f;

    /// <summary>
    /// Called once. Caches reference to camera components.
    /// </summary>
    private void Awake()
    {
        var cinemachineCamera = transform.GetComponent<CinemachineVirtualCamera>();
        framingTransposer = cinemachineCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        followOffset = framingTransposer.m_TrackedObjectOffset;
    }

    /// <summary>
    /// Called every frame. Calls functions to handle zoom and rotation. 
    /// </summary>
    private void Update()
    {
        HandleRotation();
        HandleZoom();
    }

    /// <summary>
    /// Handles the camera rotation input in a smooth manner.
    /// </summary>
    private void HandleRotation()
    {
        Vector3 rotateInput = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.Q)) rotateInput.y = +1f;
        if (Input.GetKey(KeyCode.E)) rotateInput.y = -1f;

        transform.eulerAngles += rotateInput * (rotationSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Handles the camera zoom input in a smooth manner.
    /// </summary>
    private void HandleZoom()
    {
        if (Input.mouseScrollDelta.y > 0) followOffset.y -= zoomAmount;
        if (Input.mouseScrollDelta.y < 0) followOffset.y += zoomAmount;

        followOffset.y = Mathf.Clamp(followOffset.y, minZoom, maxZoom);

        framingTransposer.m_TrackedObjectOffset =
            Vector3.Lerp(framingTransposer.m_TrackedObjectOffset, followOffset, Time.deltaTime * zoomAmount);
    }
}
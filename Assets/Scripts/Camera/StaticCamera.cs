using Cinemachine;
using UnityEngine;

public class StaticCamera : MonoBehaviour
{
    //[SerializeField] private float zoomAmount = 10f;
    [SerializeField] private float rotationSpeed = 100f;
    private Vector3 followOffset;
    
    //[SerializeField] private float minZoom = 5f;
    //[SerializeField] private float maxZoom = 100f;

    private void Awake()
    {
        var cinemachineCamera = transform.GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        HandleRotation();
        //HandleZoom();
    }

    private void HandleRotation()
    {
        Vector3 rotateInput = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.Q)) rotateInput.y = +1f;
        if (Input.GetKey(KeyCode.E)) rotateInput.y = -1f;

        transform.eulerAngles += rotateInput * (rotationSpeed * Time.deltaTime);
    }

    private void HandleZoom()
    {
        // if (Input.mouseScrollDelta.y > 0) followOffset.y -= zoomAmount;
        // if (Input.mouseScrollDelta.y < 0) followOffset.y += zoomAmount;
        //
        // followOffset.y = Mathf.Clamp(followOffset.y, minZoom, maxZoom);
        //
        // framingTransposer.m_TrackedObjectOffset =
        //     Vector3.Lerp(framingTransposer.m_TrackedObjectOffset, followOffset, Time.deltaTime * zoomAmount);
    }
}
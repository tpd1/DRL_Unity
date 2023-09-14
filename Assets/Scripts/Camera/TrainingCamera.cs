using UnityEngine;

/// <summary>
/// Author: Tim Deville (2003506)
///
/// Controls the camera during training. Allows for zoom and rotation and centres the camera on
/// the randomly generated level.
/// </summary>
public class TrainingCamera : MonoBehaviour
{
    [Header("Target Environment Instance")]
    [SerializeField] private AgentManager targetAgentManager;
    
    [Header("Camera Control")]
    [SerializeField] private float panSpeed = 20f;
    [SerializeField] private float cameraHeight = 80f;
    [SerializeField] private float  rotationSpeed = 100f;
    [SerializeField] private float scrollSpeed = 25f;
    
    private float minZoom = 10f;
    private float maxZoom = 150f;

    /// <summary>
    /// Called every physics update. Allows the user basic camera control using keyboard input.
    /// </summary>
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        if (Input.GetKey(KeyCode.W)) pos.z += panSpeed * Time.fixedDeltaTime;
        if (Input.GetKey(KeyCode.A)) pos.x -= panSpeed * Time.fixedDeltaTime;
        if (Input.GetKey(KeyCode.S)) pos.z -= panSpeed * Time.fixedDeltaTime;
        if (Input.GetKey(KeyCode.D)) pos.x += panSpeed * Time.fixedDeltaTime;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        pos.y -= scroll * scrollSpeed * Time.fixedDeltaTime;
        pos.y = Mathf.Clamp(pos.y, minZoom, maxZoom);
        transform.position = pos;
        
        var rotationVector = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.Q)) rotationVector.y = +1f;
        if (Input.GetKey(KeyCode.E)) rotationVector.y = -1f;
        transform.eulerAngles += rotationSpeed * Time.fixedDeltaTime * rotationVector;
    }

    /// <summary>
    /// Centres the camera position on the centre of the generated layout.
    /// </summary>
    public void CenterCameraOnLevel()
    {
        // Have to separate this from Awake/start so it runs after floor generation co-routine. 
        if (targetAgentManager != null)
        { 
            Vector3 newPosition = targetAgentManager.GetLevelCenterPoint();
            newPosition.y = cameraHeight;

            transform.position = newPosition;
            transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
    }
}

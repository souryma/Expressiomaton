using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DollyZoom : MonoBehaviour
{
    [SerializeField]
    private Camera cameraP1;
    
    [SerializeField]
    private Camera cameraP2;
    
    [SerializeField]
    private Transform targetP1;
    
    [SerializeField]
    private Transform targetP2;
    
    public int timeZoom = 10;

    public bool doZoom = false;

    private float speedCam;
    private float initialFrustrumHeightP1;
    private float initialFrustrumHeightP2;
    
    // Start is called before the first frame update
    void Awake()
    {
        speedCam = 65f / (timeZoom * 10f);
        Initialize();
    }
    
    // Update is called once per frame
    void Update()
    {
        float currentDistance = Vector3.Distance(cameraP1.transform.position, targetP1.position);
        if (currentDistance >= 2.2f && doZoom)
        {
            DoZoomForCamera(cameraP1, currentDistance, initialFrustrumHeightP1);
            DoZoomForCamera(cameraP2, currentDistance, initialFrustrumHeightP2);
        }
    }

    private void Initialize()
    {
        float distanceFromTarget = Vector3.Distance(cameraP1.transform.position, targetP1.transform.position);
        initialFrustrumHeightP1 = computeFrustrumHeight(cameraP1, distanceFromTarget);
        distanceFromTarget = Vector3.Distance(cameraP2.transform.position, targetP2.transform.position);
        initialFrustrumHeightP2 = computeFrustrumHeight(cameraP2, distanceFromTarget);
    }

    private void DoZoomForCamera(Camera TargetCamera, float distance, float initialFrustrumHeight)
    {
        TargetCamera.transform.Translate(speedCam * Vector3.forward * Time.deltaTime);
        
        TargetCamera.fieldOfView = computeFieldOfView(initialFrustrumHeight, distance);
    }

    private float computeFrustrumHeight(Camera TargetCamera, float distance)
    {
        return (2.0f * distance * Mathf.Tan(TargetCamera.fieldOfView * 0.5f * Mathf.Deg2Rad));
    }
    private float computeFieldOfView(float height, float distance)
    {
        return (2.0f * Mathf.Atan(height * 0.5f / distance) * Mathf.Rad2Deg);
    }
}

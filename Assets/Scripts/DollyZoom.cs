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
    
    [SerializeField]
    private float dollySpeed = 2.0f;

    private float initialFrustrumHeightP1;
    private float initialFrustrumHeightP2;
    
    // Start is called before the first frame update
    void Awake()
    {
        Initialize(cameraP1, targetP1, initialFrustrumHeightP1);
        Initialize(cameraP2, targetP2, initialFrustrumHeightP2);
    }
    
    // Update is called once per frame
    void Update()
    {
        float currentDistance = Vector3.Distance(cameraP1.transform.position, targetP1.position);
        if (currentDistance >= 2.0f)
        {
            DoZoomForCamera(cameraP1, currentDistance, initialFrustrumHeightP1);
            DoZoomForCamera(cameraP2, currentDistance, initialFrustrumHeightP2);
        }
    }

    private void Initialize(Camera camera, Transform target, float initialFrustrumHeight)
    {
        float distanceFromTarget = Vector3.Distance(camera.transform.position, target.position);
        initialFrustrumHeight = computeFrustrumHeight(camera, distanceFromTarget);
    }

    private void DoZoomForCamera(Camera camera, float distance, float initialFrustrumHeight)
    {
        camera.transform.Translate(1 * Vector3.forward * Time.deltaTime * dollySpeed);
        camera.fieldOfView = computeFieldOfView(initialFrustrumHeight, distance);
    }

    private float computeFrustrumHeight(Camera camera, float distance)
    {
        return (2.0f * distance * Mathf.Tan(camera.fieldOfView * 0.5f * Mathf.Deg2Rad));
    }
    private float computeFieldOfView(float height, float distance)
    {
        return (2.0f * Mathf.Atan(height * 0.5f / distance) * Mathf.Rad2Deg);
    }
}

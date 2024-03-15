using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebcamToMaterial : MonoBehaviour
{
    [SerializeField] private MeshRenderer targetMaterial;

    public enum Cameras
    {
        Webcam1,
        Webcam2
    }

    [SerializeField] private Cameras _cameras = Cameras.Webcam1;

    private bool _isActivated = false;

    // Start is called before the first frame update
    void Update()
    {
        if (WebcamManager.instance.isCameraSetup && !_isActivated)
        {
            if (_cameras == Cameras.Webcam1)
                targetMaterial.material.mainTexture = WebcamManager.instance.Face1Texture;
            else
                targetMaterial.material.mainTexture = WebcamManager.instance.Face2Texture;

            _isActivated = true;
        }
    }
}
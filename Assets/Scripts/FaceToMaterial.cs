using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceToMaterial : MonoBehaviour
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
        if (!WebcamManager.instance.isCameraSetup || _isActivated) return;
        
        targetMaterial.material.mainTexture = _cameras == Cameras.Webcam1 ? WebcamManager.instance.Face1Texture : WebcamManager.instance.Face2Texture;

        _isActivated = true;
    }
}
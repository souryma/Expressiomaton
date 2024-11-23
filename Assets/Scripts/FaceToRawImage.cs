using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FaceToRawImage : MonoBehaviour
{
    [SerializeField] private RawImage targetImage;

    public enum Cameras
    {
        Webcam1,
        Webcam2
    }

    [SerializeField] private Cameras _cameras = Cameras.Webcam1;

    private bool _isActivated = false;

    // Start is called before the first frame update
    private void Start()
    {
        if (!WebcamManager.instance.isCameraSetup || _isActivated) return;
        
        targetImage.texture = _cameras == Cameras.Webcam1 ? WebcamManager.instance.Face1Texture : WebcamManager.instance.Face2Texture;

        _isActivated = true;
    }
}
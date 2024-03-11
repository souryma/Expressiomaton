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

    // Start is called before the first frame update
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (_cameras == Cameras.Webcam1)
            {
                targetMaterial.material.mainTexture = WebcamManager.instance.Webcam1Texture;
            }
            else
            {
                targetMaterial.material.mainTexture = WebcamManager.instance.Webcam2Texture;
            }
        }
    }
}
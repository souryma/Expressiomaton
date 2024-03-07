using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebcamToMaterial : MonoBehaviour
{
    [SerializeField] private MeshRenderer targetMaterial;

    // Start is called before the first frame update
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            targetMaterial.material.mainTexture = WebcamManager.instance.Webcam1Texture;
        }
    }
}
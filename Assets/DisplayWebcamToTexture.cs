using UnityEngine;
using UnityEngine.UI;

public class DisplayWebcamToTexture : MonoBehaviour
{
    public RawImage Image;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Image.texture = WebcamManager.instance.Webcam1Texture;
        }
    }
}

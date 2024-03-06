using UnityEngine;
using UnityEngine.UI;

public class DisplayWebcamToTexture : MonoBehaviour
{
    public RawImage Image;
    public WebcamManager WebcamManager;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Image.texture = WebcamManager.Webcam1Texture;
        }
    }
}

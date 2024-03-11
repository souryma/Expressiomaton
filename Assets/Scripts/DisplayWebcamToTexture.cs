using UnityEngine;
using UnityEngine.UI;

public class DisplayWebcamToTexture : MonoBehaviour
{
    [SerializeField]
    public RawImage Image;

    [SerializeField]
    public RawImage Image2;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Image.texture = WebcamManager.instance.Webcam1Texture;
            Image2.texture = WebcamManager.instance.Webcam2Texture;
        }
    }
}

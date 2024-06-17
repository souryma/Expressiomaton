using UnityEngine;
using UnityEngine.UI;

public class DisplayWebcamToTexture : MonoBehaviour
{
    [SerializeField]
    public RawImage Image;

    [SerializeField]
    public RawImage Image2;
    
    private bool _isActivated = false;
    
    private void Update()
    {
        if (WebcamManager.instance.isCameraSetup && !_isActivated)
        {
            Image.texture = WebcamManager.instance.Webcam1;

            _isActivated = true;
        }
    }
}

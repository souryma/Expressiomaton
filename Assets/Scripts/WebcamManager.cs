using Unity.Mathematics;
using UnityEngine;

public class WebcamManager : MonoBehaviour
{
    public static WebcamManager instance;
    
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    
    [SerializeField] private string _camera1Name = "";
    [SerializeField] private string _camera2Name = "";

    [SerializeField] private int2 _cameraTextureResolutions = new int2(512, 512);

    private WebCamTexture _webcam1;
    private WebCamTexture _webcam2;

    private RenderTexture _webcam1Texture;
    private RenderTexture _webcam2Texture;

    public RenderTexture Webcam1Texture
    {
        get => _webcam1Texture;
        set => _webcam1Texture = value;
    }

    public RenderTexture Webcam2Texture
    {
        get => _webcam2Texture;
        set => _webcam2Texture = value;
    }

    void Start()
    {
        _webcam1 = new WebCamTexture(_camera1Name);
        _webcam2 = new WebCamTexture(_camera2Name);
        
        _webcam1Texture = new RenderTexture(_cameraTextureResolutions.x, _cameraTextureResolutions.y, 0);
        _webcam2Texture = new RenderTexture(_cameraTextureResolutions.x, _cameraTextureResolutions.y, 0);

        _webcam1.Play();
        _webcam2.Play();
        
        foreach (var device in WebCamTexture.devices)
        {
            Debug.Log(device.name);
        }
    }

    void OnDestroy()
    {
        if (_webcam1 != null) Destroy(_webcam1);
        if (_webcam2 != null) Destroy(_webcam2);
        
        if (_webcam1Texture != null) Destroy(_webcam1Texture);
        if (_webcam2Texture != null) Destroy(_webcam2Texture);
    }

    public void PauseCameras()
    {
        _webcam1.Pause();
        _webcam2.Pause();
    }

    public void PlayCameras()
    {
        _webcam1.Play();
        _webcam2.Play();
    }

    void Update()
    {
        // Crop the cameras render to a square of the desired resolution
        var scale = new Vector2((float) _webcam1.height / _webcam1.width, 1);
        var offset = new Vector2(scale.x / 2, 0);
        Graphics.Blit(_webcam1, _webcam1Texture, scale, offset);
        
        scale = new Vector2((float) _webcam2.height / _webcam2.width, 1);
        offset = new Vector2(scale.x / 2, 0);
        Graphics.Blit(_webcam2, _webcam2Texture, scale, offset);
    }
}
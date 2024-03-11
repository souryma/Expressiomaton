using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class WebcamManager : MonoBehaviour
{
    public static WebcamManager instance;

    [SerializeField] private TMP_Dropdown _camera1Choice;
    [SerializeField] private TMP_Dropdown _camera2Choice;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        DontDestroyOnLoad(this);
    }

    [SerializeField] private int2 _cameraTextureResolutions = new int2(512, 512);

    private WebCamTexture _webcam1;
    private WebCamTexture _webcam2;

    private RenderTexture _webcam1Texture;
    private RenderTexture _webcam2Texture;

    public RenderTexture Webcam1Texture => _webcam1Texture;
    public RenderTexture Webcam2Texture => _webcam2Texture;

    private List<TMP_Dropdown.OptionData> _camerasNameList;

    [HideInInspector] public bool isCameraSetup = false;

    void Start()
    {
        _camerasNameList = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
            data.text = WebCamTexture.devices[i].name;
            _camerasNameList.Add(data);
        }

        _camera2Choice.options = _camerasNameList;
        _camera1Choice.options = _camerasNameList;
    }

    // Will select the camera names that are defined in the dropdowns
    public void ConfirmCameraSelection()
    {
        _webcam1 = new WebCamTexture(_camerasNameList[_camera1Choice.value].text);
        _webcam2 = new WebCamTexture(_camerasNameList[_camera2Choice.value].text);

        _webcam1Texture = new RenderTexture(_cameraTextureResolutions.x, _cameraTextureResolutions.y, 0);
        _webcam2Texture = new RenderTexture(_cameraTextureResolutions.x, _cameraTextureResolutions.y, 0);

        _webcam1.Play();
        _webcam2.Play();

        isCameraSetup = true;
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
        if (!isCameraSetup) return;

        // Crop the cameras render to a square of the desired resolution
        var scale = new Vector2((float) _webcam1.height / _webcam1.width, 1);
        Graphics.Blit(_webcam1, _webcam1Texture, scale, new Vector2(0, 0));

        var scale2 = new Vector2((float) _webcam2.height / _webcam2.width, 1);
        Graphics.Blit(_webcam2, _webcam2Texture, scale2, new Vector2(0, 0));
    }
}
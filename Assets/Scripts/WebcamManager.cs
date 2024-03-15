using System.Collections.Generic;
using System.Linq;
using OpenCvSharp.Demo;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class WebcamManager : MonoBehaviour
{
    public static WebcamManager instance;

    [SerializeField] private TMP_Dropdown _camera1Choice;
    [SerializeField] private TMP_Dropdown _camera2Choice;

    [Header("Image Processor")] public TextAsset faces;
    public TextAsset eyes;
    public TextAsset shapes;
    public bool forceFrontalCamera;

    private FaceProcessorLive<Texture2D> processorWebCam1;
    private FaceProcessorLive<Texture2D> processorWebCam2;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        ScenesManager.instance.OnStartSceneLoaded += SetupCameras;
    }

    [SerializeField] private int2 _cameraTextureResolutions = new int2(512, 512);

    private WebCamTexture _webcam1;
    private WebCamTexture _webcam2;
    private WebCamDevice _webcamDevice1;
    private WebCamDevice _webcamDevice2;
    private Texture2D _face1;
    private Texture2D _face2;

    private RenderTexture _webcam1Texture;
    private RenderTexture _webcam2Texture;

    public RenderTexture Webcam1Texture => _webcam1Texture;
    public RenderTexture Webcam2Texture => _webcam2Texture;

    private List<TMP_Dropdown.OptionData> _camerasNameList;

    [HideInInspector] public bool isCameraSetup = false;

    private void SetupCameras()
    {
        _camerasNameList = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
            data.text = WebCamTexture.devices[i].name;
            _camerasNameList.Add(data);
        }

        bool hasPassed = false;
        foreach (var dropdown in FindObjectsOfType<TMP_Dropdown>(true))
        {
            if (hasPassed)
            {
                _camera2Choice = dropdown;
            }
            else
            {
                _camera1Choice = dropdown;
                hasPassed = true;
            }
        }

        _camera2Choice.options = _camerasNameList;
        _camera1Choice.options = _camerasNameList;
        processorInitializer(ref processorWebCam2);
        processorInitializer(ref processorWebCam1);
    }

    private void processorInitializer(ref FaceProcessorLive<Texture2D> processor)
    {
        processor = new FaceProcessorLive<Texture2D>();
        processor.Initialize(faces.text, eyes.text, shapes.bytes);

        // data stabilizer - affects face rects, face landmarks etc.
        processor.DataStabilizer.Enabled = true; // enable stabilizer
        processor.DataStabilizer.Threshold = 2.0; // threshold value in pixels
        processor.DataStabilizer.SamplesCount = 2; // how many samples do we need to compute stable data

        // performance data - some tricks to make it work faster
        processor.Performance.Downscale = 256; // processed image is pre-scaled down to N px by long side
        processor.Performance.SkipRate =
            0; // we actually process only each Nth frame (and every frame for skipRate = 0)
    }

    // Will select the camera names that are defined in the dropdowns
    public void ConfirmCameraSelection()
    {
        _webcam1 = new WebCamTexture(_camerasNameList[_camera1Choice.value].text);
        _webcam2 = new WebCamTexture(_camerasNameList[_camera2Choice.value].text);

        _webcamDevice1 = WebCamTexture.devices[_camera1Choice.value];
        _webcamDevice1 = WebCamTexture.devices[_camera2Choice.value];
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

    /// <summary>
    /// Per-frame video capture processor
    /// </summary>
    private Texture2D ProcessTexture(WebCamDevice webCamDevice, WebCamTexture input,
        FaceProcessorLive<Texture2D> processor)
    {
        OpenCvSharp.Unity.TextureConversionParams textureParameters =
            ReadTextureConversionParameters(webCamDevice, input);
        // detect everything we're interested in
        Texture2D texture2D = new Texture2D(input.width / 4, input.height, TextureFormat.RGBA32, false);
        texture2D.SetPixels(input.GetPixels(input.width / 4, 0, input.width / 2, input.height));
        // texture2D.ReadPixels(new UnityEngine.Rect(input.width/3, 0, input.width/3, input.height), 0,0);
        // Graphics.Blit(input, receiver, new Vector2(1f, 1f), new Vector2(input.width/3, 0f),0,0 );
        texture2D.Apply();
        processor.ProcessTexture(texture2D, textureParameters);

        // mark detected objects
        // processor.MarkDetected();


        // processor.Image now holds data we'd like to visualize
        texture2D = OpenCvSharp.Unity.MatToTexture(processor.Image, texture2D);
        if (processor.Faces.Count != 0)
        {
            var rect = processor.Faces.Last();
            Texture2D faceTexture = new Texture2D(rect.Region.Width, rect.Region.Height, TextureFormat.RGBA32, false);

            int yFromBottom = texture2D.height - rect.Region.Y - rect.Region.Height;
            faceTexture.SetPixels(texture2D.GetPixels(rect.Region.X, yFromBottom, rect.Region.Width, rect
                .Region.Height));
            faceTexture.Apply();
            return faceTexture;
        }

        return null;
    }

    /// <summary>
    /// This method scans source device params (flip, rotation, front-camera status etc.) and
    /// prepares TextureConversionParameters that will compensate all that stuff for OpenCV
    /// </summary>
    private OpenCvSharp.Unity.TextureConversionParams ReadTextureConversionParameters(WebCamDevice webCamDevice,
        WebCamTexture webCamTexture)
    {
        OpenCvSharp.Unity.TextureConversionParams parameters = new OpenCvSharp.Unity.TextureConversionParams();

        // frontal camera - we must flip around Y axis to make it mirror-like
        parameters.FlipHorizontally = forceFrontalCamera || webCamDevice.isFrontFacing;

        // TODO:
        // actually, code below should work, however, on our devices tests every device except iPad
        // returned "false", iPad said "true" but the texture wasn't actually flipped

        // compensate vertical flip
        //parameters.FlipVertically = webCamTexture.videoVerticallyMirrored;

        // deal with rotation
        if (0 != webCamTexture.videoRotationAngle)
            parameters.RotationAngle = webCamTexture.videoRotationAngle; // cw -> ccw

        // apply
        return parameters;

        //UnityEngine.Debug.Log (string.Format("front = {0}, vertMirrored = {1}, angle = {2}", webCamDevice.isFrontFacing, webCamTexture.videoVerticallyMirrored, webCamTexture.videoRotationAngle));
    }

    void Update()
    {
        // Crop the cameras render to a square of the desired resolution
        if (!isCameraSetup) return;
        if (_webcam1 != null && _webcam1.didUpdateThisFrame)
        {
            _face1 = ProcessTexture(_webcamDevice1, _webcam1, processorWebCam1);
        }

        if (_webcam2 != null && _webcam2.didUpdateThisFrame)
        {
            _face2 = ProcessTexture(_webcamDevice2, _webcam2, processorWebCam2);
        }

        if (_face1 is not null)
        {
            var scale = new Vector2((float) _face1.height / _face1.width, 1);
            Graphics.Blit(_face1, _webcam1Texture, scale, new Vector2(0, 0));
        }

        if (_face2 is not null)
        {
            var scale2 = new Vector2((float) _face2.height / _face2.width, 1);
            Graphics.Blit(_face2, _webcam2Texture, scale2, new Vector2(0, 0));
        }
    }
}
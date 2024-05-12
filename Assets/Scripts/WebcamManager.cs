using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UltraFace;

public sealed class WebcamManager : MonoBehaviour
{
    public static WebcamManager instance;

    #region Private members
    
    [SerializeField] private TMP_Dropdown _camera1Choice;
    [SerializeField] private TMP_Dropdown _camera2Choice;
    
    // private FaceProcessorLive<Texture2D> _processorWebCam1;
    // private FaceProcessorLive<Texture2D> _processorWebCam2;
    [Header("New Image Processor")] 
    // [SerializeField] RenderTexture renderTextureFace1 = null;
    // [SerializeField] RenderTexture renderTextureFace2 = null;
    [SerializeField, Range(0, 1)]private float _threshold = 0.5f;
    [SerializeField]private ResourceSet _resources = null;
    // [SerializeField] Shader _visualizer = null;
    private FaceDetector _detector;
    // private Material _material;
    private bool _face1Detected = false;
    private Detection? _lastFace1Detection;
    private bool _face2Detected = false;
    private Detection? _lastFace2Detection;

    private readonly float _roundingValue = 100f;
    
    [SerializeField] private int2 _cameraTextureResolutions = new int2(512, 512);

    private WebCamTexture _webcam1;
    private WebCamTexture _webcam2;

    private RenderTexture _face1Texture;
    private RenderTexture _face2Texture;
    
    private const float ErrorMarginX = 0.02f;
    private const float ErrorMarginY = 0.04f;
    
    private List<TMP_Dropdown.OptionData> _camerasNameList;
    #endregion

    #region Public members
    public RenderTexture Face1Texture => _face1Texture;

    public RenderTexture Face2Texture => _face2Texture;

    public WebCamTexture Webcam1 => _webcam1;

    public WebCamTexture Webcam2 => _webcam2;

    #endregion
    
   

    [HideInInspector] public bool isCameraSetup = false;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        FaceDetectorInitializer();

        if (ScenesManager.isSceneManagerLoaded)
        {
            ScenesManager.instance.OnStartSceneLoaded += SetupCameras;
        }
        else
        {
            SetupCameras();
        }
    }

  
    private void SetupCameras()
    {
        _camerasNameList = new List<TMP_Dropdown.OptionData>();
        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            TMP_Dropdown.OptionData data = new TMP_Dropdown.OptionData();
            data.text = WebCamTexture.devices[i].name;
            _camerasNameList.Add(data);
        }

        foreach (var dropdown in FindObjectsOfType<TMP_Dropdown>(true))
        {
            if (dropdown.CompareTag("Player2"))
            {
                
                _camera2Choice = dropdown;
            }
            else
            {
                _camera1Choice = dropdown;
            }
        }

        _camera1Choice.options = _camerasNameList;
        _camera2Choice.options = _camerasNameList;
    }

    private void FaceDetectorInitializer()
    {
        _detector = new FaceDetector(_resources);
    }


    // Will select the camera names that are defined in the dropdowns
    public void ConfirmCameraSelection()
    {
        _webcam1 = new WebCamTexture(_camerasNameList[_camera1Choice.value].text);
        _webcam2 = new WebCamTexture(_camerasNameList[_camera2Choice.value].text);
        
        _face1Texture = new RenderTexture(_cameraTextureResolutions.x, _cameraTextureResolutions.y, 0);
        _face2Texture = new RenderTexture(_cameraTextureResolutions.x, _cameraTextureResolutions.y, 0);
        
        _webcam1.Play();
        _webcam2.Play();

        isCameraSetup = true;
    }

    private void OnDestroy()
    {
        if (ScenesManager.isSceneManagerLoaded)
            ScenesManager.instance.OnStartSceneLoaded -= SetupCameras;
        
        if (_webcam1 != null) Destroy(_webcam1);
        if (_webcam2 != null) Destroy(_webcam2);
        
        if (_face1Texture != null) Destroy(_face1Texture);
        if (_face2Texture != null) Destroy(_face2Texture);
        
        DestroyFaceDetector();
    }

    private void DestroyFaceDetector()
    {
        _detector?.Dispose();
    }
    
    private void FaceDetectorDetectFace(WebCamTexture webCamTexture, RenderTexture renderTexture, ref bool 
            faceDetected, ref Detection? lastDetection)
    {
        _detector.ProcessImage(webCamTexture, _threshold);


        // Marker update
        if (_detector.Detections.Any())
        {
            faceDetected = true;
            Vector2 imageCenter = new Vector2(0.5f, 0.5f);
            var savedDetection = _detector.Detections.First();
            Vector2 vec2 = new Vector2(savedDetection.x1 + (savedDetection.x2 - savedDetection.x1) / 2,
                savedDetection.y1 + (savedDetection.y2 - savedDetection.y1) / 2);
            float smallestDistance = Vector2.Distance(imageCenter, vec2);
            foreach (var detection in _detector.Detections)
            {
                if (detection.Equals(savedDetection)) continue;
                Vector2 center = new Vector2(detection.x1 + (detection.x2 - detection.x1) / 2,
                    detection.y1 + (detection.y2 - detection.y1) / 2);
                float distance = Vector2.Distance(imageCenter, center);
                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    savedDetection = detection;
                }
            }

            if (lastDetection != null)
            {
                var centerSavedX = savedDetection.GetCenterX();
                var centerSavedY = savedDetection.GetCenterY();
                var centerLastX = lastDetection?.GetCenterX();
                var centerLastY = lastDetection?.GetCenterY();
             
                if ((centerLastX + ErrorMarginX > centerSavedX && centerLastX - ErrorMarginX < centerSavedX)
                && (centerLastY + ErrorMarginY > centerSavedY  && centerLastY - ErrorMarginY < centerSavedY))
                {
                    savedDetection = (Detection)lastDetection;
                }
                else
                {
                    lastDetection = savedDetection;
                }
                
            }
            else
            {
                lastDetection = savedDetection;
            }
            float myX2 = savedDetection.x2;//Mathf.Floor(savedDetection.x2 * _roundingValue) / _roundingValue;
            float myX1 = savedDetection.x1; //Mathf.Floor(savedDetection.x1 * _roundingValue) / _roundingValue;
            float myY2 = savedDetection.y2;// Mathf.Floor(savedDetection.y2 * _roundingValue) / _roundingValue;
            float myY1 = savedDetection.y1;// Mathf.Floor(savedDetection.y1 * _roundingValue) / _roundingValue;
            Vector2 scale = new Vector2(myX2 - myX1,
                myY2 - myY1);

            Graphics.Blit(webCamTexture, renderTexture, scale, new Vector2(myX1, 1 - myY2));
        }
        else
        {
            faceDetected = false;
        }
        
        webCamTexture = null;
        renderTexture = null;
    }

    private void LateUpdate()
    {
        if (!isCameraSetup) return;
        if (_webcam1 is not null && _webcam1.didUpdateThisFrame)
        {
            FaceDetectorDetectFace(_webcam1, _face1Texture, ref _face1Detected, ref _lastFace1Detection);
        }

        if (_webcam2 is not null && _webcam2.didUpdateThisFrame)
        {
            FaceDetectorDetectFace(_webcam2, _face2Texture, ref _face2Detected, ref _lastFace2Detection);

        }
        
    }

    public bool DoesCamera1DetectFace()
    {
        return _face1Detected;
    }
    public bool DoesCamera2DetectFace()
    {
        return _face2Detected;
    }
    
}
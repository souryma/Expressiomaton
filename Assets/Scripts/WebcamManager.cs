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
    // [SerializeField] private TMP_Dropdown _camera2Choice;
    
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

    public bool Face1Detected => _face1Detected;
    public bool Face2Detected => _face2Detected;

    public Detection? LastFace1Detection => _lastFace1Detection;
    public Detection? LastFace2Detection => _lastFace2Detection;


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
            // if (dropdown.CompareTag("Player2"))
            // {
            //     
            //     _camera2Choice = dropdown;
            // }
            // else
            // {
                _camera1Choice = dropdown;
            // }
        }

        _camera1Choice.options = _camerasNameList;
        // _camera2Choice.options = _camerasNameList;
    }

    private void FaceDetectorInitializer()
    {
        _detector = new FaceDetector(_resources);
    }


    // Will select the camera names that are defined in the dropdowns
    public void ConfirmCameraSelection()
    {
        _webcam1 = new WebCamTexture(_camerasNameList[_camera1Choice.value].text);
        // _webcam2 = new WebCamTexture(_camerasNameList[_camera2Choice.value].text);
        
        _face1Texture = new RenderTexture(_cameraTextureResolutions.x, _cameraTextureResolutions.y, 0);
        _face2Texture = new RenderTexture(_cameraTextureResolutions.x, _cameraTextureResolutions.y, 0);
        
        _webcam1.Play();
        // _webcam2.Play();

        isCameraSetup = true;
    }

    private void OnDestroy()
    {
        if (ScenesManager.isSceneManagerLoaded)
            ScenesManager.instance.OnStartSceneLoaded -= SetupCameras;
        
        if (_webcam1 != null) Destroy(_webcam1);
        
        if (_face1Texture != null) Destroy(_face1Texture);
        if (_face2Texture != null) Destroy(_face2Texture);
        
        DestroyFaceDetector();
    }

    private void DestroyFaceDetector()
    {
        _detector?.Dispose();
    }
    
    private void FaceDetectorDetectFace()
    {
        _detector.ProcessImage(_webcam1, _threshold);

        _face1Detected = false;
        _face2Detected = false;
        if (!_detector.Detections.Any())
        {
            _lastFace1Detection = null;
            _lastFace2Detection = null;
         
            return;
        }
        
       
     
        var centerP1 = new Vector2(0.4f, 0.5f);
        var centerP2 = new Vector2(0.6f, 0.5f);
        
        Detection? currentDetectionP1 = null;
        Detection? currentDetectionP2 = null;
        
        float smallestDistanceP1 = 1f;
        float smallestDistanceP2 = 1f;

        foreach (var detection in _detector.Detections)
        {
            
            Vector2 centerCurrentDetection = new Vector2(detection.GetCenterX(), detection.GetCenterY());
            if (centerCurrentDetection.x > 0.5f)
            {
                float distance = Vector2.Distance(centerP1, centerCurrentDetection);
                if (distance < smallestDistanceP1)
                {
                    currentDetectionP1 = detection;
                }
            }
            else
            {
                float distance = Vector2.Distance(centerP2, centerCurrentDetection);
                if (distance < smallestDistanceP2)
                {
                    currentDetectionP2 = detection;
                }
            }
        }

        if (_lastFace1Detection != null && currentDetectionP1 != null)
        {
            var centerSavedX = currentDetectionP1.Value.GetCenterX();
            var centerSavedY = currentDetectionP1.Value.GetCenterY();
            var centerLastX = _lastFace1Detection?.GetCenterX();
            var centerLastY = _lastFace1Detection?.GetCenterY();
             
            if ((centerLastX + ErrorMarginX > centerSavedX && centerLastX - ErrorMarginX < centerSavedX)
                && (centerLastY + ErrorMarginY > centerSavedY  && centerLastY - ErrorMarginY < centerSavedY))
            {
                currentDetectionP1 = _lastFace1Detection;
            }
            else
            {
                _lastFace1Detection = currentDetectionP1;
            }
                
        }
        else
        {
            _lastFace1Detection = currentDetectionP1;
        }
        
        if (_lastFace2Detection != null && currentDetectionP2 != null)
        {
            var centerSavedX = currentDetectionP2.Value.GetCenterX();
            var centerSavedY = currentDetectionP2.Value.GetCenterY();
            var centerLastX = _lastFace2Detection?.GetCenterX();
            var centerLastY = _lastFace2Detection?.GetCenterY();
             
            if ((centerLastX + ErrorMarginX > centerSavedX && centerLastX - ErrorMarginX < centerSavedX)
                && (centerLastY + ErrorMarginY > centerSavedY  && centerLastY - ErrorMarginY < centerSavedY))
            {
                currentDetectionP2 = _lastFace2Detection;
            }
            else
            {
                _lastFace2Detection = currentDetectionP2;
            }
                
        }
        else
        {
            _lastFace2Detection = currentDetectionP2;
        }
        
        
        
        if (currentDetectionP2 != null)
        {
            float myX22 = currentDetectionP2.Value.x2; //Mathf.Floor(savedDetection.x2 * _roundingValue) / _roundingValue;
            float myX12 = currentDetectionP2.Value.x1; //Mathf.Floor(savedDetection.x1 * _roundingValue) / _roundingValue;
            float myY22 = currentDetectionP2.Value.y2; // Mathf.Floor(savedDetection.y2 * _roundingValue) / _roundingValue;
            float myY12 = currentDetectionP2.Value.y1; // Mathf.Floor(savedDetection.y1 * _roundingValue) / _roundingValue;
            Vector2 scale2 = new Vector2(myX22 - myX12,
                myY22 - myY12);

            Graphics.Blit(_webcam1, _face2Texture, scale2, new Vector2(myX12, 1 - myY22));
            _face2Detected = true;
        }
        else
        {
            _face2Detected = false;
        }


        if (currentDetectionP1 != null)
        {
            float myX2 = currentDetectionP1.Value.x2;
            float myX1 = currentDetectionP1.Value.x1; 
            float myY2 = currentDetectionP1.Value.y2; 
            float myY1 = currentDetectionP1.Value.y1; 
            Vector2 scale = new Vector2(myX2 - myX1,
                myY2 - myY1);
            Graphics.Blit(_webcam1, _face1Texture, scale, new Vector2(myX1, 1 - myY2));
            _face1Detected = true;
        }
        else
        {
            _face1Detected = true;
        }
      
    }

    private void LateUpdate()
    {
        if (!isCameraSetup) return;
        if (_webcam1 is not null && _webcam1.didUpdateThisFrame)
        {
            FaceDetectorDetectFace();

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
using System;
using System.Linq;
using UnityEngine;
using UI = UnityEngine.UI;
// using Klak.TestTools;

namespace UltraFace {

public sealed class Visualizer : MonoBehaviour
{
    #region Editable attributes

    // [SerializeField] ImageSource _source = null;
    [SerializeField, Range(0, 1)] float _threshold = 0.5f;
    [SerializeField] ResourceSet _resources = null;
    [SerializeField] Shader _visualizer = null;
    [SerializeField] Texture2D _texture = null;
    [SerializeField] UI.RawImage _previewUI = null;
    [SerializeField] RenderTexture renderTexture = null;

    #endregion

    #region Private objects

    FaceDetector _detector;
    Material _material;
    ComputeBuffer _drawArgs;
    private WebCamTexture _webCamTexture;
    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        Debug.Log(WebCamTexture.devices[0].name);
        _webCamTexture = new WebCamTexture(WebCamTexture.devices[0].name);
        _webCamTexture.Play();
        _detector = new FaceDetector(_resources);
        _material = new Material(_visualizer);
        _drawArgs = new ComputeBuffer(4, sizeof(uint),
                                      ComputeBufferType.IndirectArguments);
        _drawArgs.SetData(new [] {6, 0, 0, 0});
    }

    void OnDestroy()
    {
        _detector?.Dispose();
        Destroy(_material);
        _drawArgs?.Dispose();
    }

    void Update()
    {
        _detector.ProcessImage(_webCamTexture, _threshold);
        // _detector.ProcessImage(_source.Texture, _threshold);
        _previewUI.texture = _webCamTexture;
   
        // Face detection
      

        // Marker update
        if (_detector.Detections.Any())
        {
            Vector2 imageCenter = new Vector2(0.5f, 0.5f);
            var savedDetection = _detector.Detections.First();
            Vector2 vec2 = new Vector2(savedDetection.x1 + (savedDetection.x2 - savedDetection.x1) / 2,
                savedDetection.y1 + (savedDetection.y2 - savedDetection.y1) / 2);
            float smallestDistance = Vector2.Distance(imageCenter, vec2);
            foreach (var detection in _detector.Detections)
            {
                if(detection.Equals(savedDetection)) continue;
                Vector2 center = new Vector2(detection.x1 + (detection.x2 - detection.x1) / 2,
                    detection.y1 + (detection.y2 - detection.y1) / 2);
                float distance = Vector2.Distance(imageCenter, center);
                if (distance < smallestDistance)
                {
                    smallestDistance = distance;
                    savedDetection = detection;
                }
            }
            // var detector = _detector.Detections[savedIndex];
            // // Debug.Log("extent " + detector.extent);
            // // Debug.Log("center " +detector.center);
            Vector2 scale = new Vector2(savedDetection.x2 - savedDetection.x1,
                savedDetection.y2 - savedDetection.y1);
            // Debug.Log(savedDetection.x1  +" " + savedDetection.x2 + "/" + savedDetection.y1 + " " 
            //           + savedDetection.y2 +"/" + (savedDetection.x2 - savedDetection.x1) + " "
            //           + (savedDetection.y2 - savedDetection.y1));
   
            Graphics.Blit(_webCamTexture, renderTexture, scale, new Vector2(savedDetection.x1, 1-savedDetection.y2) );
          
        }
        
    }

    // void OnRenderObject()
    // {
    //     _detector.SetIndirectDrawCount(_drawArgs);
    //     _material.SetFloat("_Threshold", _threshold);
    //     _material.SetTexture("_Texture", _texture);
    //     _material.SetBuffer("_Detections", _detector.DetectionBuffer);
    //     _material.SetPass(_texture == null ? 0 : 1);
    //     Graphics.DrawProceduralIndirectNow(MeshTopology.Triangles, _drawArgs, 0);
    // }

    #endregion
}

} // namespace UltraFace

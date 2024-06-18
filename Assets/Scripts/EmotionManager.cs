using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;

public class EmotionManager : MonoBehaviour
{
    public static EmotionManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public enum EMOTION
    {
        Neutral = 0,
        Happy = 1,
        Surprise = 2,
        Sadness = 3,
        Anger = 4,
    }

    [SerializeField] private NNModel _model = null;
    [SerializeField] private ComputeShader _preprocessor = null;

    [Header("Neutral Score")] 
    [SerializeField] 
    public float neutralScoreMax = 0.9f; 
    
    [SerializeField] 
    public float neutralScoreMin = 0.5f; 

    
    private ComputeBuffer _preprocessed;
    private IWorker _worker;

    private const int ImageSize = 64;

    private float neutralScoreP1 = 1f;
    private float neutralScoreP2 = 1f;
    

    public float NeutralScoreP1 => neutralScoreP1;
    public float NeutralScoreP2 => neutralScoreP2;
    
    void Start()
    {
        _preprocessed = new ComputeBuffer(ImageSize * ImageSize, sizeof(float));
        _worker = ModelLoader.Load(_model).CreateWorker();
    }

    void OnDisable()
    {
        _preprocessed?.Dispose();
        _preprocessed = null;

        _worker?.Dispose();
        _worker = null;
    }

    public EMOTION GetPlayer1Emotion()
    {
        return GetPlayerEmotion(WebcamManager.instance.Face1Texture, ref neutralScoreP1);
    }

    public EMOTION GetPlayer2Emotion()
    {
        return GetPlayerEmotion(WebcamManager.instance.Face2Texture, ref neutralScoreP2);
    }

    private EMOTION GetPlayerEmotion(RenderTexture faceTexture, ref float neutralScoreVariable)
    {
        if (!CheckCameras()) return EMOTION.Neutral;

        // Preprocessing
        _preprocessor.SetTexture(0, "_Texture", faceTexture);
        _preprocessor.SetBuffer(0, "_Tensor", _preprocessed);
        _preprocessor.Dispatch(0, ImageSize / 32, ImageSize / 32, 1);

        // Emotion recognition model
        using (var tensor = new Tensor(1, ImageSize, ImageSize, 1, _preprocessed))
            _worker.Execute(tensor);

        // Output aggregation
        var probs = _worker.PeekOutput().AsFloats().Select(x => Mathf.Exp(x));

        return GetMaxEmotion(probs.ToList(), ref neutralScoreVariable);
    }

    private bool CheckCameras()
    {
        // bool ret = true;
        //
        // // If the cameras are not ready, return neutral by default.
        // if (!WebcamManager.instance.isCameraSetup)
        // {
        //     // Debug.LogWarning("Warning : the cameras are not initialized, returning neutral expression by default.");
        //     ret = false;
        // }

        return WebcamManager.instance.isCameraSetup;
    }

    private EMOTION GetMaxEmotion(List<float> probs, ref float neutralScore)
    {
        var sum = probs.Sum();

        int emotionNumber = 0;
        float maxEmotionValue = 0;
        neutralScore = 0f;
        // Return neutral if neutral is more than 50%
        float localScore = probs[0] / sum ;
        if (localScore>= 0.5)
        {
            neutralScore = localScore;
            return (EMOTION) 0;
        }
   
        for (int i = 0; i < probs.Count; i++)
        {
            float newEmotion = probs[i] / sum;
            maxEmotionValue = Math.Max(maxEmotionValue, newEmotion);
            if (Math.Abs(maxEmotionValue - newEmotion) < 0.001f)
            {
                emotionNumber = i;
                neutralScore = i == 0 ? maxEmotionValue : 0f;
            }
                
            
        }

        return (EMOTION) emotionNumber;
    }

    public static string GetEmotionString(EMOTION emotion)
    {
        string emotionText = "";
        switch (emotion)
        {
            case EmotionManager.EMOTION.Anger:
                emotionText = "Anger";
                break;
            case EmotionManager.EMOTION.Happy:
                emotionText = "Happy";
                break;
            case EmotionManager.EMOTION.Neutral:
                emotionText = "Neutral";
                break;
            case EmotionManager.EMOTION.Sadness:
                emotionText = "Sadness";
                break;
            case EmotionManager.EMOTION.Surprise:
                emotionText = "Surprise";
                break;
        }

        return emotionText;
    }
}
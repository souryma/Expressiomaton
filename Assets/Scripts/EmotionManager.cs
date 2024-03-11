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
        //Disgust = 5,
        //Fear = 6,
        //Contempt = 7
    }

    [SerializeField] private NNModel _model = null;
    [SerializeField] private ComputeShader _preprocessor = null;

    private ComputeBuffer _preprocessed;
    private IWorker _worker;

    private const int ImageSize = 64;

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
        if (!CheckCameras()) return EMOTION.Neutral;

        // Preprocessing
        _preprocessor.SetTexture(0, "_Texture", WebcamManager.instance.Webcam1Texture);
        _preprocessor.SetBuffer(0, "_Tensor", _preprocessed);
        _preprocessor.Dispatch(0, ImageSize / 32, ImageSize / 32, 1);

        // Emotion recognition model
        using (var tensor = new Tensor(1, ImageSize, ImageSize, 1, _preprocessed))
            _worker.Execute(tensor);

        // Output aggregation
        var probs = _worker.PeekOutput().AsFloats().Select(x => Mathf.Exp(x));

        return GetMaxEmotion(probs.ToList());
    }

    public EMOTION GetPlayer2Emotion()
    {
        if (!CheckCameras()) return EMOTION.Neutral;

        // Preprocessing
        _preprocessor.SetTexture(0, "_Texture", WebcamManager.instance.Webcam2Texture);
        _preprocessor.SetBuffer(0, "_Tensor", _preprocessed);
        _preprocessor.Dispatch(0, ImageSize / 32, ImageSize / 32, 1);

        // Emotion recognition model
        using (var tensor = new Tensor(1, ImageSize, ImageSize, 1, _preprocessed))
            _worker.Execute(tensor);

        // Output aggregation
        var probs = _worker.PeekOutput().AsFloats().Select(x => Mathf.Exp(x));

        return GetMaxEmotion(probs.ToList());
    }

    private bool CheckCameras()
    {
        bool ret = true;

        // If the cameras are not ready, return neutral by default.
        if (!WebcamManager.instance.isCameraSetup)
        {
            Debug.LogWarning("Warning : the cameras are not initialized, returning neutral expression by default.");
            ret = false;
        }

        return ret;
    }

    private EMOTION GetMaxEmotion(List<float> probs)
    {
        var sum = probs.Sum();

        int emotionNumber = 0;
        float maxEmotionValue = 0;

        for (int i = 0; i < probs.Count; i++)
        {
            // Return neutral if its more than 50%
            if (probs[0] / sum >= 0.5)
            {
                emotionNumber = 0;
            }
            else
            {
                float newEmotion = probs[i] / sum;
                maxEmotionValue = Math.Max(maxEmotionValue, newEmotion);
                if (Math.Abs(maxEmotionValue - newEmotion) < 0.001f)
                    emotionNumber = i;
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
            // case EmotionManager.EMOTION.Contempt:
            //     emotionText = "Contempt";
            //     break;
            // case EmotionManager.EMOTION.Disgust:
            //     emotionText = "Disgust";
            //     break;
            // case EmotionManager.EMOTION.Fear:
            //     emotionText = "Fear";
            //     break;
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
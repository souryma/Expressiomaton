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
        Disgust = 5,
        Fear = 6,
        Contempt = 7
    }

    [SerializeField] private NNModel _model = null;
    [SerializeField] private ComputeShader _preprocessor = null;

    private ComputeBuffer _preprocessed;
    private IWorker _worker;

    private const int ImageSize = 64;

    // Create a function that sends the emotion of a player

    // Start is called before the first frame update
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
    
    static readonly string[] Labels =
    {
        "Neutral", "Happiness", "Surprise", "Sadness",
        "Anger", "Disgust", "Fear", "Contempt"
    };

    public EMOTION GetPlayer1Emotion()
    {
        // Preprocessing
        _preprocessor.SetTexture(0, "_Texture", WebcamManager.instance.Webcam1Texture);
        _preprocessor.SetBuffer(0, "_Tensor", _preprocessed);
        _preprocessor.Dispatch(0, ImageSize / 32, ImageSize / 32, 1);

        // Emotion recognition model
        using (var tensor = new Tensor(1, ImageSize, ImageSize, 1, _preprocessed))
            _worker.Execute(tensor);

        // Output aggregation
        var probs = _worker.PeekOutput().AsFloats().Select(x => Mathf.Exp(x));
        
        return GetMaxEmotion(probs);
    }
    
    public EMOTION GetPlayer2Emotion()
    {
        // Preprocessing
        _preprocessor.SetTexture(0, "_Texture", WebcamManager.instance.Webcam2Texture);
        _preprocessor.SetBuffer(0, "_Tensor", _preprocessed);
        _preprocessor.Dispatch(0, ImageSize / 32, ImageSize / 32, 1);

        // Emotion recognition model
        using (var tensor = new Tensor(1, ImageSize, ImageSize, 1, _preprocessed))
            _worker.Execute(tensor);

        // Output aggregation
        var probs = _worker.PeekOutput().AsFloats().Select(x => Mathf.Exp(x));
        
        return GetMaxEmotion(probs);
    }

    private EMOTION GetMaxEmotion(IEnumerable<float> probs)
    {
        var sum = probs.Sum();

        var emotionValues = Labels.Zip(probs, (l, p) => $"{p / sum:0.00}");

        int emotionNb = 0;
        float maxValue = 0;
        EMOTION maxEmotion = 0;
        // Ignore first value (neutral)
        foreach (var emotion in emotionValues)
        {
            float emotionValue = float.Parse(emotion);
            if (emotionNb != 0)
            {
                if (emotionValue > maxValue)
                {
                    maxValue = emotionValue;
                    maxEmotion = (EMOTION) emotionNb;
                }
            }

            emotionNb++;
        }

        return maxEmotion;
    }

    // void Update()
    // {
    //     
    //     var enumerable = probs.ToList();
    //
    //     Debug.Log("Happy : " + enumerable[1]);
    // }

    public static string GetEmotionString(EMOTION emotion)
    {
        string emotionText = "";
        switch (emotion)
        {
            case EmotionManager.EMOTION.Anger:
                emotionText = "Anger";
                break;
            case EmotionManager.EMOTION.Contempt:
                emotionText = "Contempt";
                break;
            case EmotionManager.EMOTION.Disgust:
                emotionText = "Disgust";
                break;
            case EmotionManager.EMOTION.Fear:
                emotionText = "Fear";
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
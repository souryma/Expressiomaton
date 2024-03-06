using Unity.Barracuda;
using UnityEngine;

public class EmotionManager : MonoBehaviour
{
    [SerializeField] private WebCamTexture _webcamManager;
    
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

    private RenderTexture _webcamBuffer;
    private ComputeBuffer _preprocessed;
    private IWorker _worker;

    private const int ImageSize = 64;
    
    // Create a function that sends the emotion of a player

    // Start is called before the first frame update
    void Start()
    {
        _preprocessed = new ComputeBuffer(ImageSize * ImageSize, sizeof(float));
    }

    public EMOTION GetPlayer1Emotion()
    {
        return EMOTION.Happy;
    }
    
    public EMOTION GetPlayer2Emotion()
    {
        return EMOTION.Sadness;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
using UnityEngine;
using Unity.Barracuda;
using System.Linq;
using Debug = UnityEngine.Debug;

namespace EmotionFerPlus
{
    sealed class Test : MonoBehaviour
    {
        #region Editable attributes

        [SerializeField] NNModel _model = null;
        [SerializeField] UnityEngine.UI.RawImage _preview = null;
        [SerializeField] UnityEngine.UI.Text _label = null;
        [SerializeField] UnityEngine.UI.Text _emotionLabel = null;
        [SerializeField] ComputeShader _preprocessor = null;

        #endregion

        #region Internal objects

        WebCamTexture _webcamRaw;
        RenderTexture _webcamBuffer;
        ComputeBuffer _preprocessed;
        IWorker _worker;

        const int ImageSize = 64;

        static readonly string[] Labels =
        {
            "Neutral", "Happiness", "Surprise", "Sadness",
            "Anger", "Disgust", "Fear", "Contempt"
        };

        #endregion

        #region MonoBehaviour implementation

        void Start()
        {
            _webcamRaw = new WebCamTexture("Integrated Camera");
            _webcamBuffer = new RenderTexture(512, 512, 0);
            _preprocessed = new ComputeBuffer(ImageSize * ImageSize, sizeof(float));
            _worker = ModelLoader.Load(_model).CreateWorker();

            _webcamRaw.Play();
            _preview.texture = _webcamBuffer;

            foreach (var device in WebCamTexture.devices)
            {
                Debug.Log(device.name);
            }
        }

        void OnDisable()
        {
            _preprocessed?.Dispose();
            _preprocessed = null;

            _worker?.Dispose();
            _worker = null;
        }

        void OnDestroy()
        {
            if (_webcamRaw != null) Destroy(_webcamRaw);
            if (_webcamBuffer != null) Destroy(_webcamBuffer);
        }

        void Update()
        {
            // Cropping
            var scale = new Vector2((float) _webcamRaw.height / _webcamRaw.width, 1);
            var offset = new Vector2(scale.x / 2, 0);
            Graphics.Blit(_webcamRaw, _webcamBuffer, scale, offset);

            // Preprocessing
            _preprocessor.SetTexture(0, "_Texture", _webcamBuffer);
            _preprocessor.SetBuffer(0, "_Tensor", _preprocessed);
            _preprocessor.Dispatch(0, ImageSize / 8, ImageSize / 8, 1);

            // Emotion recognition model
            using (var tensor = new Tensor(1, ImageSize, ImageSize, 1, _preprocessed))
                _worker.Execute(tensor);

            // Output aggregation
            var probs = _worker.PeekOutput().AsFloats().Select(x => Mathf.Exp(x));
            var sum = probs.Sum();
            var lines = Labels.Zip(probs, (l, p) => $"{l,-12}: {p / sum:0.00}");
            _label.text = string.Join("\n", lines);

            var emotionValues = Labels.Zip(probs, (l, p) => $"{p / sum:0.00}");

            int emotionNb = 0;
            float maxValue = 0;
            EmotionManager.EMOTION maxEmotion = 0;
            // Ignore first value (neutral)
            foreach (var emotion in emotionValues)
            {
                float emotionValue = float.Parse(emotion);
                if (emotionNb != 0)
                {
                    if (emotionValue > maxValue)
                    {
                        maxValue = emotionValue;
                        maxEmotion = (EmotionManager.EMOTION) emotionNb;
                    }
                }

                emotionNb++;
            }

            switch (maxEmotion)
            {
                case EmotionManager.EMOTION.Anger:
                    _emotionLabel.text = "Anger";
                    break;
                case EmotionManager.EMOTION.Contempt:
                    _emotionLabel.text = "Contempt";
                    break;
                case EmotionManager.EMOTION.Disgust:
                    _emotionLabel.text = "Disgust";
                    break;
                case EmotionManager.EMOTION.Fear:
                    _emotionLabel.text = "Fear";
                    break;
                case EmotionManager.EMOTION.Happy:
                    _emotionLabel.text = "Happy";
                    break;
                case EmotionManager.EMOTION.Neutral:
                    _emotionLabel.text = "Neutral";
                    break;
                case EmotionManager.EMOTION.Sadness:
                    _emotionLabel.text = "Sadness";
                    break;
                case EmotionManager.EMOTION.Surprise:
                    _emotionLabel.text = "Surprise";
                    break;
            }
        }

        #endregion
    }
} // namespace EmotionFerPlus
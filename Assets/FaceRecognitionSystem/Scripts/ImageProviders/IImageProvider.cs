using UnityEngine;
using UnityEngine.Events;

namespace FaceRecognitionSystem {
    [System.Serializable]
    public class ImageProviderReadyEvent : UnityEvent<IImageProvider> { }
    public interface IImageProvider {
        public Color32 [ ] ImgData { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}

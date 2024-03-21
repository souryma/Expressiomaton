using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "EmotionData", menuName = "EmotionData", order = 0)]
    public class EmotionData : ScriptableObject
    {
        [SerializeField] public EmotionManager.EMOTION FileEmotion;
        [SerializeField] public Sprite ImageEmotion;
        [SerializeField] public string TextEmotion;

    }
}
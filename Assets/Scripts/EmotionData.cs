using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "EmotionData", menuName = "EmotionData", order = 0)]
    public class EmotionData : ScriptableObject
    {
        [FormerlySerializedAs("FileEmotion")] [SerializeField] public EmotionManager.EMOTION TypeEmotion;
        [SerializeField] public Sprite ImageEmotion;
        [SerializeField] public LocalizedString TextEmotion;

    }
}
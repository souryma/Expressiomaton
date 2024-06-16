using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Emotion = EmotionManager.EMOTION;

public class EmotionDataset : MonoBehaviour
{
    [SerializeField]
    private EmotionData[] emotionsData;

    public Dictionary<Emotion, EmotionData> data = new Dictionary<Emotion, EmotionData>();

    public static EmotionDataset Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        foreach (EmotionData emotionData in emotionsData)
            data.Add(emotionData.TypeEmotion, emotionData);
    }
}
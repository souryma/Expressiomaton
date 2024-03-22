using DefaultNamespace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Emotion = EmotionManager.EMOTION;

public class EmotionsData : MonoBehaviour
{
    [SerializeField]
    private EmotionData[] emotionsData;

    public Dictionary<Emotion, EmotionData> data;

    public static EmotionsData Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        foreach (EmotionData emotionData in emotionsData)
            data.Add(emotionData.FileEmotion, emotionData);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Emotion = EmotionManager.EMOTION;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private float maxEmotionTestDuration = 10.0f;

    private List<Emotion> emotionsToTest = new List<Emotion>();

    int currentEmotionIdx = 0;
    float startTestTime = 0;
    float currentTestTime = 0;
    float endTestTime = 0;

    bool bPrintedCurrentEmotion = false;
    bool bOnceFlag = true;


    void Start()
    {
        for (int i = 0; i < System.Enum.GetNames(typeof(Emotion)).Length; i++)
            emotionsToTest.Add((Emotion)i);

        currentEmotionIdx = 0;
        startTestTime = Time.time;
        endTestTime = startTestTime + maxEmotionTestDuration;
    }

    void Update()
    {
        _ProcessEmotionTestForPlayer(1);
        _ProcessEmotionTestForPlayer(2);
    }

    private void _ProcessEmotionTestForPlayer(int playerID)
    {
        currentTestTime = Time.time;
        int remainingTime = (int)Mathf.Round(endTestTime - currentTestTime);

        // Test emotion
        if (currentEmotionIdx < emotionsToTest.Count && remainingTime > 0)
        {
            Emotion currentEmotionToTest = emotionsToTest[currentEmotionIdx];

            if (!bPrintedCurrentEmotion) Debug.Log($"Player {playerID} current emotion : {currentEmotionToTest.ToString()}");

            Debug.Log($"Remaining time : {remainingTime}s");

            if (_GetEmotionOfPlayer(playerID) == currentEmotionToTest)
            {
                // Set new emotion
                currentEmotionIdx++;

                // Set new start time for the new emotion
                startTestTime = Time.time;

                // Set new end time
                endTestTime = startTestTime + maxEmotionTestDuration;
            }

            bOnceFlag = true;
        }
        else if (currentEmotionIdx < emotionsToTest.Count)
        {
            if (bOnceFlag)
            {
                Debug.Log($"Fail to recognize player {playerID} emotion");
                bOnceFlag = false;
            }
        }
        else if (remainingTime > 0)
        {
            if (bOnceFlag)
            {
                Debug.Log($"Finish to recognize player {playerID} emotions");
                bOnceFlag = false;
            }
        }
    }

    private Emotion _GetEmotionOfPlayer(int playerID)
    {
        return playerID == 1 ? EmotionManager.instance.GetPlayer1Emotion() : EmotionManager.instance.GetPlayer2Emotion();
    }
}

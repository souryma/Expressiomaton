using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Emotion = EmotionManager.EMOTION;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private float maxEmotionTestDuration = 10.0f;

    private List<Emotion> emotionsToTest = new List<Emotion>();

    int currentEmotionIdx = 0;
    float startTestTime = 0;
    float currentTestTime = 0;
    float endTestTime = 0;

    bool bPrintedCurrentEmotion = false;
    bool bOnceFlag = true;

    [SerializeField] private TextMeshProUGUI _player1Text;
    [SerializeField] private TextMeshProUGUI _player2Text;
    [SerializeField] private TextMeshProUGUI _player1FailText;
    [SerializeField] private TextMeshProUGUI _player2FailText;
    [SerializeField] private GameObject _returnToMenuButton1;
    [SerializeField] private GameObject _returnToMenuButton2;


    void Start()
    {
        for (int i = 0; i < System.Enum.GetNames(typeof(Emotion)).Length; i++)
            emotionsToTest.Add((Emotion) i);

        currentEmotionIdx = 0;
        startTestTime = Time.time;
        endTestTime = startTestTime + maxEmotionTestDuration;

        string txt = "Prepare your emotions !";
        _player1Text.text = txt;
        _player2Text.text = txt;

        _player1FailText.text = "";
        _player2FailText.text = "";
        _returnToMenuButton1.SetActive(false);
        _returnToMenuButton2.SetActive(false);
    }

    void Update()
    {
        _ProcessEmotionTestForPlayer(1);
        _ProcessEmotionTestForPlayer(2);
    }

    private void _ProcessEmotionTestForPlayer(int playerID)
    {
        currentTestTime = Time.time;
        int remainingTime = (int) Mathf.Round(endTestTime - currentTestTime);

        // Test emotion
        if (currentEmotionIdx < emotionsToTest.Count && remainingTime > 0)
        {
            Emotion currentEmotionToTest = emotionsToTest[currentEmotionIdx];

            if (!bPrintedCurrentEmotion)
            {
                var txt = $"Player {playerID} current emotion : {currentEmotionToTest.ToString()}";
                Debug.Log(txt);
                _player1Text.text = txt;
                _player2Text.text = txt;
            }

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
                var txt = $"Fail to recognize player {playerID} emotion";
                Debug.Log(txt);

                if (playerID == 1)
                {
                    _player1FailText.text = txt;
                    _returnToMenuButton1.SetActive(true);
                }
                else
                {
                    _player2FailText.text = txt;
                    _returnToMenuButton2.SetActive(true);
                }
                
                bOnceFlag = false;
            }
        }
        else if (remainingTime > 0)
        {
            if (bOnceFlag)
            {
                Debug.Log($"Finish to recognize player {playerID} emotions");
                bOnceFlag = false;
                
                if (playerID == 1)
                {
                    _player1FailText.text = "";
                    _returnToMenuButton1.SetActive(false);
                }
                else
                {
                    _player2FailText.text = "";
                    _returnToMenuButton2.SetActive(false);
                }
            }
        }
    }

    private Emotion _GetEmotionOfPlayer(int playerID)
    {
        return playerID == 1
            ? EmotionManager.instance.GetPlayer1Emotion()
            : EmotionManager.instance.GetPlayer2Emotion();
    }
}
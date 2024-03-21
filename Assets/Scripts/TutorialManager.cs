using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

    [SerializeField] private RawImage _player1Camera;
    [SerializeField] private RawImage _player2Camera;
    [SerializeField] private TextMeshProUGUI _player1Text;
    [SerializeField] private TextMeshProUGUI _player2Text;
    [SerializeField] private TextMeshProUGUI _player1FailText;
    [SerializeField] private TextMeshProUGUI _player2FailText;
    [SerializeField] private GameObject _returnToMenuButton1;
    [SerializeField] private GameObject _returnToMenuButton2;

    [SerializeField] private string _menuSceneName;
    [SerializeField] private string _nextSceneName;

    [SerializeField] private TextMeshProUGUI _launchText1;
    [SerializeField] private TextMeshProUGUI _launchText2;

    private bool _player1Ready = false;
    private bool _player2Ready = false;

    void Start()
    {
        for (int i = 0; i < System.Enum.GetNames(typeof(Emotion)).Length; i++)
            emotionsToTest.Add((Emotion) i);

        _player1Camera.texture = WebcamManager.instance.Webcam1;
        _player2Camera.texture = WebcamManager.instance.Webcam2;

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
        
        _launchText1.gameObject.SetActive(false);
        _launchText2.gameObject.SetActive(false);
    }

    void Update()
    {
        _ProcessEmotionTestForPlayer(1);
        _ProcessEmotionTestForPlayer(2);
    }

    public void BackToMenu()
    {
        ScenesManager.instance.LoadScene(_menuSceneName);
    }

    private void _ProcessEmotionTestForPlayer(int playerID)
    {
        if (_player1Ready && _player2Ready)
        {
            StartCoroutine(LaunchGame());
            return;
        }

        currentTestTime = Time.time;
        int remainingTime = (int) Mathf.Round(endTestTime - currentTestTime);

        // Test emotion
        if (currentEmotionIdx < emotionsToTest.Count && remainingTime > 0)
        {
            Emotion currentEmotionToTest = emotionsToTest[currentEmotionIdx];

            if (!bPrintedCurrentEmotion)
            {
                var txt = $"Do a {currentEmotionToTest.ToString()} face !";
                //Debug.Log(txt);
                if (playerID == 1)
                {
                    _player1Text.text = txt;
                }
                else
                {
                    _player2Text.text = txt;
                }
            }

            // Debug.Log($"Remaining time : {remainingTime}s");

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
                var txt = "Sadly, it seems like we aren't able to read your expression..";
                //Debug.Log(txt);

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

            Emotion currentEmotionToTest = emotionsToTest[currentEmotionIdx];
            if (_GetEmotionOfPlayer(playerID) == currentEmotionToTest)
            {
                // Set new emotion
                currentEmotionIdx++;

                // Set new start time for the new emotion
                startTestTime = Time.time;

                // Set new end time
                endTestTime = startTestTime + maxEmotionTestDuration;
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
                    _player1Ready = true;
                    _player1Text.text = "Waiting for player 2";
                }
                else
                {
                    _player2Ready = true;
                    _player2Text.text = "Waiting for player 1";
                }
            }
        }
    }

    private IEnumerator LaunchGame()
    {
        _launchText1.gameObject.SetActive(true);
        _launchText2.gameObject.SetActive(true);

        string text = "The game will start in 5";
        _launchText1.text = text;
        _launchText2.text = text;
        yield return new WaitForSecondsRealtime(1);

        text = "The game will start in 4";
        _launchText1.text = text;
        _launchText2.text = text;
        yield return new WaitForSecondsRealtime(1);

        text = "The game will start in 3";
        _launchText1.text = text;
        _launchText2.text = text;
        yield return new WaitForSecondsRealtime(1);

        text = "The game will start in 2";
        _launchText1.text = text;
        _launchText2.text = text;
        yield return new WaitForSecondsRealtime(1);

        text = "The game will start in 1";
        _launchText1.text = text;
        _launchText2.text = text;
        yield return new WaitForSecondsRealtime(1);

        ScenesManager.instance.LoadScene(_nextSceneName);
    }

    private Emotion _GetEmotionOfPlayer(int playerID)
    {
        return playerID == 1
            ? EmotionManager.instance.GetPlayer1Emotion()
            : EmotionManager.instance.GetPlayer2Emotion();
    }
}
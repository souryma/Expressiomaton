using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Emotion = EmotionManager.EMOTION;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private float maxEmotionTestDuration = 10.0f;

    // TODO : @WILL Replace emotionToTest by _emotionsToTestP1 and _emotionsToTestP2
    private List<Emotion> _emotionsToTestP1 = new List<Emotion>();
    private List<Emotion> _emotionsToTestP2 = new List<Emotion>();

    int _currentEmotionIdxP1 = 0;
    int _currentEmotionIdxP2 = 0;
    float _startTestTimeP1 = 0;
    float _startTestTimeP2 = 0;
    float _currentTestTimeP1 = 0;
    float _currentTestTimeP2 = 0;
    float _endTestTimeP1 = 0;
    float _endTestTimeP2 = 0;

    bool bPrintedCurrentEmotionP1 = false;
    bool bPrintedCurrentEmotionP2 = false;
    bool _bOnceFlagP1 = true;
    bool _bOnceFlagP2 = true;

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

    [SerializeField] private Texture2D angerSprite;
    [SerializeField] private Texture2D neutralSprite;
    [SerializeField] private Texture2D happySprite;
    [SerializeField] private Texture2D sadSprite;
    [SerializeField] private Texture2D surpriseSprite;

    [SerializeField] private RawImage spriteP1;
    [SerializeField] private RawImage spriteP2;

    private bool _player1Ready = false;
    private bool _player2Ready = false;

    void Start()
    {
        for (int i = 0; i < System.Enum.GetNames(typeof(Emotion)).Length; i++)
        {
            _emotionsToTestP1.Add((Emotion) i);
            _emotionsToTestP2.Add((Emotion) i);
        }

        _player1Camera.texture = WebcamManager.instance.Webcam1;
        _player2Camera.texture = WebcamManager.instance.Webcam2;

        _currentEmotionIdxP1 = 0;
        _currentEmotionIdxP2 = 0;
        _startTestTimeP1 = Time.time;
        _endTestTimeP1 = _startTestTimeP1 + maxEmotionTestDuration;

        _startTestTimeP2 = Time.time;
        _endTestTimeP2 = _startTestTimeP2 + maxEmotionTestDuration;

        string txt = "Prepare your emotions !";
        _player1Text.text = txt;
        _player2Text.text = txt;

        _player1FailText.text = "";
        _player2FailText.text = "";
        _returnToMenuButton1.SetActive(false);
        _returnToMenuButton2.SetActive(false);

        _launchText1.gameObject.SetActive(false);
        _launchText2.gameObject.SetActive(false);

        spriteP1.texture = neutralSprite;
        spriteP2.texture = neutralSprite;
    }

    private bool _gameLaunched = false;

    void Update()
    {
        _ProcessEmotionTestForPlayer1();
        _ProcessEmotionTestForPlayer2();

        if (_player1Ready && _player2Ready && _gameLaunched == false)
        {
            _gameLaunched = true;
            StartCoroutine(LaunchGame());
        }
    }

    public void BackToMenu()
    {
        ScenesManager.instance.LoadScene(_menuSceneName);
    }

    private Texture2D EmotionToTexture(Emotion emotion)
    {
        var ret = neutralSprite;
        switch (emotion)
        {
            case Emotion.Anger:
                ret = angerSprite;
                break;
            case Emotion.Happy:
                ret = happySprite;
                break;
            case Emotion.Neutral:
                ret = neutralSprite;
                break;
            case Emotion.Surprise:
                ret = surpriseSprite;
                break;
            case Emotion.Sadness:
                ret = sadSprite;
                break;
        }

        return ret;
    }

    private void _ProcessEmotionTestForPlayer1()
    {
        _currentTestTimeP1 = Time.time;
        int remainingTime = (int) Mathf.Round(_endTestTimeP1 - _currentTestTimeP1);

        // Test emotion
        if (_currentEmotionIdxP1 < _emotionsToTestP1.Count && remainingTime > 0)
        {
            Emotion currentEmotionToTest = _emotionsToTestP1[_currentEmotionIdxP1];

            if (!bPrintedCurrentEmotionP1)
            {
                var txt = $"Do a {currentEmotionToTest.ToString()} face !";
                _player1Text.text = txt;
                spriteP1.texture = EmotionToTexture(currentEmotionToTest);
            }

            if (_GetEmotionOfPlayer(1) == currentEmotionToTest)
            {
                // Set new emotion
                _currentEmotionIdxP1++;

                // Set new start time for the new emotion
                _startTestTimeP1 = Time.time;

                // Set new end time
                _endTestTimeP1 = _startTestTimeP1 + maxEmotionTestDuration;
            }

            _bOnceFlagP1 = true;
        }
        else if (_currentEmotionIdxP1 < _emotionsToTestP1.Count)
        {
            if (_bOnceFlagP1)
            {
                var txt = "Sadly, it seems like we aren't able to read your expression..";
                //Debug.Log(txt);

                _player1FailText.text = txt;
                _returnToMenuButton1.SetActive(true);

                _bOnceFlagP1 = false;
            }

            Emotion currentEmotionToTest = _emotionsToTestP1[_currentEmotionIdxP1];
            if (_GetEmotionOfPlayer(1) == currentEmotionToTest)
            {
                // Set new emotion
                _currentEmotionIdxP1++;

                // Set new start time for the new emotion
                _startTestTimeP1 = Time.time;

                // Set new end time
                _endTestTimeP1 = _startTestTimeP1 + maxEmotionTestDuration;
            }
        }
        else if (remainingTime > 0)
        {
            if (_bOnceFlagP1)
            {
                Debug.Log($"Finish to recognize player 1 emotions");
                _bOnceFlagP1 = false;

                _player1Ready = true;
                _player1Text.text = "Waiting for player 2";
                spriteP1.gameObject.SetActive(false);
            }
        }
    }

    private void _ProcessEmotionTestForPlayer2()
    {
        _currentTestTimeP2 = Time.time;
        int remainingTime = (int) Mathf.Round(_endTestTimeP2 - _currentTestTimeP2);

        // Test emotion
        if (_currentEmotionIdxP2 < _emotionsToTestP1.Count && remainingTime > 0)
        {
            Emotion currentEmotionToTest = _emotionsToTestP2[_currentEmotionIdxP2];

            if (!bPrintedCurrentEmotionP2)
            {
                var txt = $"Do a {currentEmotionToTest.ToString()} face !";
                _player2Text.text = txt;
                spriteP2.texture = EmotionToTexture(currentEmotionToTest);
            }

            if (_GetEmotionOfPlayer(2) == currentEmotionToTest)
            {
                // Set new emotion
                _currentEmotionIdxP2++;

                // Set new start time for the new emotion
                _startTestTimeP2 = Time.time;

                // Set new end time
                _endTestTimeP2 = _startTestTimeP2 + maxEmotionTestDuration;
            }

            _bOnceFlagP2 = true;
        }
        else if (_currentEmotionIdxP2 < _emotionsToTestP2.Count)
        {
            if (_bOnceFlagP2)
            {
                var txt = "Sadly, it seems like we aren't able to read your expression..";
                //Debug.Log(txt);

                _player2FailText.text = txt;
                _returnToMenuButton2.SetActive(true);

                _bOnceFlagP2 = false;
            }

            Emotion currentEmotionToTest = _emotionsToTestP2[_currentEmotionIdxP2];
            if (_GetEmotionOfPlayer(2) == currentEmotionToTest)
            {
                // Set new emotion
                _currentEmotionIdxP2++;

                // Set new start time for the new emotion
                _startTestTimeP2 = Time.time;

                // Set new end time
                _endTestTimeP2 = _startTestTimeP2 + maxEmotionTestDuration;
            }
        }
        else if (remainingTime > 0)
        {
            if (_bOnceFlagP2)
            {
                Debug.Log($"Finish to recognize player 1 emotions");
                _bOnceFlagP2 = false;

                _player2Ready = true;
                _player2Text.text = "Waiting for player 1";
                spriteP2.gameObject.SetActive(false);
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
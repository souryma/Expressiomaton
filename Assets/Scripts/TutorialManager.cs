using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Emotion = EmotionManager.EMOTION;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private float maxEmotionTestDuration = 10.0f;

    private List<EmotionData> _emotionsToTestP1 = new List<EmotionData>();
    private List<EmotionData> _emotionsToTestP2 = new List<EmotionData>();

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

    [SerializeField] private List<EmotionData> emotionsNeededForGame = new();
    // [SerializeField] private EmotionData angerSprite;
    // [SerializeField] private EmotionData neutralSprite;
    // [SerializeField] private EmotionData happySprite;
    // [SerializeField] private EmotionData sadSprite;
    // [SerializeField] private EmotionData surpriseSprite;

    [SerializeField] private RawImage spriteP1;
    [SerializeField] private RawImage spriteP2;


    [Header("Localization")] 
    [SerializeField] private LocalizedString prepareText;
    [SerializeField] private LocalizedString doEmotionText;
    [SerializeField] private LocalizedString emotionUnreadableText;
    [SerializeField] private LocalizedString emotionUnreadableP2Text;
    [SerializeField] private LocalizedString waitingP1;
    [SerializeField] private LocalizedString waitingP2;
    
    [Header("Countdown")]
    [SerializeField] private int countDownStart = 5;
    [SerializeField] private LocalizedString countDownText;
    private bool _player1Ready = false;
    private bool _player2Ready = false;

    void Start()
    {
        // for (int i = 0; i < System.Enum.GetNames(typeof(Emotion)).Length; i++)
        // {
        //     _emotionsToTestP1.Add((Emotion) i);
        //     _emotionsToTestP2.Add((Emotion) i);
        // }

        _emotionsToTestP1 = new List<EmotionData>(emotionsNeededForGame);
        _emotionsToTestP2 = new List<EmotionData>(emotionsNeededForGame);
        
        
        _player1Camera.texture = WebcamManager.instance.Webcam1;
        _player2Camera.texture = WebcamManager.instance.Webcam2;

        _currentEmotionIdxP1 = 0;
        _currentEmotionIdxP2 = 0;
        _startTestTimeP1 = Time.time;
        _endTestTimeP1 = _startTestTimeP1 + maxEmotionTestDuration;

        _startTestTimeP2 = Time.time;
        _endTestTimeP2 = _startTestTimeP2 + maxEmotionTestDuration;

        string txt = prepareText.GetLocalizedString();
        _player1Text.text = txt;
        _player2Text.text = txt;

        _player1FailText.text = "";
        _player2FailText.text = "";
        _returnToMenuButton1.SetActive(false);
        _returnToMenuButton2.SetActive(false);

        _launchText1.gameObject.SetActive(false);
        _launchText2.gameObject.SetActive(false);

        spriteP1.texture = emotionsNeededForGame.First().ImageEmotion.texture;
        spriteP2.texture = emotionsNeededForGame.First().ImageEmotion.texture;
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
    

    private void _ProcessEmotionTestForPlayer1()
    {
        _currentTestTimeP1 = Time.time;
        int remainingTime = (int) Mathf.Round(_endTestTimeP1 - _currentTestTimeP1);

        // Test emotion
        if (_currentEmotionIdxP1 < _emotionsToTestP1.Count && remainingTime > 0)
        {
            EmotionData currentEmotionToTest = _emotionsToTestP1[_currentEmotionIdxP1];

            if (!bPrintedCurrentEmotionP1)
            {
                if (doEmotionText["emotionName"] is StringVariable emotionName)
                {
                    emotionName.Value = currentEmotionToTest.TextEmotion.GetLocalizedString();
                    doEmotionText["emotionName"] = emotionName;
                }
               
                var txt = doEmotionText.GetLocalizedString();
                // var txt = $"Do a {currentEmotionToTest.ToString()} face !";
                _player1Text.text = txt;
                spriteP1.texture = currentEmotionToTest.ImageEmotion.texture;
            }

            if (_GetEmotionOfPlayer(1) == currentEmotionToTest.TypeEmotion)
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
                var txt = emotionUnreadableText.GetLocalizedString();
                //Debug.Log(txt);

                _player1FailText.text = txt;
                _returnToMenuButton1.SetActive(true);

                _bOnceFlagP1 = false;
            }

            EmotionData currentEmotionToTest = _emotionsToTestP1[_currentEmotionIdxP1];
            if (_GetEmotionOfPlayer(1) == currentEmotionToTest.TypeEmotion)
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
                _player1Text.text = waitingP2.GetLocalizedString();
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
            EmotionData currentEmotionToTest = _emotionsToTestP2[_currentEmotionIdxP2];

            if (!bPrintedCurrentEmotionP2)
            {
                if (doEmotionText["emotionName"] is StringVariable emotionName)
                {
                    emotionName.Value = currentEmotionToTest.TextEmotion.GetLocalizedString();
                    doEmotionText["emotionName"] = emotionName;
                }
                var txt = doEmotionText.GetLocalizedString();
                _player2Text.text = txt;
                spriteP2.texture = currentEmotionToTest.ImageEmotion.texture;
            }

            if (_GetEmotionOfPlayer(2) == currentEmotionToTest.TypeEmotion)
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
                // var txt = "Sadly, it seems like we aren't able to read your expression..";
                var txt = emotionUnreadableText.GetLocalizedString();

                //Debug.Log(txt);

                _player2FailText.text = txt;
                _player1FailText.text = emotionUnreadableP2Text.GetLocalizedString();
                _returnToMenuButton1.SetActive(true);
                // _returnToMenuButton2.SetActive(true);

                _bOnceFlagP2 = false;
            }

            EmotionData currentEmotionToTest = _emotionsToTestP2[_currentEmotionIdxP2];
            if (_GetEmotionOfPlayer(2) == currentEmotionToTest.TypeEmotion)
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
                _bOnceFlagP2 = false;

                _player2Ready = true;
                _player2Text.text = waitingP1.GetLocalizedString();
                spriteP2.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator LaunchGame()
    {
        _launchText1.gameObject.SetActive(true);
        _launchText2.gameObject.SetActive(true);
        for (int i = 0; i < countDownStart; i++)
        {
            _launchText1.text = _launchText2.text = countDownText.GetLocalizedString() + " " + i;
            yield return new WaitForSecondsRealtime(1);
        }

        ScenesManager.instance.LoadScene(_nextSceneName);
    }

    private Emotion _GetEmotionOfPlayer(int playerID)
    {
        return playerID == 1
            ? EmotionManager.instance.GetPlayer1Emotion()
            : EmotionManager.instance.GetPlayer2Emotion();
    }
}
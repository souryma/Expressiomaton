using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UltraFace;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Emotion = EmotionManager.EMOTION;

public class TutorialManager : MonoBehaviour
{
    private int _currentEmotionIdxP1 = 0;
    private int _currentEmotionIdxP2 = 0;
    private float _startTestTimeP1 = 0;
    private float _startTestTimeP2 = 0;
    private float _currentTestTimeP1 = 0;
    private float _currentTestTimeP2 = 0;
    private float _endTestTimeP1 = 0;
    private float _endTestTimeP2 = 0;
    private float _screenEnd = 0;


    [SerializeField] private string _menuSceneName;
    [SerializeField] private string _nextSceneName;
    
    [SerializeField] private float maxEmotionTestDuration = 10.0f;
    [SerializeField] private int countDownStart = 5;
    [SerializeField] private int timeBeforeScreenStart = 3;
    [SerializeField] private List<EmotionData> emotionsNeededForGame = new();

    [SerializeField]
    private RawImage _camera;
    [SerializeField] private TextMeshProUGUI _player1NeutralText;
    [SerializeField] private GameObject _returnToMenuButton1;
    [SerializeField] private GameObject _skipButton;
    [SerializeField] private TextMeshProUGUI _launchText1;

    [Header("Player 1")] 
    [SerializeField] private GameObject _player1Name;
    [SerializeField] private TextMeshProUGUI _player1Text;
    [SerializeField] private Image spriteP1;
    [SerializeField] private RectTransform _player1Target;

    [Header("Player 2")]
    [SerializeField] private GameObject _player2Name;
    [SerializeField] private TextMeshProUGUI _player2Text;
    [SerializeField] private Image spriteP2;
    [SerializeField] private RectTransform _player2Target;
    
    [Header("Localization")] 
    [SerializeField] private LocalizedString prepareText;
    [SerializeField] private LocalizedString doEmotionText;
    [SerializeField] private LocalizedString emotionUnreadableText;
    [SerializeField] private LocalizedString emotionUnreadableP2Text;
    [SerializeField] private LocalizedString waitingP1;
    [SerializeField] private LocalizedString waitingP2;
    [SerializeField] private LocalizedString countDownText;
    
    private bool _player1Ready = false;
    private bool _player2Ready = false;
    private bool _gameLaunched = false;
    
    private void Start()
    {
        _screenEnd = Time.time + timeBeforeScreenStart;
        // _emotionsToTestP1 = new List<EmotionData>(emotionsNeededForGame);
        // _emotionsToTestP2 = new List<EmotionData>(emotionsNeededForGame);
        _player1Target.gameObject.transform.localScale = Vector3.zero;
        _player2Target.gameObject.transform.localScale = Vector3.zero;
        _camera.texture = WebcamManager.instance.Webcam1;

        _currentEmotionIdxP1 = 0;
        _currentEmotionIdxP2 = 0;
        
        _startTestTimeP1 = Time.time;
        _endTestTimeP1 = _startTestTimeP1 + maxEmotionTestDuration;

        _startTestTimeP2 = Time.time;
        _endTestTimeP2 = _startTestTimeP2 + maxEmotionTestDuration;

        string txt = prepareText.GetLocalizedString();
        _player1Text.text = txt;
        _player2Text.text = txt;

        _launchText1.gameObject.SetActive(false);
        
        _player1NeutralText.gameObject.SetActive(false);
        
        UpdateUiP2();
        UpdateUiP1();
    }

    private void Update()
    {
        PositionTargetText(_player1Target, WebcamManager.instance.LastFace1Detection);
        PositionTargetText(_player2Target, WebcamManager.instance.LastFace2Detection);
        var currentTime = Time.time;
        if(currentTime < _screenEnd) return;
        _ProcessEmotionTestForPlayer1();
        _ProcessEmotionTestForPlayer2();

        if (_player1Ready && _player2Ready && _gameLaunched == false)
        {
            _gameLaunched = true;
            StartCoroutine(LaunchGame());
        }
    }

    private void PositionTargetText(RectTransform playerTarget, Detection? lastFaceDetection)
    {
        if (!lastFaceDetection.HasValue)
        {
            playerTarget.gameObject.transform.localScale = Vector3.zero;
            return;
        }
        playerTarget.gameObject.transform.localScale = Vector3.one;
        float minX = 1f - lastFaceDetection.Value.x2;//> 0f ? lastFaceDetection.Value.x1 : 0f;
        float maxX =1f -lastFaceDetection.Value.x1 ;// < 1f ? lastFaceDetection.Value.x2 : 1f;
        float minY = 1f-lastFaceDetection.Value.y2-0.075f;// > 0f ? lastFaceDetection.Value.y1-0.01f : 0f;
        float maxY = 1f-lastFaceDetection.Value.y2;//-0.01f > 0f ? lastFaceDetection.Value.y1 : 0.01f;
        
        playerTarget.anchorMin = new Vector2(minX, minY);
        playerTarget.anchorMax = new Vector2(maxX, maxY);
    }

    private void UpdateUiP1()
    {
        EmotionData currentEmotionToTest = emotionsNeededForGame[_currentEmotionIdxP1];
        
            if (doEmotionText["emotionName"] is StringVariable emotionName)
            {
                emotionName.Value = currentEmotionToTest.TextEmotion.GetLocalizedString();
                doEmotionText["emotionName"] = emotionName;
            }
               
            var txt = doEmotionText.GetLocalizedString();
            // var txt = $"Do a {currentEmotionToTest.ToString()} face !";
            _player1Text.text = txt;
            spriteP1.sprite = currentEmotionToTest.ImageEmotion;
    }

    private void UpdateUiP2()
    {
        EmotionData currentEmotionToTest = emotionsNeededForGame[_currentEmotionIdxP2];
        if (doEmotionText["emotionName"] is StringVariable emotionName)
        {
            emotionName.Value = currentEmotionToTest.TextEmotion.GetLocalizedString();
            doEmotionText["emotionName"] = emotionName;
        }
        var txt = doEmotionText.GetLocalizedString();
        _player2Text.text = txt;
        spriteP2.sprite = currentEmotionToTest.ImageEmotion;
    }

    

    private void _ProcessEmotionTestForPlayer1()
    {
        if(_player1Ready || _currentEmotionIdxP1 >= emotionsNeededForGame.Count || !WebcamManager.instance.Face1Detected)
            return;
        
        _currentTestTimeP1 = Time.time;
        int remainingTime = (int) Mathf.Round(_endTestTimeP1 - _currentTestTimeP1);
            
        EmotionData currentEmotionToTest = emotionsNeededForGame[_currentEmotionIdxP1];

        if (_GetEmotionOfPlayer(1) == currentEmotionToTest.TypeEmotion || remainingTime <= 0)
        {
            // Set new emotion
            _currentEmotionIdxP1++;
            if (_currentEmotionIdxP1 >= emotionsNeededForGame.Count)
            {
                
                _player1Ready = true;
                // _player1Text.text = waitingP2.GetLocalizedString();
                _player1Text.gameObject.SetActive(false);
                spriteP1.gameObject.SetActive(false);
                return;
            }
            UpdateUiP1();
            // Set new start time for the new emotion
            _startTestTimeP1 = Time.time;

            // Set new end time
            _endTestTimeP1 = _startTestTimeP1 + maxEmotionTestDuration;
        }
    }

    private void _ProcessEmotionTestForPlayer2()
    {
        

        if(_player2Ready || _currentEmotionIdxP2 >= emotionsNeededForGame.Count || !WebcamManager.instance.Face2Detected)
            return;
        
        _currentTestTimeP2 = Time.time;
        int remainingTime = (int) Mathf.Round(_endTestTimeP2 - _currentTestTimeP2);
        
        EmotionData currentEmotionToTest = emotionsNeededForGame[_currentEmotionIdxP2];

        if (_GetEmotionOfPlayer(2) == currentEmotionToTest.TypeEmotion || remainingTime <= 0)
        {
            // Set new emotion
            _currentEmotionIdxP2++;
            if (_currentEmotionIdxP2 >= emotionsNeededForGame.Count)
            {
                
                _player2Ready = true;
                // _player1Text.text = waitingP2.GetLocalizedString();
                _player2Text.gameObject.SetActive(false);
                spriteP2.gameObject.SetActive(false);
                return;
            }
            UpdateUiP2();
            // Set new start time for the new emotion
            _startTestTimeP2 = Time.time;

            // Set new end time
            _endTestTimeP2 = _startTestTimeP2 + maxEmotionTestDuration;
        }
        // Test emotion
       
    }

   
    private IEnumerator LaunchGame()
    {
        _launchText1.gameObject.SetActive(true);
        _player1NeutralText.gameObject.SetActive(true);
        _player1Text.gameObject.SetActive(false);
        _player2Text.gameObject.SetActive(false);
        _player2Name.SetActive(false);
        _player1Name.SetActive(false);
        spriteP1.gameObject.SetActive(false);
        spriteP2.gameObject.SetActive(false);
        for (int i = countDownStart; i > 0 ; i--)
        {
            _launchText1.text = countDownText.GetLocalizedString() + " " + i;
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
    
    public void BackToMenu()
    {
        ScenesManager.instance.LoadScene(_menuSceneName);
    }
    
    public void StartGame()
    {
        StartCoroutine(LaunchGame());
    }
}
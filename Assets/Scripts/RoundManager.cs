using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

using Random = UnityEngine.Random;
using Emotion = EmotionManager.EMOTION;
using TMPro;
using System;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    ///===============================================
    /// 
    ///                 MEMBERS
    /// 
    ///===============================================

    //============ Editor

    [SerializeField]
    private float maxRoundDuration = 10.0f; // in seconds

    [SerializeField]
    private Emotion _emotionForPass = Emotion.Neutral;

    [SerializeField]
    private int maxRoundCount = 1;

    [Header("HUD Params")]

    [SerializeField]
    private TextMeshProUGUI countDownText;

    [SerializeField]
    private TextMeshProUGUI roundText;

    [SerializeField]
    private RawImage background;

    [SerializeField]
    private float animDuration = 1f;

    [SerializeField]
    private DollyZoom dollyZoomer;

    //============ Static
    public static RoundManager Instance { get; private set; }

    //============ Logic

    private Emotion currentRoundEmotion;

    private byte winnerID; // 0: nobody, 1: player1, 2: player2, 3: both
    private bool winnerFound;

    private bool bNewRound;
    private bool bRoundStarted;
    private bool bRoundPlaying;

    private int currentRoundCount;
    private int countDownCount;

    private int player1winCount = 0;
    private int player2winCount = 0;
    private bool bAnnounceOnceFlag = true;
    private bool bCountdownOnceFlag = true;


    private float startRoundTime = 0;
    private bool bPlayRoundOnceFlag = true;


    //============ Events

    public event Action onPlayer1Loose;
    public event Action onPlayer2Loose;


    ///===============================================
    /// 
    ///                 FUNCTIONS
    /// 
    ///===============================================

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        winnerID = 0;
        winnerFound = false;
        bNewRound = true;
        bRoundStarted = false;
        bRoundPlaying = false;

        currentRoundCount = 1;
    }

    void Update()
    {
        if (currentRoundCount < maxRoundCount)
            _LaunchOneRound();
        else
            _AnnounceResult();
    }

    void _AnnounceResult()
    {
        if (!bAnnounceOnceFlag)
            return;

        bAnnounceOnceFlag = false;

        if (player1winCount == player2winCount)
        {
            Debug.Log("Equality");
        }
        else if (player1winCount > player2winCount)
        {
            Debug.Log("Player 1 Win ! ");
            Debug.Log("Player 2 you looooooose ! ");
        }
        else if (player2winCount > player1winCount)
        {
            Debug.Log("Player 2 Win ! ");
            Debug.Log("Player 1 you looooooose ! ");
        }
    }

    private void _LaunchOneRound()
    {
        StartCoroutine(_BeginOneRound());
        _PlayOneRound();
        _EndOneRound();
    }

    private IEnumerator _BeginOneRound()
    {
        if (bRoundStarted)
            yield return null;

        // 1) Init winner id
        winnerID = 0;

        // 2) Choose an emotion
        if (bNewRound)
        {
            bNewRound = false;
            bCountdownOnceFlag = false;
            currentRoundEmotion = _GetRandomEmotion();

            roundText.text = $"Round {currentRoundCount}";

            // Animation
            roundText.DOFade(1f, animDuration).SetUpdate(true).SetDelay(animDuration).OnComplete(() =>
            {
                roundText.DOFade(0f, animDuration * .3f).SetUpdate(true);
            });

            roundText.rectTransform.DOScale(Vector3.one, animDuration).SetUpdate(true).SetDelay(animDuration).OnComplete(() =>
            {
                roundText.rectTransform.DOScale(new Vector3(2.8675f, 2.8675f, 2.8675f), animDuration * .3f).SetUpdate(true);
                bCountdownOnceFlag = true;
            });

            Debug.Log($"Round {currentRoundCount + 1}, Emotion to reproduce : {currentRoundEmotion.ToString()}");

            yield return new WaitForSeconds(animDuration + 0.5f);
        }

        // Check for neutral state of the 2 players
        if (_GetEmotionOfPlayer(1) != _emotionForPass || _GetEmotionOfPlayer(2) != _emotionForPass)
        {
            Debug.Log("Please be " + _emotionForPass.ToString() + " before to start.");
            yield return null;
        }

        // 3) Start to count down
        if (bCountdownOnceFlag) _StartCountDown();
    }

    private void _PlayOneRound()
    {
        if (!bRoundStarted)
            return;

        if (bPlayRoundOnceFlag)
        {
            startRoundTime = Time.time;
            bPlayRoundOnceFlag = false;
        }


        float currentRoundTime = Time.time;
        float roundDuration = currentRoundTime - startRoundTime;

        if (!winnerFound && roundDuration < maxRoundDuration)
        {
            // 3) Check for a winner
            if (_GetEmotionOfPlayer(1) == currentRoundEmotion && _GetEmotionOfPlayer(2) == currentRoundEmotion)
            {
                // Match nul
                winnerID = 3;
                winnerFound = true;
                bRoundPlaying = false;
            }
            else if (_GetEmotionOfPlayer(1) == currentRoundEmotion)
            {
                // Player 1 win
                winnerID = 1;
                winnerFound = true;
                bRoundPlaying = false;
            }
            else if (_GetEmotionOfPlayer(2) == currentRoundEmotion)
            {
                // Player 2 win
                winnerID = 2;
                winnerFound = true;
                bRoundPlaying = false;
            }
            else
            {
                winnerFound = false;
                bRoundPlaying = true;
            }
        }
        else if (roundDuration > maxRoundDuration)
        {
            bRoundPlaying = false;
            bPlayRoundOnceFlag = true;
            winnerID = 0;
        }
    }

    private void _EndOneRound()
    {
        if (!bRoundStarted || bRoundPlaying)
            return;

        switch (winnerID)
        {
            case 0:
                Debug.Log("Nobody win");
                onPlayer1Loose?.Invoke();
                break;

            case 1:
                Debug.Log("Player 1 win");
                player1winCount++;
                onPlayer2Loose?.Invoke();
                break;

            case 2:
                Debug.Log("Player 2 win");
                player2winCount++;
                onPlayer1Loose?.Invoke();
                break;

            case 3:
                Debug.Log("Both win");
                break;
        }

        bNewRound = true;
        bRoundStarted = false;
        bCountdownOnceFlag = true;
        currentRoundCount++;
    }

    private void _StartCountDown()
    {
        countDownCount = 3;
        StartCoroutine(_CountdownFor());
        bCountdownOnceFlag = false;
    }

    private IEnumerator _CountdownFor()
    {
        if (countDownCount > 0)
        {
            countDownText.text = countDownCount.ToString();
        }
        else
        {
            countDownText.text = "Go !!";
            bRoundStarted = true;

            dollyZoomer.timeZoom = (int)Mathf.Round(animDuration);
            dollyZoomer.doZoom = true;
            background.DOFade(0, 0.000001f).SetUpdate(true);
        }

        // Animation
        countDownText.DOFade(1f, .3f).SetUpdate(true).OnComplete(() =>
        {
            countDownText.DOFade(0f, .3f).SetUpdate(true);
        });

        countDownText.rectTransform.DOScale(new Vector3(2.8675f, 2.8675f, 2.8675f), .3f).SetUpdate(true).OnComplete(() =>
        {
            countDownText.rectTransform.DOScale(Vector3.one, .3f).SetUpdate(true);
        });

        yield return new WaitForSeconds(1f);

        countDownCount--;

        if (countDownCount>=0) StartCoroutine(_CountdownFor());
        else Time.timeScale = 1f;
    }

    private Emotion _GetRandomEmotion()
    {
        return (Emotion)Random.Range(1, System.Enum.GetNames(typeof(Emotion)).Length);
    }

    private Emotion _GetEmotionOfPlayer(int playerID)
    {
        // return playerID == 1 ? EmotionManager.instance.GetPlayer1Emotion() : EmotionManager.instance.GetPlayer2Emotion();
        return Emotion.Neutral;
    }
}
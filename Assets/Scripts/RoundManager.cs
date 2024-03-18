using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;
using Emotion = EmotionManager.EMOTION;

public class RoundManager : MonoBehaviour
{
    [SerializeField]
    private float maxRoundDuration = 10.0f; // in seconds

    [SerializeField]
    private Emotion _emotionForPass = Emotion.Neutral;

    [SerializeField]
    private int maxRoundCount = 1;

    private Emotion currentRoundEmotion;

    private byte winnerID; // 0: nobody, 1: player1, 2: player2, 3: both
    private bool winnerFound;

    private bool isNewRound;
    private bool isRoundStarted;
    private bool isRoundPlaying;

    private int currentRoundCount;
    private float countdownOverTimestamp;

    private int player1winCount = 0;
    private int player2winCount = 0;
    private bool bAnnounceOnceFlag = true;

    void Start()
    {
        winnerID = 0;
        winnerFound = false;
        isNewRound = true;
        isRoundStarted = false;
        isRoundPlaying = false;

        currentRoundCount = 0;
        countdownOverTimestamp = 0.0f;
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
        _BeginOneRound();
        _PlayOneRound();
        _EndOneRound();
    }

    private void _BeginOneRound()
    {
        if (isRoundStarted)
            return;

        // 1) Init winner id
        winnerID = 0;

        // 2) Choose an emotion
        if (isNewRound)
        {
            isNewRound = false;
            currentRoundEmotion = _GetRandomEmotion();

            Debug.Log($"Round {currentRoundCount + 1}, Emotion to reproduce : {currentRoundEmotion.ToString()}");
        }

        // Check for neutral state of the 2 players
        if (_GetEmotionOfPlayer(1) != _emotionForPass || _GetEmotionOfPlayer(2) != _emotionForPass)
        {
            Debug.Log("Please be " + _emotionForPass.ToString() + " before to start.");
            return;
        }

        // 3) Start to count down
        StartCoroutine(_CountdownFor(3));
    }

    private void _PlayOneRound()
    {
        if (!isRoundStarted)
            return;

        float timeLimit = (countdownOverTimestamp + maxRoundDuration) - Time.time;

        if (!winnerFound && timeLimit > 0.0f)
        {
            Debug.Log(Mathf.Floor(timeLimit));

            // 3) Check for a winner
            if (_GetEmotionOfPlayer(1) == currentRoundEmotion && _GetEmotionOfPlayer(2) == currentRoundEmotion)
            {
                // Match nul
                winnerID = 3;
                winnerFound = true;
                isRoundPlaying = false;
            }
            else if (_GetEmotionOfPlayer(1) == currentRoundEmotion)
            {
                // Player 1 win
                winnerID = 1;
                winnerFound = true;
                isRoundPlaying = false;
            }
            else if (_GetEmotionOfPlayer(2) == currentRoundEmotion)
            {
                // Player 2 win
                winnerID = 2;
                winnerFound = true;
                isRoundPlaying = false;
            }
            else
            {
                winnerFound = false;
                isRoundPlaying = true;
            }
        }
        else if (Time.time > countdownOverTimestamp + maxRoundDuration)
        {
            isRoundPlaying = false;
            winnerID = 0;
        }
    }

    private void _EndOneRound()
    {
        if (!isRoundStarted || isRoundPlaying)
            return;

        switch (winnerID)
        {
            case 0:
                Debug.Log("Nobody win");
                break;

            case 1:
                Debug.Log("Player 1 win");
                player1winCount++;
                break;

            case 2:
                Debug.Log("Player 2 win");
                player2winCount++;
                break;

            case 3:
                Debug.Log("Both win");
                break;
        }

        isNewRound = true;
        isRoundStarted = false;
        currentRoundCount++;
    }

    private IEnumerator _CountdownFor(int seconds)
    {
        Debug.Log("Be ready....");

        for (int i = seconds; i > 0; i--)
        {
            Debug.Log(i);
            yield return new WaitForSeconds(1);
        }
        Debug.Log("Go !!");

        isRoundStarted = true;
        countdownOverTimestamp = Time.time;
        winnerFound = false;
    }

    private Emotion _GetRandomEmotion()
    {
        return (Emotion)Random.Range(1, System.Enum.GetNames(typeof(Emotion)).Length);
    }

    private Emotion _GetEmotionOfPlayer(int playerID)
    {
        return playerID == 1 ? EmotionManager.instance.GetPlayer1Emotion() : EmotionManager.instance.GetPlayer2Emotion();
    }
}
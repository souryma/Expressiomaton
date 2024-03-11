using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;
using Emotion = EmotionManager.EMOTION;

public class RoundManager : MonoBehaviour
{
    [SerializeField]
    private float maxRoundDuration = 10.0f; // in seconds

    //========================= INTERNALS

    private byte winnerID; // 0: nobody, 1: player1, 2: player2, 3: both
    private bool winnerFound = false;
    private bool isNewRound = false;
    private bool isRoundStarted = false;
    private Emotion currentRoundEmotion;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _LaunchRound();
    }

    private void _LaunchRound()
    {
        _BeginRound();
        _PlayRound();
        _EndRound();
    }

    private void _BeginRound()
    {
        // 1) Init winner id
        winnerID = 0;

        // 2) Choose an emotion
        if (isNewRound)
        {
            isNewRound = false;
            currentRoundEmotion = _GetRandomEmotion();
            Debug.Log("Round Emotion" + currentRoundEmotion.ToString());
        }

        // Check for neutral state of the 2 players
/*        if (_GetEmotionOfPlayer(1) != Emotion.Neutral || _GetEmotionOfPlayer(2) != Emotion.Neutral)
        {
            Debug.Log("Please be neutral ma boy.");
            return;
        }*/

        // 3) Start to count down
        StartCoroutine(_CountdownFor(3));
    }

    private void _PlayRound()
    {
        StartCoroutine(_PlayRoundCoroutine());
    }

    private IEnumerator _PlayRoundCoroutine()
    {
        if (!isRoundStarted)
            yield return null;

        winnerFound = false;

        while (!winnerFound && Time.time < maxRoundDuration)
        {
            // 3) Check for a winner
            if (_GetEmotionOfPlayer(1) == currentRoundEmotion && _GetEmotionOfPlayer(2) == currentRoundEmotion)
            {
                // Match nul
                winnerFound = true;
                winnerID = 3;
            }
            else if (_GetEmotionOfPlayer(1) == currentRoundEmotion)
            {
                // Player 1 win
                winnerFound = true;
                winnerID = 1;
            }
            else if (_GetEmotionOfPlayer(2) == currentRoundEmotion)
            {
                // Player 2 win
                winnerFound = true;
                winnerID = 2;
            }

            yield return null;
        }
    }

    private void _EndRound()
    {
        if (!isRoundStarted)
            return;

        switch (winnerID)
        {
            case 0:
                Debug.Log("Nobody win");
                break;

            case 1:
                Debug.Log("Player 1 win");
                break;

            case 2:
                Debug.Log("Player 2 win");
                break;

            case 3:
                Debug.Log("Both win");
                break;
        }

        isNewRound = true;
        isRoundStarted = false;
    }

    private IEnumerator _CountdownFor(int seconds)
    {
        for (int i = seconds; i > 0; i--)
        {
            Debug.Log("Counter : " + i);
            yield return new WaitForSeconds(1);
        }
        Debug.Log("Go !!");
        isRoundStarted = true;
    }

    private Emotion _GetRandomEmotion()
    {
        return (Emotion)Random.Range(0, System.Enum.GetNames(typeof(Emotion)).Length);
    }

    private Emotion _GetEmotionOfPlayer(int playerID)
    {
        return playerID == 1 ? EmotionManager.instance.GetPlayer1Emotion() : EmotionManager.instance.GetPlayer1Emotion();
    }
}
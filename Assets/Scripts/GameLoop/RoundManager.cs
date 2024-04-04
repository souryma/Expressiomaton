using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Localization;
using Random = UnityEngine.Random;
using Emotion = EmotionManager.EMOTION;

public class RoundManager : MonoBehaviour
{
    ///===============================================
    /// 
    ///                 MEMBERS
    /// 
    ///===============================================

    //============ Editor

    [SerializeField]
    private float maxRoundDuration = 20.0f; // in seconds

    [SerializeField]
    private Emotion _emotionForPass = Emotion.Neutral;

    [SerializeField]
    private int maxRoundCount = 1;

    [SerializeField]
    private float animDuration = 1f;

    [SerializeField]
    private float dollyZoomDuration = 4f;

    [SerializeField]
    private DollyZoom dollyZoomer;

    [Header("Player HUDs")]

    [SerializeField]
    private HUD[] playersHUD;

    [SerializeField]
    private Sprite defaultTexture;

    [SerializeField]
    private Vector3 rewardInitPos;

    
    [Header("Translations")]
    [SerializeField]
    private LocalizedString youWin;
    
    [SerializeField]
    private LocalizedString youLose;
    
    [SerializeField]
    private LocalizedString round;
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
    private bool bAnnounceOnceFlag = false;
    private bool bCountdownOnceFlag = true;

    private bool bContinueGame = true;


    private float startRoundTime = 0;
    private bool bPlayRoundOnceFlag = true;

    private float rewardSpacing = 200f;

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
        else
        {
            // Destroy(this);
        }
    }

    void Start()
    {
        winnerID = 0;
        winnerFound = false;
        bNewRound = true;
        bRoundStarted = false;
        bRoundPlaying = false;

        currentRoundCount = 1;

/*        for (int i = 0; i < *//* playerCount= *//* 2; i++)
        {
            for (int j = 0; j < *//* rewardCount= *//* 3; j++)
            {
                starInitialPositions[3 * i + j] = playersHUD[i].rewardManager.transform.GetChild(i).position;
                starInitialRotators[3 * i + j] = playersHUD[i].rewardManager.transform.GetChild(i).rotation;
            }
        }*/
    }

    void Update()
    {
        if (bContinueGame)
            _LaunchOneRound();
        else
            _AnnounceResult();
    }

    void _AnnounceResult()
    {
        if (bAnnounceOnceFlag)
            return;

        bAnnounceOnceFlag = true;

        // Load Victory scene here with some params.

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

        if (!bRoundStarted || bRoundPlaying)
            return;

        StartCoroutine(_EndOneRound());
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

            _AnnounceRound();

            Debug.Log($"Round {currentRoundCount}, Emotion to reproduce : {currentRoundEmotion.ToString()}");

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

    private IEnumerator _EndOneRound()
    {
        SoundManager.instance.PlayShotgunSound();

        switch (winnerID)
        {
            case 0:
                Debug.Log("Nobody win");
                onPlayer1Loose?.Invoke();
                player2winCount++;
                playersHUD[1].roundResult.text = "You Loose";
                playersHUD[0].roundResult.text = "You Win";

                playersHUD[0].scorePlayer.transform.GetChild(player2winCount - 1).DOScale(1f, animDuration).SetUpdate(true).SetDelay(animDuration).SetEase(Ease.OutElastic).OnComplete(() =>
                {
                    playersHUD[0].scorePlayer.transform.GetChild(player2winCount - 1).DOMove(new Vector3(rewardInitPos.x + (player1winCount - 1) * rewardSpacing, rewardInitPos.y, rewardInitPos.z), animDuration).SetUpdate(true).SetDelay(animDuration).SetEase(Ease.InOutCubic);
                });
                break;

            case 1:
                Debug.Log("Player 1 win");
                player1winCount++;
                onPlayer2Loose?.Invoke();
                playersHUD[0].roundResult.text = "You Loose";
                playersHUD[1].roundResult.text = "You Win";

                // reward animation
                playersHUD[1].scorePlayer.transform.GetChild(player1winCount - 1).DOScale(1f, animDuration).SetUpdate(true).SetDelay(animDuration).SetEase(Ease.OutElastic).OnComplete(() =>
                {
                    playersHUD[1].scorePlayer.transform.GetChild(player1winCount - 1).DOMove(new Vector3(rewardInitPos.x + (player1winCount - 1) * rewardSpacing, rewardInitPos.y, rewardInitPos.z), animDuration).SetUpdate(true).SetDelay(animDuration).SetEase(Ease.InOutCubic);
                });
                break;

            case 2:
                Debug.Log("Player 2 win");
                player2winCount++;
                onPlayer1Loose?.Invoke();
                playersHUD[1].roundResult.text = "You Loose";
                playersHUD[0].roundResult.text = "You Win";

                playersHUD[1].scorePlayer.transform.GetChild(player2winCount - 1).DOScale(1f, animDuration).SetUpdate(true).SetDelay(animDuration).SetEase(Ease.OutElastic).OnComplete(() =>
                {
                    playersHUD[1].scorePlayer.transform.GetChild(player2winCount - 1).DOMove(new Vector3(rewardInitPos.x + (player2winCount - 1) * rewardSpacing, rewardInitPos.y, rewardInitPos.z), animDuration).SetUpdate(true).SetDelay(animDuration).SetEase(Ease.InOutCubic);
                });
                break;

            case 3:
                Debug.Log("Both win");
                break;
        }

        // Animation
        foreach (HUD playerHUD in playersHUD)
        {
            playerHUD.roundResult.DOFade(1f, animDuration).SetUpdate(true).SetDelay(animDuration).OnComplete(() =>
            {
                playerHUD.roundResult.DOFade(0f, animDuration * .3f).SetUpdate(true);
            });

            playerHUD.roundResult.rectTransform.DOScale(Vector3.one, animDuration).SetUpdate(true).SetDelay(animDuration).OnComplete(() =>
            {
                playerHUD.roundResult.rectTransform.DOScale(new Vector3(2.8675f, 2.8675f, 2.8675f), animDuration * .3f).SetUpdate(true);
            });
        }

        currentRoundCount++;
        bContinueGame = currentRoundCount <= maxRoundCount;
        yield return new WaitForSeconds(4f);


        // Reinit flags
        bNewRound = true;
        bRoundStarted = false;
        bCountdownOnceFlag = true;
        winnerFound = false;
        bPlayRoundOnceFlag = true;
    }

    // Launch the countdown
    private void _StartCountDown()
    {
        countDownCount = 3;
        StartCoroutine(_CountdownFor());
        bCountdownOnceFlag = false;
    }

    // Compute the countdown
    private IEnumerator _CountdownFor()
    {
        if (countDownCount > 0)
        {
            _UpdateCountdownText(/* text= */countDownCount.ToString());
        }
        else
        {
            _UpdateCountdownText(/* text= */"Go !!", /* bRemovebg= */true);
            _LaunchSuspenseCinematic();

            bRoundStarted = true;
        }

        yield return new WaitForSeconds(1f);

        countDownCount--;

        if (countDownCount>=0) StartCoroutine(_CountdownFor());
        else Time.timeScale = 1f;
    }

    // Returns a random emotion
    private Emotion _GetRandomEmotion()
    {
        return (Emotion)Random.Range(1, System.Enum.GetNames(typeof(Emotion)).Length);
    }

    // Returns the emotion of a given player
    private Emotion _GetEmotionOfPlayer(int playerID)
    {
        return playerID == 1 ? EmotionManager.instance.GetPlayer1Emotion() : EmotionManager.instance.GetPlayer2Emotion();
        // return Emotion.Neutral;
    }

    // Start the dolly zoom animation for adding suspense
    private void _LaunchSuspenseCinematic()
    {
        dollyZoomer.timeZoom = (int)Mathf.Round(animDuration);
        dollyZoomer.doZoom = true;
        SoundManager.instance.PlayWind();
    }

    // Display the current round on all the HUDs
    private void _AnnounceRound()
    {
        float randomDelay = 10f +  Mathf.Clamp(Random.Range(-1, 1), -1,  1) * Random.Range(1, 6);
        // Animation
        foreach (HUD playerHUD in playersHUD)
        {
            // playerHUD.animHandler.DOFade(1f, animDuration).SetUpdate(true).SetDelay(/* duration= */0f).OnComplete(() =>
            // {
            //     playerHUD.roundText.text = $"Round {currentRoundCount}";
            // });

            playerHUD.roundText.DOFade(1f, animDuration).SetUpdate(true).SetDelay(animDuration).OnComplete(() =>
            {
                playerHUD.roundText.DOFade(0f, animDuration * .3f).SetUpdate(true).OnComplete(() =>
                {
                    playerHUD.roundText.text = "";
                });
            });

            playerHUD.roundText.rectTransform.DOScale(Vector3.one, animDuration).SetUpdate(true).SetDelay(animDuration).OnComplete(() =>
            {
                playerHUD.roundText.rectTransform.DOScale(new Vector3(2.8675f, 2.8675f, 2.8675f), animDuration * .3f).SetUpdate(true).OnComplete(() =>
                {
                    bCountdownOnceFlag = true;
                });
            });
            
            //
            // playerHUD.animHandler.DOFade(1f, randomDelay).SetUpdate(true).SetDelay(/* duration= */0f).OnComplete(() =>
            // {
            //     playerHUD.emotionText.text = currentRoundEmotion.ToString();
            //     playerHUD.emotionImage.sprite = EmotionDataset.Instance.data[currentRoundEmotion].ImageEmotion;
            // });

            playerHUD.emotionText.DOFade(1f, animDuration).SetUpdate(true).SetDelay(randomDelay).OnComplete(() =>
            {
                playerHUD.emotionText.DOFade(0f, animDuration * .3f).SetUpdate(true).OnComplete(() =>
                {
                    playerHUD.emotionText.text = "";
                });
            });

            playerHUD.emotionText.rectTransform.DOScale(Vector3.one, animDuration).SetUpdate(true).SetDelay(randomDelay).OnComplete(() =>
            {
                playerHUD.emotionText.rectTransform.DOScale(new Vector3(2.8675f, 2.8675f, 2.8675f), animDuration * .3f).SetUpdate(true);
            });

            playerHUD.emotionImage.DOFade(1f, animDuration).SetUpdate(true).SetDelay(randomDelay).OnComplete(() =>
            {
                playerHUD.emotionImage.DOFade(0f, animDuration * .3f).SetUpdate(true).OnComplete(() =>
                {
                    playerHUD.emotionImage.sprite = defaultTexture;
                });

            });

            playerHUD.emotionImage.rectTransform.DOScale(Vector3.one, animDuration).SetUpdate(true).SetDelay(randomDelay).OnComplete(() =>
            {
                playerHUD.emotionImage.rectTransform.DOScale(new Vector3(2.8675f, 2.8675f, 2.8675f), animDuration * .3f).SetUpdate(true);
            });
        }
    }

    // Display the countdown on all the HUDs
    private void _UpdateCountdownText(string text, bool bRemovebg = false)
    {
        foreach (HUD playerHUD in playersHUD)
        {
            playerHUD.countDownText.text = text;

            // Animation
            playerHUD.countDownText.DOFade(1f, .3f).SetUpdate(true).OnComplete(() =>
            {
                playerHUD.countDownText.DOFade(0f, .3f).SetUpdate(true);
            });

            playerHUD.countDownText.rectTransform.DOScale(new Vector3(2.8675f, 2.8675f, 2.8675f), .3f).SetUpdate(true).OnComplete(() =>
            {
                playerHUD.countDownText.rectTransform.DOScale(Vector3.one, .3f).SetUpdate(true);
            });

            if (bRemovebg)
                playerHUD.background.DOFade(0, 0.000001f).SetUpdate(true);
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Emotion = EmotionManager.EMOTION;
namespace GameLoop
{
    public class RoundManagerNew: MonoBehaviour
    {
        [Header("Game Data")] 
        [SerializeField] 
        private List<EmotionData> playableEmotions;
    
        [SerializeField]
        private float maxRoundDuration = 20.0f; // in seconds
    
        [SerializeField]
        private int countDownBeforeRoundStart = 3; // in seconds
  
        [SerializeField]
        private int minTimeBeforeEmotionDisplay = 2; // in seconds
        [SerializeField]
        private int maxTimeBeforeEmotionDisplay = 7; // in seconds

        [SerializeField]
        private int roundNeededToWin = 3;

        [SerializeField] 
        private int maxRoundCount = 9;
    
        [FormerlySerializedAs("emotionForPass")] [SerializeField]
        private EmotionData emotionToKeepOnCountdown;
        [SerializeField]
        private float animDuration = 1f;

        [SerializeField]
        private int dollyZoomDuration = 10;

        [SerializeField]
        private DollyZoom dollyZoomer;

        [Header("Player HUDs")]

        [SerializeField]
        private HUD[] playersHUD;
    
        [SerializeField]
        private Vector3 rewardInitPos;

        [Header("Misc")] 
        [SerializeField] private string victoryScene = "VictoryScene";
    
        [Header("Translations")]
        [SerializeField]
        private LocalizedString youWinText;
    
        [SerializeField]
        private LocalizedString youLoseText;
    
        [SerializeField]
        private LocalizedString drawText;
    
        [SerializeField]
        private LocalizedString roundText;
    
        [SerializeField]
        private LocalizedString goText;
    
        [Header("GameEvents")]
        [SerializeField]
        private GameEvent player1WinRound;
    
        [SerializeField]
        private GameEvent player2WinRound;
        [SerializeField]
        private GameEvent drawWinRound;
    
        [SerializeField]
        private GameEvent takeWebCamScreenShot;

        [SerializeField] 
        private int timeToDetectWrongEmotion = 300;
        [SerializeField] 
        private int timeToWaitNeutral = 1000;
        
        private DateTime player1SaveTimeStart = DateTime.Now;
        private bool player1WrongEmotionDetected = false;
        private bool player1NeutralDetected = false;
        private DateTime player2SaveTimeStart;
        private bool player2WrongEmotionDetected = false;
        private bool player2NeutralDetected = false;

    
        //============ Logic

        private Emotion currentRoundEmotion;
    
        public enum Winner
        {
            NONE,
            PLAYER_1,
            PLAYER_2,
            BOTH
        }

        private int _currentRoundCount;
        private int _player1Wins = 0; // 0: nobody, 1: player1, 2: player2, 3: both
        private int _player2Wins = 0; // 0: nobody, 1: player1, 2: player2, 3: both
        private bool _isCurrentlySearchingForEmotion;

        private EmotionData _currentRoundEmotionData;
        private int _currentRoundTimeBeforeEmotionDisplay;

        private Coroutine _roundTooLongCoroutine;
        private Coroutine _roundCoroutine;
        private bool _gameIsLaunched = false;
        private Emotion p1Emotion;
        private Emotion p2Emotion;

        //============ Events

    
    
        private void Start()
        {
            ResetHUD();
            foreach (var hud in playersHUD)
            {
                foreach (var child in hud.scorePlayer.GetComponentsInChildren<Image>())
                {
                    child.DOFade(0F, 0F);
                }

                foreach (var child in hud.scoreOpponent.GetComponentsInChildren<Image>())
                {
                    child.DOFade(0F, 0F);
                }
            }

            // LaunchGame();
        }

        private void ResetHUD()
        {
            foreach (var hud in playersHUD)
            {
                hud.cameraReturn.DOFade(0f, 0f);
                hud.neutralityScore.DOFade(0f, 0f);
                hud.roundResult.DOFade(0f, 0f);
                hud.emotionImage.DOFade(0f, 0f);
                hud.countDownText.DOFade(0f, 0f);
                hud.emotionText.DOFade(0f, 0f);
                hud.roundText.DOFade(0f, 0f);
            
            }
        }

        public void LaunchGame()
        { 

            _currentRoundCount = 1;
            _roundCoroutine = StartCoroutine(StartOneRound());

        }

        private void SetupRound()
        {
            SetCurrentRoundEmotionData();
            _currentRoundTimeBeforeEmotionDisplay = Random.Range(minTimeBeforeEmotionDisplay, maxTimeBeforeEmotionDisplay);
            player1WrongEmotionDetected = false;
            player2WrongEmotionDetected = false;
            player1NeutralDetected = false;
            player2NeutralDetected = false;
        }

        private IEnumerator StartOneRound()
        {
            // Hide everything from ui
            foreach (HUD playerHUD in playersHUD)
            {
                playerHUD.keepNeutralText.DOFade(1f, animDuration);
                ShowNeutralScore(playerHUD);
            }

            //Engineer all roundText logic
            SetupRound();
            //Display Round and roundText Number
            RoundAnnouncement();

            yield return new WaitForSeconds(animDuration * 2.3f);
            foreach (HUD playerHUD in playersHUD)
            {
                ShowEmotionPrompt(playerHUD, emotionToKeepOnCountdown);
            }
            while (WaitForPlayersBeforeStart() )
                   // || !WebcamManager.instance.Face1Detected || !WebcamManager.instance.Face2Detected)
            {
                yield return new WaitForEndOfFrame();
            }
            
            yield return new WaitForSeconds(animDuration);
        
            foreach (HUD playerHUD in playersHUD)
            {
                HideEmotionPrompt(playerHUD);
                playerHUD.keepNeutralText.DOFade(0f, animDuration* 0.3f);
            }
            yield return new WaitForSeconds(animDuration* 0.3f);
            //Display Countdown until roundText start
            for (int i = countDownBeforeRoundStart; i > 0; i--)
            {
                AnimateCountDownText(i.ToString());

                yield return new WaitForSeconds(1f);
            }
            // foreach (HUD playerHUD in playersHUD)
            // {
            //     EmotionPromptPopup(playerHUD, emotionToKeepOnCountdown);
            // }
            AnimateCountDownText(goText.GetLocalizedString());
       
            _roundTooLongCoroutine = StartCoroutine(EndPlayerNotWork());
        
 
            _gameIsLaunched = true;
            //Start dolly
            LaunchDolly();
        
            // Make emotion prompt appear
            yield return new WaitForSeconds(_currentRoundTimeBeforeEmotionDisplay);
            foreach (HUD playerHUD in playersHUD)
            {
                EmotionPromptPopup(playerHUD, _currentRoundEmotionData);
                HideNeutralScore(playerHUD);
            }

            //start expression recognition
            _isCurrentlySearchingForEmotion = true;

        }

        private bool WaitForPlayersBeforeStart()
        {
            if (!player1NeutralDetected && p1Emotion == emotionToKeepOnCountdown.TypeEmotion)
            {
                player1SaveTimeStart = DateTime.Now;
                player1NeutralDetected = true;
            }
            else if(p1Emotion != emotionToKeepOnCountdown.TypeEmotion)
            {
                player1NeutralDetected = false;
            }

            if (!player2NeutralDetected && p2Emotion == emotionToKeepOnCountdown.TypeEmotion)
            {
                player2SaveTimeStart = DateTime.Now;
                player2NeutralDetected = true;
            }
            else if(p2Emotion != emotionToKeepOnCountdown.TypeEmotion)
            {
                player2NeutralDetected = false;
            }

            return !((player1NeutralDetected && DateTime.Now.Subtract(player1SaveTimeStart).TotalMilliseconds >=
                         timeToWaitNeutral) &&
                   (player2NeutralDetected && DateTime.Now.Subtract(player2SaveTimeStart).TotalMilliseconds >=
                       timeToWaitNeutral));
        }

        private void HideNeutralScore(HUD playerHUD)
        {
            playerHUD.neutralityScore.DOFade(0f, animDuration * 0.3f);
            // playerHUD.cameraReturn.DOFade(0f, animDuration * 0.3f);
        }

        private void HideCameraReturn(HUD playerHUD)
        {
            playerHUD.cameraReturn.DOFade(0f, animDuration * 0.3f);
        }

        private void ShowNeutralScore(HUD playerHUD)
        {
            playerHUD.neutralityScore.DOFade(1f, animDuration * 0.3f);
            playerHUD.cameraReturn.DOFade(1f, animDuration * 0.3f);

        }

        private void EmotionPromptPopup(HUD playerHUD, EmotionData emotionToShow)
        {
            // playerHUD.emotionImage.DOKill();
            // playerHUD.emotionText.DOKill();
            // playerHUD.emotionImage.color = new Color(1,1,1, 0);
            playerHUD.emotionText.text = emotionToShow.TextEmotion.GetLocalizedString();
            playerHUD.emotionImage.sprite = emotionToShow.ImageEmotion;
            Sequence emotionTextSequence = DOTween.Sequence();
            emotionTextSequence.Append(playerHUD.emotionText.DOFade(1f, animDuration));
            emotionTextSequence.AppendInterval(animDuration*0.5f);
            emotionTextSequence.Append(playerHUD.emotionText.DOFade(0f, animDuration*0.3f));
            Sequence emotionScaleSequence = DOTween.Sequence();
            emotionScaleSequence.AppendInterval(animDuration);
            emotionScaleSequence.AppendInterval(animDuration*0.5f);
            emotionScaleSequence.Append(  playerHUD.emotionText.transform.DOScale(new Vector3(2.8675f, 2.8675f, 2.8675f), animDuration * .3f));
            emotionScaleSequence.Append(playerHUD.emotionText.transform.DOScale(Vector3.one, 0f));

            Sequence emotionImageSequence = DOTween.Sequence();
            emotionImageSequence.Append(playerHUD.emotionImage.DOFade(1f, animDuration));
            emotionImageSequence.AppendInterval(animDuration*0.5f);
            emotionImageSequence.Append(playerHUD.emotionImage.DOFade(0f, animDuration*0.3f));
            Sequence emotionImageScaleSequence = DOTween.Sequence();
            emotionImageScaleSequence.AppendInterval(animDuration);
            emotionImageScaleSequence.AppendInterval(animDuration*0.5f);
            emotionImageScaleSequence.Append( playerHUD.emotionImage.transform.DOScale(new Vector3(2.8675f, 2.8675f, 2.8675f), 
                animDuration * .3f));
            emotionImageScaleSequence.Append(playerHUD.emotionImage.transform.DOScale(Vector3.one, 0f));
            emotionTextSequence.Play();
            emotionScaleSequence.Play();
            emotionImageSequence.Play();
            emotionImageScaleSequence.Play();
        }

        private void ShowEmotionPrompt(HUD playerHUD, EmotionData emotionToShow)
        {
            playerHUD.emotionImage.DOKill();
            playerHUD.emotionText.DOKill();
            // playerHUD.emotionImage.color = new Color(1,1,1, 0);
            playerHUD.emotionText.text = emotionToShow.TextEmotion.GetLocalizedString();
            playerHUD.emotionImage.sprite = emotionToShow.ImageEmotion;
            Sequence emotionTextSequence = DOTween.Sequence();
            emotionTextSequence.Append(playerHUD.emotionText.DOFade(1f, animDuration));
            Sequence emotionImageSequence = DOTween.Sequence();
            emotionImageSequence.Append(playerHUD.emotionImage.DOFade(1f, animDuration));
            emotionTextSequence.Play();
            emotionImageSequence.Play();
        }
    
        private void HideEmotionPrompt(HUD playerHUD)
        {
            playerHUD.emotionImage.DOKill();
            playerHUD.emotionText.DOKill();
            Sequence emotionTextSequence1 = DOTween.Sequence();
            emotionTextSequence1.Append(playerHUD.emotionText.DOFade(0f, animDuration*0.3f));
            // emotionTextSequence.Append(playerHUD.emotionText.DOFade(0f, 0f));
            Sequence emotionScaleSequence2 = DOTween.Sequence();
            emotionScaleSequence2.Append(  playerHUD.emotionText.transform.DOScale(new Vector3(2.8675f, 2.8675f, 2.8675f), 
                animDuration * .29f));
            emotionScaleSequence2.Append(playerHUD.emotionText.transform.DOScale(Vector3.one, 0f));
            Sequence emotionImageSequence3 = DOTween.Sequence();
            emotionImageSequence3.Append(playerHUD.emotionImage.DOFade(0f, animDuration*0.3f));
            emotionImageSequence3.Append(playerHUD.emotionText.DOFade(0f, 0f));
            Sequence emotionImageScaleSequence4 = DOTween.Sequence();
            emotionImageScaleSequence4.Append(  playerHUD.emotionImage.transform.DOScale(new Vector3(2.8675f, 2.8675f, 2.8675f), 
                animDuration * .29f));
            emotionImageScaleSequence4.Append(playerHUD.emotionImage.transform.DOScale(Vector3.one, 0f));
            emotionTextSequence1.Play();
            emotionScaleSequence2.Play();
            emotionImageSequence3.Play();
            emotionImageScaleSequence4.Play();
        }
        private IEnumerator EndPlayerNotWork()
        {
            yield return new WaitForSeconds(maxRoundDuration);
            if (_isCurrentlySearchingForEmotion)
            {
                _isCurrentlySearchingForEmotion = false;
                RoundWinner(Winner.NONE);
            }
        }

        private void RoundAnnouncement()
        {
            foreach (HUD playerHUD in playersHUD)
            {
                // playerHUD.roundText.DOKill();
                playerHUD.roundText.text = roundText.GetLocalizedString() + " " + _currentRoundCount;
                Sequence roundSequence = DOTween.Sequence();
                roundSequence.Append(playerHUD.roundText.DOFade(1f, animDuration));
                roundSequence.AppendInterval(animDuration);
                roundSequence.Append(playerHUD.roundText.DOFade(0f, animDuration * 0.3f));
                Sequence roundScaleSequence = DOTween.Sequence();
                roundScaleSequence.AppendInterval(animDuration*2);
                roundScaleSequence.Append(playerHUD.roundText.rectTransform.DOScale(new Vector3(2.8675f, 2.8675f, 2.8675f), animDuration * .3f));
                roundScaleSequence.Append(playerHUD.roundText.rectTransform.DOScale(Vector3.one, 0f));
                roundSequence.Play();
                roundScaleSequence.Play();
            }
        }

        private void AnimateCountDownText(string text)
        {
            foreach (HUD playerHUD in playersHUD)
            {
                playerHUD.countDownText.text = text;
                Sequence countDownSequence = DOTween.Sequence();
                countDownSequence.Append(playerHUD.countDownText.DOFade(1f, 0.3f));
                countDownSequence.Append(playerHUD.countDownText.DOFade(0f, 0.3f));
                Sequence countDownScaleSequence = DOTween.Sequence();
                countDownScaleSequence.Append( playerHUD.countDownText.rectTransform.DOScale(new Vector3(2.8675f, 2.8675f, 2.8675f), .3f));
                countDownScaleSequence.Append(playerHUD.countDownText.rectTransform.DOScale(Vector3.one, .3f));
                countDownSequence.Play();
                countDownScaleSequence.Play();
            }
        }

        private void LaunchDolly()
        {
            dollyZoomer.timeZoom = dollyZoomDuration;
            dollyZoomer.doZoom = true;
            SoundManager.instance.PlayWind();
        }

        private void StopZoom()
        {
            dollyZoomer.doZoom = false;
            SoundManager.instance.StopWind();
        }

        private void SetCurrentRoundEmotionData()
        {
            int index = Random.Range(0, playableEmotions.Count);
            _currentRoundEmotionData = playableEmotions[index];
        }

        private void TestRoundWinner()
        {
       
            if (_isCurrentlySearchingForEmotion)
            {
                if (p1Emotion == _currentRoundEmotionData.TypeEmotion && p2Emotion == _currentRoundEmotionData.TypeEmotion)
                {
                    //Egality
                    RoundWinner(Winner.BOTH);
                }
                else if(p1Emotion == _currentRoundEmotionData.TypeEmotion)
                {
                    //P1 gagne roundText
                    RoundWinner(Winner.PLAYER_1);
                }
                else if(p2Emotion == _currentRoundEmotionData.TypeEmotion)
                {
                    //P2 gagne roundText
                    RoundWinner(Winner.PLAYER_2);
                }
            }
            else
            {
                if (!player1WrongEmotionDetected && p1Emotion != emotionToKeepOnCountdown.TypeEmotion)
                {
                    player1WrongEmotionDetected = true;
                    player1SaveTimeStart = DateTime.Now;
                }
                else if(p1Emotion == emotionToKeepOnCountdown.TypeEmotion)
                {
                    player1WrongEmotionDetected = false;
                }

                if (!player2WrongEmotionDetected && p2Emotion != emotionToKeepOnCountdown.TypeEmotion)
                {
                    player2WrongEmotionDetected = true;
                    player2SaveTimeStart = DateTime.Now;
                }  
                else if(p2Emotion == emotionToKeepOnCountdown.TypeEmotion)
                {
                    player2WrongEmotionDetected = false;
                }

                
                // First, stopping round and resetting
                if (( player1WrongEmotionDetected && DateTime.Now.Subtract(player1SaveTimeStart).TotalMilliseconds >= timeToDetectWrongEmotion ) || 
                    ( player2WrongEmotionDetected && DateTime.Now.Subtract(player2SaveTimeStart).TotalMilliseconds >= timeToDetectWrongEmotion))
                {
                    StopCoroutine(_roundCoroutine);
                    ResetHUD();
                    
                    if (player1WrongEmotionDetected && player2WrongEmotionDetected )
                    {
                        //Egality
                        RoundWinner(Winner.BOTH);
                    }
                    // if p1 is not neutral
                    else if(player1WrongEmotionDetected )
                    {
                        //P2 gagne roundText
                        RoundWinner(Winner.PLAYER_2);
                    }
                    // if p2 is not neutral
                    else if(player2WrongEmotionDetected )
                    {
                        //P1 gagne roundText
                        RoundWinner(Winner.PLAYER_1);
                    }
                }
                //If 2 players not neutrals, none wins
                
            }
        }

        private void RoundWinner(Winner winner)
        {
            SoundManager.instance.PlayShotgunSound();
            takeWebCamScreenShot.Raise();
            StopZoom();
            StopCoroutine(_roundTooLongCoroutine);
            _gameIsLaunched = false;
            _isCurrentlySearchingForEmotion = false;
            foreach (HUD hud in playersHUD)
            {
                HideCameraReturn(hud);
            }
            switch (winner)
            {
                case Winner.NONE:
                case Winner.BOTH:
                    Debug.Log("Both and nobody win");
                    drawWinRound.Raise();                
                    // onPlayer1Loose?.Invoke();
                    // player2winCount++;
                    playersHUD[1].roundResult.text = drawText.GetLocalizedString();
                    playersHUD[0].roundResult.text = drawText.GetLocalizedString();
                    break;
                case Winner.PLAYER_1:
                    Debug.Log("Player 1 win");
                    _player1Wins++;
                    player1WinRound.Raise();
                    // onPlayer2Loose?.Invoke();

                    playersHUD[1].roundResult.text = youWinText.GetLocalizedString();
                    playersHUD[0].roundResult.text = youLoseText.GetLocalizedString();
                    //Anim Star
                
                    var starPlayer = playersHUD[1].scorePlayer.transform.GetChild(_player1Wins - 1).GetComponent<Image>();
                    StarAnimation(starPlayer, playersHUD[1].animHandler.transform);

                    // starPlayer.DOFade(1f, animDuration).SetDelay(animDuration);

                    var starOpponent = playersHUD[0].scoreOpponent.transform.GetChild(_player1Wins - 1)
                        .GetComponent<Image>();
                    StarAnimation(starOpponent, playersHUD[0].animHandler.transform);

                    // starOpponent.DOFade(1f, animDuration).SetDelay(animDuration);
             
                    break;

                case Winner.PLAYER_2:
                    Debug.Log("Player 2 win");
                    _player2Wins++;
                    player2WinRound.Raise();

                    // onPlayer1Loose?.Invoke();
                    playersHUD[0].roundResult.text = youWinText.GetLocalizedString();
                    playersHUD[1].roundResult.text = youLoseText.GetLocalizedString();
                    //Anim Star
                    var starPlayer2 = playersHUD[0].scorePlayer.transform.GetChild(_player2Wins - 1).GetComponent<Image>();
                    // starPlayer2.DOFade(1f, animDuration).SetDelay(animDuration);
                    StarAnimation(starPlayer2, playersHUD[0].animHandler.transform);
                

                    var starOpponent2 = playersHUD[1].scoreOpponent.transform.GetChild(_player2Wins - 1)
                        .GetComponent<Image>();
                    StarAnimation(starOpponent2, playersHUD[1].animHandler.transform);

                    // starOpponent2.DOFade(1f, animDuration).SetDelay(animDuration);
               
                    break;

           
            }

            StartCoroutine(EndRoundCoroutine());


        }

        private void StarAnimation(Image star, Transform anchor)
        {
            Sequence starScaleSequence = DOTween.Sequence();
            starScaleSequence.Append(star.transform.DOScale(Vector3.zero, 0f));
            starScaleSequence.AppendInterval(animDuration);
            starScaleSequence.AppendInterval(animDuration- (animDuration*.3f));
            starScaleSequence.Append(star.transform.DOScale(new Vector3(2.8675f, 2.8675f, 2.8675f), animDuration*0.3f));
            starScaleSequence.Append(star.transform.DOScale(Vector3.one, animDuration*0.3f));
            Sequence starFadeSequence = DOTween.Sequence();
            starFadeSequence.AppendInterval(animDuration);
            starFadeSequence.Append(star.DOFade(1f, animDuration));
                
            Sequence starTransformSequence = DOTween.Sequence();
            starTransformSequence.Append(star.transform.DOLocalMove(anchor.transform
                .localPosition, 0f));
            starTransformSequence.AppendInterval(animDuration);
            starTransformSequence.AppendInterval(animDuration);
            starTransformSequence.AppendInterval(animDuration- (animDuration*.3f));
            starTransformSequence.Append(star.transform.DOLocalMove(star.transform.localPosition, 
                animDuration*0.3f));
            starFadeSequence.Play();
            starScaleSequence.Play();
            starTransformSequence.Play();
        }
        private IEnumerator EndRoundCoroutine()
        {
            // Animation
            foreach (HUD playerHUD in playersHUD)
            {
                HideNeutralScore(playerHUD);
                Sequence roundTextSequence = DOTween.Sequence();
                roundTextSequence.Append(playerHUD.roundResult.DOFade(1f, animDuration));
                roundTextSequence.Append(playerHUD.roundResult.DOFade(0f, animDuration * .3f));
                Sequence roundScaleSequence = DOTween.Sequence();
                roundScaleSequence.AppendInterval(animDuration);
                roundScaleSequence.Append( playerHUD.countDownText.rectTransform.DOScale(new Vector3(2.8675f, 2.8675f, 2.8675f), animDuration*.3f));
                roundScaleSequence.Append(playerHUD.roundResult.rectTransform.DOScale(Vector3.one, 0f));
                roundTextSequence.Play();
                roundScaleSequence.Play();
            }

            yield return new WaitForSeconds(5f);
            _currentRoundCount++;

            if (_player1Wins >= roundNeededToWin)
            {
                VictoryPlayer.instance.hasPlayer1Win = true;
                ScenesManager.instance.LoadScene(victoryScene);
                yield break;
            }

            if (_player2Wins >= roundNeededToWin)
            {
                VictoryPlayer.instance.hasPlayer1Win = false;
                ScenesManager.instance.LoadScene(victoryScene);
                yield break;
            }

            if (_currentRoundCount >= maxRoundCount)
            {
                ScenesManager.instance.LoadScene(victoryScene);
                yield break;
            }
            _roundCoroutine = StartCoroutine(StartOneRound());
        
        }
        private void Update()
        {
            p1Emotion = EmotionManager.instance.GetPlayer1Emotion();
            p2Emotion = EmotionManager.instance.GetPlayer2Emotion();
            if (_gameIsLaunched)
            {
                TestRoundWinner();
            }
        }


        public void ShowCameraReturn()
        {
            foreach (var playerHUD in playersHUD)
            {
                playerHUD.cameraReturn.DOFade(1f, animDuration);
                playerHUD.neutralityScore.DOFade(1f, animDuration);

            }

        }
    }
}
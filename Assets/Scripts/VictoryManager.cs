using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Serialization;

public class VictoryScene : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private VictoryUI victoryUIP1;
    [SerializeField] private VictoryUI victoryUIP2;
    [Header("Mesh")] 
    [SerializeField] private MeshRenderer winnerFace;
    [SerializeField] private MeshRenderer loserFace;
    
    [Header("Parameters")]
    [SerializeField] private LocalizedString victoryPrompt;
    [SerializeField] private LocalizedString loserPrompt;

    [FormerlySerializedAs("timerBeforeScreen")] [SerializeField] private int timerBeforePicture = 3;
    [SerializeField] private GameEvent screenShotEvent;
    
    private bool _pictureCountDownEngaged = false;
    private float _startTime;
    private float _currentTime;
    private float _endTime;
    // Start is called before the first frame update
    void Start()
    {
        InitUI();
    }

    public void InitUI()
    {
        victoryUIP1.ShowStartScreen();
        victoryUIP2.ShowStartScreen();
        if (VictoryPlayer.instance.hasPlayer1Win)
        {
            winnerFace.material.mainTexture =  WebcamManager.instance.Face1Texture;
            loserFace.material.mainTexture =  WebcamManager.instance.Face2Texture;
            victoryUIP1.InitUIMail(victoryPrompt.GetLocalizedString(), timerBeforePicture.ToString());
            victoryUIP2.InitUIMail(loserPrompt.GetLocalizedString(), timerBeforePicture.ToString());

        }
        else
        {
            winnerFace.material.mainTexture =  WebcamManager.instance.Face2Texture;
            loserFace.material.mainTexture =  WebcamManager.instance.Face1Texture;
            victoryUIP1.InitUIMail(loserPrompt.GetLocalizedString(), timerBeforePicture.ToString());
            victoryUIP2.InitUIMail(victoryPrompt.GetLocalizedString(), timerBeforePicture.ToString());
        }
        victoryUIP1.ShowStartScreen();
        victoryUIP2.ShowStartScreen();

    }

    public void StartPictureTaking()
    {
        _pictureCountDownEngaged = true;
        _startTime =  Time.time;
        _endTime =  _startTime + timerBeforePicture;
        victoryUIP2.ShowPictureScreen();
        victoryUIP1.ShowPictureScreen();
    }
    // Update is called once per frame
    private void Update()
    {
        if(!_pictureCountDownEngaged)return;
        _currentTime = Time.time;
        int remainingTime = (int)Mathf.Round(_endTime - _currentTime);
        victoryUIP1.UpdateTimer(remainingTime.ToString());
        victoryUIP2.UpdateTimer(remainingTime.ToString());
        if (remainingTime <= 0)
        {
            _pictureCountDownEngaged = false;
            victoryUIP1.ShowCommonScreen();
            victoryUIP2.ShowCommonScreen();
            screenShotEvent.Raise();
            StartCoroutine(PictureTaken());
        }
    }

    private IEnumerator PictureTaken()
    {
        yield return new WaitForSeconds(1f);
        victoryUIP1.ShowEmailScreen();
        victoryUIP2.ShowEmailScreen();
    }
    
}
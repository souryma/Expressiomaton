using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] private string victoryPrompt = "Do your winner face !";
    [SerializeField] private string loserPrompt = "Do your loser face !";

    [SerializeField] private int timerBeforePicturePrompt = 3;
    [FormerlySerializedAs("timerBeforeScreen")] [SerializeField] private int timerBeforePicture = 3;
    [SerializeField] private GameEvent screenShotEvent;
    
    private bool _pictureCountDownEngaged = false;
    private float _startTime;
    private float _currentTime;
    private float _endTime;
    // Start is called before the first frame update
    void Start()
    {
        InitUI(true);
    }

    public void InitUI(bool isP1Winner)
    {
        victoryUIP1.StartScreen();
        victoryUIP2.StartScreen();
        if (isP1Winner)
        {
            winnerFace.material.mainTexture =  WebcamManager.instance.Face1Texture;
            loserFace.material.mainTexture =  WebcamManager.instance.Face2Texture;
           
        }
        else
        {
            winnerFace.material.mainTexture =  WebcamManager.instance.Face2Texture;
            loserFace.material.mainTexture =  WebcamManager.instance.Face1Texture;
        }

        StartCoroutine(AfterPrompt(isP1Winner));
    }

    private IEnumerator AfterPrompt(bool isP1Winner)
    {
       
        
        yield return new WaitForSeconds(timerBeforePicturePrompt);
        if (isP1Winner)
        {
            victoryUIP1.InitTakePicture(victoryPrompt, timerBeforePicture.ToString());
            victoryUIP2.InitTakePicture(loserPrompt, timerBeforePicture.ToString());
           
        }
        else
        {
            victoryUIP1.InitTakePicture(loserPrompt, timerBeforePicture.ToString());
            victoryUIP2.InitTakePicture(victoryPrompt, timerBeforePicture.ToString());
        }
      
        _pictureCountDownEngaged = true;
        _startTime =  Time.time;
        _endTime =  _startTime + timerBeforePicture;
    }
    // Update is called once per frame
    void Update()
    {
        if(!_pictureCountDownEngaged)return;
        _currentTime = Time.time;
        int remainingTime = (int)Mathf.Round(_endTime - _currentTime);
        victoryUIP1.UpdateTimer(remainingTime.ToString());
        victoryUIP2.UpdateTimer(remainingTime.ToString());
        if (remainingTime <= 0)
        {
            _pictureCountDownEngaged = false;
            victoryUIP1.HideOverlay();
            victoryUIP2.HideOverlay();
            screenShotEvent.Raise();
            StartCoroutine(PictureTaken());
        }
    }

    private IEnumerator PictureTaken()
    {
        yield return new WaitForSeconds(1.5f);
        victoryUIP1.SwitchToEmailScreen();
        victoryUIP2.SwitchToEmailScreen();
    }
    
}
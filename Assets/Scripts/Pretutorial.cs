using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pretutorial : MonoBehaviour
{
    [SerializeField] private RawImage _player1Camera;
    [SerializeField] private RawImage _player1Face;
    [SerializeField] private GameObject _player1ButtonReady;
    [SerializeField] private GameObject _player1Text;
    [SerializeField] private GameObject _player1WantedPoster;

    // Start is called before the first frame update
    void Start()
    {
        _player1ButtonReady.SetActive(false);
        
        _player1Camera.texture = WebcamManager.instance.Webcam1;
        
        // Detect when face is on camera
        Player1Ready();
    }
    
    private void Player1Ready()
    {
        _player1Text.SetActive(false);
        _player1WantedPoster.SetActive(true);
        _player1ButtonReady.SetActive(true);
        _player1Face.texture = WebcamManager.instance.Face1Texture;
    }
}
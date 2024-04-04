using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GameAnimation : MonoBehaviour
{
    public static GameAnimation Instance;
    [SerializeField] private GameObject Player1;
    [SerializeField] private GameObject Player2;
    // Start is called before the first frame update
    public event Action reinitialise;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        // RoundManager.Instance.onPlayer1Loose += killEventPlayer1;
        // RoundManager.Instance.onPlayer2Loose += killEventPlayer2;
    }

    public void killEventPlayer1()
    {
        StartCoroutine(killPlayer1());
    }
    
    public void killEventPlayer2()
    {
        StartCoroutine(killPlayer2());
    }
    
    public void killNonePlayers()
    {
        StartCoroutine(killNonePlayer());
    }

    private IEnumerator killNonePlayer()
    {
        // Fonction pour faire tirer le joueur 2 et tuer le joueur 1
        GameObject playerModel = Player1.transform.Find("PlayerModel").gameObject;
        // GameObject playerParentCam = Player1.transform.Find("ParentCam").gameObject;
        GameObject playerShootingModel = Player1.transform.Find("PlayerShootingModel").gameObject;
        GameObject playerModelLoosing = Player2.transform.Find("PlayerModel").gameObject;
        GameObject playerDeadModel = Player2.transform.Find("PlayerShootingModel").gameObject;
        playerModel.SetActive(false);
        playerShootingModel.SetActive(true);
        playerModelLoosing.SetActive(false);
        playerDeadModel.SetActive(true);
        // playerParentCam.transform.rotation = Quaternion.Euler(5f, 5f, -77f);

        yield return new WaitForSeconds(5);
        reinitialiseModel();
        reinitialise?.Invoke();
    }

    private IEnumerator killPlayer1()
    {
        // Fonction pour faire tirer le joueur 2 et tuer le joueur 1
        GameObject playerModel = Player1.transform.Find("PlayerModel").gameObject;
        GameObject playerParentCam = Player1.transform.Find("ParentCamP2").gameObject;
        GameObject playerShootingModel = Player1.transform.Find("PlayerShootingModel").gameObject;
        GameObject playerModelLoosing = Player2.transform.Find("PlayerModel").gameObject;
        GameObject playerDeadModel = Player2.transform.Find("PlayerDeadModel").gameObject;
        playerModel.SetActive(false);
        playerShootingModel.SetActive(true);
        playerModelLoosing.SetActive(false);
        playerDeadModel.SetActive(true);
        playerParentCam.transform.Rotate(new Vector3(0f, 0f, -88f));
        // playerParentCam.transform.Translate(0f, .6f, 0f);
        // // playerParentCam.transform.rotation =  Quaternion.Euler(0f, 0f, -88f);
        var position = playerParentCam.transform.localPosition;
        position = new Vector3(position.x, -1.9f, position.z);
        playerParentCam.transform.localPosition =  position;

        yield return new WaitForSeconds(5);
        reinitialiseModel();
        reinitialise?.Invoke();
    }

    private IEnumerator killPlayer2()
    {
        // Fonction pour faire tirer le joueur 1 et tuer le joueur 2
        GameObject playerModelWinner = Player2.transform.Find("PlayerModel").gameObject;
        GameObject playerParentCam = Player2.transform.Find("ParentCamP1").gameObject;
        GameObject playerShootingModel = Player2.transform.Find("PlayerShootingModel").gameObject;
        GameObject playerModelLoosing = Player1.transform.Find("PlayerModel").gameObject;
        GameObject playerDeadModel = Player1.transform.Find("PlayerDeadModel").gameObject;
        playerModelWinner.SetActive(false);
        playerShootingModel.SetActive(true);
        playerModelLoosing.SetActive(false);
        playerDeadModel.SetActive(true);
        playerParentCam.transform.Rotate(new Vector3(0f, 0f, -88f));
        
        var position = playerParentCam.transform.localPosition;
        position = new Vector3(position.x, -1.9f, position.z);
        playerParentCam.transform.localPosition =  position;
        // playerParentCam.transform.Translate(0f, .6f, 0f);
        // // playerParentCam.transform.rotation = 
        // var position = playerParentCam.transform.position;
        // // position = ;
        // playerParentCam.transform.position = new Vector3(0, .6f,
        //     0);

        yield return new WaitForSeconds(5f);
        reinitialiseModel();
        reinitialise?.Invoke();
    }

    private void reinitialiseModel()
    {
        GameObject player1Model = Player1.transform.Find("PlayerModel").gameObject;
        GameObject player1ShootingModel = Player1.transform.Find("PlayerShootingModel").gameObject;
        GameObject player1ParentCam = Player1.transform.Find("ParentCamP2").gameObject;
        GameObject player1DeadModel = Player1.transform.Find("PlayerDeadModel").gameObject;
        
        GameObject player2Model = Player2.transform.Find("PlayerModel").gameObject;
        GameObject player2ShootingModel = Player2.transform.Find("PlayerShootingModel").gameObject;
        GameObject player2ParentCam = Player2.transform.Find("ParentCamP1").gameObject;
        GameObject player2DeadModel = Player2.transform.Find("PlayerDeadModel").gameObject;
        
        player1Model.SetActive(true);
        player1ShootingModel.SetActive(false);
        player1DeadModel.SetActive(false);
        // player1ParentCam.transform.Rotate(new Vector3(0f, 0f, 88f));

        player1ParentCam.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        
        var position = player1ParentCam.transform.localPosition;
        position = new Vector3(position.x, -2.5f, position.z);
        player1ParentCam.transform.localPosition =  position;
        // var position = player1ParentCam.transform.position;
        // // position =
        // player1ParentCam.transform.position =  new Vector3(0, -.6f,
        //     0);

        player2Model.SetActive(true);
        player2ShootingModel.SetActive(false);
        player2DeadModel.SetActive(false);
        // player2ParentCam.transform.Rotate(new Vector3(0f, 0f, 88f));
        player2ParentCam.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        var position2 = player2ParentCam.transform.localPosition;
        position2 = new Vector3(position2.x, -2.5f, position2.z);
        player2ParentCam.transform.localPosition =  position2;
        // var position2 = player2ParentCam.transform.position;
        // // position2 = ;
        // player2ParentCam.transform.position = new Vector3(0, -.6f,
        //     0);
    }
}

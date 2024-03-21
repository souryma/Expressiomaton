using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using OpenCvSharp.Util;
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
        RoundManager.Instance.onPlayer1Loose += killEventPlayer1;
        RoundManager.Instance.onPlayer2Loose += killEventPlayer2;
    }

    private void killEventPlayer1()
    {
        StartCoroutine(killPlayer1());
    }
    
    private void killEventPlayer2()
    {
        StartCoroutine(killPlayer2());
    }

    public IEnumerator killPlayer1()
    {
        GameObject playerModel = Player1.transform.Find("PlayerModel").gameObject;
        GameObject playerParentCam = Player1.transform.Find("ParentCam").gameObject;
        GameObject playerShootingModel = Player1.transform.Find("PlayerShootingModel").gameObject;
        GameObject playerModelLoosing = Player2.transform.Find("PlayerModel").gameObject;
        GameObject playerDeadModel = Player2.transform.Find("PlayerDeadModel").gameObject;
        playerModel.SetActive(false);
        playerShootingModel.SetActive(true);
        playerModelLoosing.SetActive(false);
        playerDeadModel.SetActive(true);
        playerParentCam.transform.rotation = new quaternion(0f,0f,-77f,0f);

        yield return new WaitForSeconds(5);
        reinitialiseModel();
        reinitialise?.Invoke();
    }
    
    public IEnumerator killPlayer2()
    {
        GameObject playerModelWinner = Player2.transform.Find("PlayerModel").gameObject;
        GameObject playerParentCam = Player2.transform.Find("ParentCam").gameObject;
        GameObject playerShootingModel = Player2.transform.Find("PlayerShootingModel").gameObject;
        GameObject playerModelLoosing = Player1.transform.Find("PlayerModel").gameObject;
        GameObject playerDeadModel = Player1.transform.Find("PlayerDeadModel").gameObject;
        playerModelWinner.SetActive(false);
        playerShootingModel.SetActive(true);
        playerModelLoosing.SetActive(false);
        playerDeadModel.SetActive(true);
        
        playerParentCam.transform.rotation = new quaternion(0f, 0f, -77f, 0f);

        yield return new WaitForSeconds(5);
        reinitialiseModel();
        reinitialise?.Invoke();
    }
    
    public void reinitialiseModel()
    {
        GameObject player1Model = Player1.transform.Find("PlayerModel").gameObject;
        GameObject player1ShootingModel = Player1.transform.Find("PlayerShootingModel").gameObject;
        GameObject player1ParentCam = Player1.transform.Find("ParentCam").gameObject;
        GameObject player1DeadModel = Player1.transform.Find("PlayerDeadModel").gameObject;
        
        GameObject player2Model = Player2.transform.Find("PlayerModel").gameObject;
        GameObject player2ShootingModel = Player2.transform.Find("PlayerShootingModel").gameObject;
        GameObject player2ParentCam = Player2.transform.Find("ParentCam").gameObject;
        GameObject player2DeadModel = Player1.transform.Find("PlayerDeadModel").gameObject;
        
        player1Model.SetActive(true);
        player1ShootingModel.SetActive(false);
        player1DeadModel.SetActive(false);
        player1ParentCam.transform.rotation = new quaternion(0f,0f,0f,0f);
        
        player2Model.SetActive(true);
        player2ShootingModel.SetActive(false);
        player2DeadModel.SetActive(false);
        player2ParentCam.transform.rotation = new quaternion(0f,0f,0f,0f);
    }
}

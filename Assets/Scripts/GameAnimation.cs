using System.Collections;
using System.Collections.Generic;
using OpenCvSharp.Util;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GameAnimation : MonoBehaviour
{
    [SerializeField] private GameObject Player1;
    [SerializeField] private GameObject Player2;
    
    private 
    // Start is called before the first frame update
    void Awake()
    {
        RoundManager.Instance.onPlayer1Loose += killPlayer1;
        RoundManager.Instance.onPlayer2Loose += killPlayer2;
    }
    void Start()
    {
        
    }


    public void killPlayer1()
    {
        GameObject playerModel = Player1.transform.Find("PlayerModel").gameObject;
        GameObject playerParentCam = Player1.transform.Find("ParentCam").gameObject;
        GameObject playerShootingModel = Player1.transform.Find("PlayerShootingModel").gameObject;
        playerModel.SetActive(false);
        playerShootingModel.SetActive(true);
        playerParentCam.transform.rotation = new quaternion(0f,0f,-77f,0f);
    }
    
    public void killPlayer2()
    {
        GameObject playerModel = Player2.transform.Find("PlayerModel").gameObject;
        GameObject playerParentCam = Player2.transform.Find("ParentCam").gameObject;
        GameObject playerShootingModel = Player2.transform.Find("PlayerShootingModel").gameObject;
        playerModel.SetActive(false);
        playerShootingModel.SetActive(true);
        playerParentCam.transform.rotation = new quaternion(0f, 0f, -77f, 0f);
    }
    
    public void reinitilise()
    {
        GameObject player1Model = Player1.transform.Find("PlayerModel").gameObject;
        GameObject player1ShootingModel = Player1.transform.Find("PlayerShootingModel").gameObject;
        GameObject player1ParentCam = Player1.transform.Find("ParentCam").gameObject;
        
        GameObject player2Model = Player2.transform.Find("PlayerModel").gameObject;
        GameObject player2ShootingModel = Player2.transform.Find("PlayerShootingModel").gameObject;
        GameObject player2ParentCam = Player2.transform.Find("ParentCam").gameObject;
        
        player1Model.SetActive(false);
        player1ShootingModel.SetActive(true);
        player1ParentCam.transform.rotation = new quaternion(0f,0f,-77f,0f);
        
        player2Model.SetActive(false);
        player2ShootingModel.SetActive(true);
        player2ParentCam.transform.rotation = new quaternion(0f,0f,-77f,0f);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}

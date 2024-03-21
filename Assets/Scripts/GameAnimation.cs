using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameAnimation : MonoBehaviour
{
    [SerializeField] private GameObject Player1;
    [SerializeField] private GameObject Player2;
    // Start is called before the first frame update
    void Awake()
    {
        
    }
    void Start()
    {
        
    }


    public void killPlayer(GameObject deadPlayer, GameObject killerPlayer)
    {
        GameObject player2Model = deadPlayer.transform.Find("PlayerModel").gameObject;
        GameObject player2ParentCam = deadPlayer.transform.Find("ParentCam").gameObject;
        GameObject player2ShootingModel = deadPlayer.transform.Find("PlayerShootingModel").gameObject;
        
        player2Model.SetActive(false);
        //player2ParentCam.Set
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}

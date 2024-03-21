using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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


    public void killPlayer(GameObject deadPlayer)
    {
        GameObject playerModel = deadPlayer.transform.Find("PlayerModel").gameObject;
        GameObject playerParentCam = deadPlayer.transform.Find("ParentCam").gameObject;
        GameObject playerShootingModel = deadPlayer.transform.Find("PlayerShootingModel").gameObject;
        
        playerModel.SetActive(false);
        playerParentCam.SetActive(true);
        playerShootingModel.transform.rotation = new quaternion(0f,0f,-77f,0f);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}

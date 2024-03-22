using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryPlayer : MonoBehaviour
{
    public static VictoryPlayer instance;

    public bool hasPlayer1Win = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    
    
}

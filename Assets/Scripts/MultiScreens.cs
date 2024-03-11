using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiScreens : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (var display in Display.displays)
        {
            display.Activate(1920, 1080, Screen.currentResolution.refreshRateRatio);
        }
    }
}

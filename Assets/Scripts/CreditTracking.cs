using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
    
public class CreditTracking : MonoBehaviour
{
    [SerializeField] private GameObject Objectif;
    // Start is called before the first frame update
    public void DoTracking()
    {
        this.GameObject().transform.Translate(1 * (-Vector3.forward) * Time.deltaTime);
    }

    private void Update()
    {
        if (Vector3.Distance(this.GameObject().transform.position, Objectif.transform.position) <= 19f)
        {
            DoTracking();
        }
        else
        {
            ScenesManager.instance.LoadScene("MenuScene");
        }
    }
}

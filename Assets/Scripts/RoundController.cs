using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoundController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Random emotion : " + GetRandomEmotion());
        StartCoroutine(Countdown(3, () => Debug.Log("Hey")));
    }

    // Update is called once per frame
    void Update()
    {
    }

    private IEnumerator Countdown(int seconds, Action actionOnComplete)
    {
        for (int i = seconds; i >= 0; i--)
        {
            Debug.Log("Counter : " + i);
            yield return new WaitForSeconds(1);
        }
        Debug.Log("Go !!");
        actionOnComplete();
    }

    private Emotion GetRandomEmotion()
    {
        return (Emotion)Random.Range(0, System.Enum.GetNames(typeof(Emotion)).Length);
    }
}

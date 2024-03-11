using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    public TextMeshProUGUI player2Text;

    private void Start()
    {
        StartCoroutine(Player2TextAnimation());
    }

    private string _basicText = "Waiting for player 1";

    public void GameAboutToStart()
    {
        _basicText = "The game is about to start";
    }

    private IEnumerator Player2TextAnimation()
    {
        player2Text.text = _basicText;
        yield return new WaitForSeconds(1);
        player2Text.text = _basicText + ".";
        yield return new WaitForSeconds(1);
        player2Text.text = _basicText + "..";
        yield return new WaitForSeconds(1);
        player2Text.text = _basicText + "...";
        yield return new WaitForSeconds(1);

        StartCoroutine(Player2TextAnimation());
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

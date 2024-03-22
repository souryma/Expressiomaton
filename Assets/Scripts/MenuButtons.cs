using System.Collections;
using TMPro;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    public string TutorialScene = "";
    
    public TextMeshProUGUI player2WaitingText;
    public TextMeshProUGUI player2AboutToStartText;

    public GameObject _player2Rules1;
    public GameObject _player2Rules2;
    public GameObject _player2Rules3;

    private void Start()
    {
        SoundManager.instance.PlayMenuSound();

        player2AboutToStartText.gameObject.SetActive(false);
        StartCoroutine(Player2TextAnimation(player2WaitingText, "Waiting for player 1"));
    }

    public void GameAboutToStart()
    {
        player2AboutToStartText.gameObject.SetActive(true);
        player2WaitingText.gameObject.SetActive(false);

        StartCoroutine(Player2TextAnimation(player2AboutToStartText, "The game is about to start"));
    }

    private IEnumerator Player2TextAnimation(TextMeshProUGUI textGui, string text)
    {
        textGui.text = text;
        yield return new WaitForSeconds(1);
        textGui.text = text + ".";
        yield return new WaitForSeconds(1);
        textGui.text = text + "..";
        yield return new WaitForSeconds(1);
        textGui.text = text + "...";
        yield return new WaitForSeconds(1);

        StartCoroutine(Player2TextAnimation(textGui, text));
    }

    public void ConfirmCameraSelection()
    {
        SoundManager.instance.PlayShotgunSound();
        WebcamManager.instance.ConfirmCameraSelection();
    }

    public void PlayWithTutorial()
    {
        SoundManager.instance.PlayShotgunSound();
        ScenesManager.instance.LoadScene(TutorialScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void DisplayRules1()
    {
        SoundManager.instance.PlayShotgunSound();
        _player2Rules1.SetActive(true);
        player2WaitingText.gameObject.SetActive(false);
    }
    
    public void DisplayRules2()
    {
        SoundManager.instance.PlayShotgunSound();
        _player2Rules1.SetActive(false);
        _player2Rules2.SetActive(true);
        player2WaitingText.gameObject.SetActive(false);
    }
    
    public void DisplayRules3()
    {
        SoundManager.instance.PlayShotgunSound();
        _player2Rules2.SetActive(false);
        _player2Rules3.SetActive(true);
        player2WaitingText.gameObject.SetActive(false);
    }
    
    public void BackToMenu()
    {
        SoundManager.instance.PlayShotgunSound();
        _player2Rules1.SetActive(false);
        _player2Rules2.SetActive(false);
        _player2Rules3.SetActive(false);
        player2WaitingText.gameObject.SetActive(true);
    }

    public void loadCreditScene()
    {
        SoundManager.instance.PlayShotgunSound();
        ScenesManager.instance.LoadScene("Credits");
    }
}

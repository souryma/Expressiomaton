using System.Collections;
using TMPro;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    public string TutorialScene = "";
    public string NoTutorialScene = "";
    
    public TextMeshProUGUI player2WaitingText;
    public TextMeshProUGUI player2AboutToStartText;

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
    
    public void PlayWithoutTutorial()
    {
        SoundManager.instance.PlayShotgunSound();
        ScenesManager.instance.LoadScene(NoTutorialScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

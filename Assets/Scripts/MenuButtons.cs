using System.Collections;
using TMPro;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    public string TutorialScene = "";
    public string NoTutorialScene = "";
    
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

    public void ConfirmCameraSelection()
    {
        WebcamManager.instance.ConfirmCameraSelection();
    }

    public void PlayWithTutorial()
    {
        ScenesManager.instance.LoadScene(TutorialScene);
    }
    
    public void PlayWithoutTutorial()
    {
        ScenesManager.instance.LoadScene(NoTutorialScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

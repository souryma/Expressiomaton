using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Pretutorial : MonoBehaviour
{
    [SerializeField] private RawImage _player1Camera;
    [SerializeField] private RawImage _player1Face;
    [SerializeField] private GameObject _player1Text;
    [SerializeField] private GameObject _player1WantedPoster;
    [SerializeField] private TextMeshProUGUI _launchText1;

    [SerializeField] private RawImage _player2Camera;
    [SerializeField] private RawImage _player2Face;
    [SerializeField] private GameObject _player2Text;
    [SerializeField] private GameObject _player2WantedPoster;
    [SerializeField] private TextMeshProUGUI _launchText2;

    private bool _player1ready = false;
    private bool _player2ready = false;

    [SerializeField] private string nextSceneName = "";

    // Start is called before the first frame update
    void Start()
    {
        _player1Camera.texture = WebcamManager.instance.Webcam1;
        _player2Camera.texture = WebcamManager.instance.Webcam2;

        _launchText1.gameObject.SetActive(false);
        _launchText2.gameObject.SetActive(false);
    }

    private bool _launchStarted = false;

    private void Update()
    {
        if (WebcamManager.instance.DoesCamera1DetectFace() && _launchStarted == false)
            Player1Ready();

        if (WebcamManager.instance.DoesCamera2DetectFace() && _launchStarted == false)
            Player2Ready();

        if (_player1ready && _player2ready && _launchStarted == false)
        {
            StartCoroutine(LaunchGame());
            _launchStarted = true;
        }
    }

    private IEnumerator LaunchGame()
    {
        string text = "The game will start in 5";
        _launchText1.text = text;
        _launchText2.text = text;
        yield return new WaitForSecondsRealtime(1);

        text = "The game will start in 4";
        _launchText1.text = text;
        _launchText2.text = text;
        yield return new WaitForSecondsRealtime(1);

        text = "The game will start in 3";
        _launchText1.text = text;
        _launchText2.text = text;
        yield return new WaitForSecondsRealtime(1);

        text = "The game will start in 2";
        _launchText1.text = text;
        _launchText2.text = text;
        yield return new WaitForSecondsRealtime(1);

        text = "The game will start in 1";
        _launchText1.text = text;
        _launchText2.text = text;
        yield return new WaitForSecondsRealtime(1);

        ScenesManager.instance.LoadScene(nextSceneName);
    }

    private void Player1Ready()
    {
        _player1Text.SetActive(false);
        _player1WantedPoster.SetActive(true);
        _player1Face.texture = WebcamManager.instance.Face1Texture;

        _player1Camera.gameObject.SetActive(false);
        _launchText1.gameObject.SetActive(true);
        _launchText1.text = "Waiting for player 2";

        _player1ready = true;
    }

    private void Player2Ready()
    {
        _player2Text.SetActive(false);
        _player2WantedPoster.SetActive(true);
        _player2Face.texture = WebcamManager.instance.Face2Texture;

        _player2Camera.gameObject.SetActive(false);
        _launchText2.gameObject.SetActive(true);
        _launchText2.text = "Waiting for player 1";

        _player2ready = true;
    }
}
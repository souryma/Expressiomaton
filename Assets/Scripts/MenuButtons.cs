using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class MenuButtons : MonoBehaviour
{
    public string TutorialScene = "";
    
    public LocalizedString waitingP1;
    public LocalizedString gameStarting;
    public GameObject _player2Rules1;
    public GameObject _player2Rules2;
    public GameObject _player2Rules3;

    private void Start()
    {
        SoundManager.instance.PlayMenuSound();

    }

    public void GameAboutToStart()
    {

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
    }
    
    public void DisplayRules2()
    {
        SoundManager.instance.PlayShotgunSound();
        _player2Rules1.SetActive(false);
        _player2Rules2.SetActive(true);
    }
    
    public void DisplayRules3()
    {
        SoundManager.instance.PlayShotgunSound();
        _player2Rules2.SetActive(false);
        _player2Rules3.SetActive(true);
    }
    
    public void BackToMenu()
    {
        SoundManager.instance.PlayShotgunSound();
        _player2Rules1.SetActive(false);
        _player2Rules2.SetActive(false);
        _player2Rules3.SetActive(false);
    }

    public void ChangeLanguageToFrench()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
    }

    public void ChangeLanguageToEnglish()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
    }

    public void loadCreditScene()
    {
        SoundManager.instance.PlayShotgunSound();
        ScenesManager.instance.LoadScene("Credits");
    }
}

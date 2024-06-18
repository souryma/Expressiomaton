using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    public string TutorialScene = "";
    
    public LocalizedString waitingP1;
    public LocalizedString gameStarting;
    public GameObject _player2Rules1;
    public GameObject _player2Rules2;
    public GameObject _player2Rules3;

    [SerializeField] private Button frenchButton;
    [SerializeField]private Button englishButton;
    private void Start()
    {
        SoundManager.instance.PlayMenuSound();
        if (LocalizationSettings.Instance.GetSelectedLocale() == LocalizationSettings.AvailableLocales.Locales[0])
        {
            englishButton.interactable = false;
            frenchButton.interactable = true;
        }
        else
        {
            englishButton.interactable = true;
            frenchButton.interactable = false;
        }
        frenchButton.onClick.AddListener(ChangeLanguageToFrench);
        englishButton.onClick.AddListener(ChangeLanguageToEnglish);
    }

    private void OnDestroy()
    {
        frenchButton.onClick.RemoveListener(ChangeLanguageToFrench);
        englishButton.onClick.RemoveListener(ChangeLanguageToEnglish);
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
        englishButton.interactable = true;
        frenchButton.interactable = false;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
    }

    public void ChangeLanguageToEnglish()
    {
        englishButton.interactable = false;
        frenchButton.interactable = true;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
    }

    public void loadCreditScene()
    {
        SoundManager.instance.PlayShotgunSound();
        ScenesManager.instance.LoadScene("Credits");
    }
}

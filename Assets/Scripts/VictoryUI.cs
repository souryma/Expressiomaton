using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
    [Header("Start Screen")]
    [SerializeField] private CanvasGroup startScreen;
    [Header("Take Picture Screen")]
    [SerializeField] private CanvasGroup takePictureScreen;
    [SerializeField] private TMP_Text promptFaceField;
    [SerializeField] private TMP_Text timerField;
    [Header("Send Email Screen")]
    [SerializeField] private CanvasGroup emailScreen;
    [SerializeField] private bool hasInput;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_Text warningField;
    [SerializeField] private TMP_Text emailSendingField;
    [SerializeField] private TMP_Text emailSentField;
    [SerializeField] private Button sendEmailButton;
    [SerializeField] private Button passButton;
    [SerializeField] private EmailGameEvent emailGameEvent;

    private bool _emailIsSending = false;
    private void Start()
    {
        if (!hasInput) return;
        sendEmailButton.onClick.AddListener(OnSendEmail);
        passButton.onClick.AddListener(OnPass);
    }

    public void UpdateTimer(string number)
    {
        timerField.text = number;
    }

    public void StartScreen()
    {
        HideCanvas(takePictureScreen);
        HideCanvas(emailScreen);
        ShowCanvas(startScreen);
    }

    // Start is called before the first frame update
    public void InitTakePicture(string promptText, string timerStart)
    {
        HideCanvas(startScreen);
        HideCanvas(emailScreen);
        ShowCanvas(takePictureScreen);
        promptFaceField.text = promptText;
        timerField.text = timerStart;
    }

    public void HideOverlay()
    {
        HideCanvas(takePictureScreen);
        HideCanvas(emailScreen);
        HideCanvas(startScreen);
    }

    public void SwitchToEmailScreen()
    {
        HideCanvas(startScreen);
        HideCanvas(takePictureScreen);
        ShowCanvas(emailScreen);
    }

    private void OnSendEmail()
    {
        if (!_emailIsSending && EmailGameEvent.IsValid(emailInput.text))
        {
            _emailIsSending = true;
            ShowEmailSending();
            emailGameEvent.RaiseEvent(emailInput.text);
        }
        else
        {
            ShowWarning();
        }
    }

    private void HideCanvas(CanvasGroup canvas)
    {
        canvas.alpha = 0;
        canvas.interactable = false;
    }

    private void ShowCanvas(CanvasGroup canvas)
    {
        canvas.alpha = 1;
        canvas.interactable = true;
    }

    private void ShowWarning()
    {
        warningField.alpha = 1;
        emailSendingField.alpha = 0;
        emailSentField.alpha = 0;
    }

    private void ShowEmailSending()
    {
        warningField.alpha = 0;
        emailSendingField.alpha = 1;
        emailSentField.alpha = 0;
    }

    private void ShowEmailSent()
    {
        warningField.alpha = 0;
        emailSendingField.alpha = 0;
        emailSentField.alpha = 1;
    }

    private void ShowNone()
    {
        warningField.alpha = 0;
        emailSendingField.alpha = 0;
        emailSentField.alpha = 0;
    }
    public void EmailGotThrough()
    {
        _emailIsSending = false;
        ShowEmailSent();
    }
    private void OnPass()
    {
        ScenesManager.instance.LoadScene("MenuScene");
    }
    
    
}

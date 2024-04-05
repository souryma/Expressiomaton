using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VictoryUI : MonoBehaviour
{
    [SerializeField] private bool hasInput;

    [Header("Common Screen")]
    [SerializeField] private CanvasGroup commonScreen;

    [Header("Start Screen")]
    [SerializeField] private CanvasGroup startScreen;
    [SerializeField] private Button startPictureScreen;
    [SerializeField] private GameEvent onStartPictureScreen;

    [Header("Take Picture Screen")]
    [SerializeField] private CanvasGroup takePictureScreen;
    [SerializeField] private TMP_Text promptFaceField;
    [SerializeField] private TMP_Text timerField;
    [Header("Send Email Screen")]
    [SerializeField] private CanvasGroup emailScreen;
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
        HideOverlay();
        sendEmailButton.onClick.AddListener(OnSendEmail);
        passButton.onClick.AddListener(OnPass);
        startPictureScreen.onClick.AddListener(OnPictureStart);
        StartCoroutine(StartScreen());
    }

    public void UpdateTimer(string number)
    {
        timerField.text = number;
    }

    public void ShowStartScreen()
    {
        HideCanvas(commonScreen);
        ShowCanvas(startScreen);
        HideCanvas(takePictureScreen);
        HideCanvas(emailScreen);
        
    }

    private IEnumerator StartScreen()
    {
        yield return new WaitForSeconds(3f);
        OnPictureStart();
    }

    public void ShowPictureScreen()
    {
        HideCanvas(commonScreen);
        HideCanvas(startScreen);
        ShowCanvas(takePictureScreen);
        HideCanvas(emailScreen);
    }

    public void ShowEmailScreen()
    {
        HideCanvas(commonScreen);
        HideCanvas(startScreen);
        HideCanvas(takePictureScreen);
        ShowCanvas(emailScreen);
    }
    public void HideOverlay()
    {
        HideCanvas(commonScreen);
        HideCanvas(startScreen);
        HideCanvas(takePictureScreen);
        HideCanvas(emailScreen);
    }

    public void ShowCommonScreen()
    {
        ShowCanvas(commonScreen);
        HideCanvas(startScreen);
        HideCanvas(takePictureScreen);
        HideCanvas(emailScreen);
    }
    // Start is called before the first frame update
    public void InitUIMail(string promptText, string timerStart)
    {
        promptFaceField.text = promptText;
        timerField.text = timerStart;
    }
    

    private void OnPictureStart()
    {
        onStartPictureScreen.Raise();
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
        canvas.blocksRaycasts = false;
    }

    private void ShowCanvas(CanvasGroup canvas)
    {
        canvas.alpha = 1;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
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

    private void ShowEmailNone()
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

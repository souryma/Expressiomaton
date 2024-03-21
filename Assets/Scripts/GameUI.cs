using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Round Screen")]
    [SerializeField] private CanvasGroup roundScreen;
    [SerializeField] private TMP_Text roundPromptField;
    [SerializeField] private string roundText;
    [Header("Emotion Screen")]
    [SerializeField] private CanvasGroup emotionScreen;
    [SerializeField] private TMP_Text promptEmotionField;
    [SerializeField] private Image emotionImageField;
    [Header("Score Screen")]
    [SerializeField] private List<Image> scorePlayerField;
    [SerializeField] private List<Image> scoreOpponentField;
    // Start is called before the first frame update
    private void Start()
    {
        foreach (var image in scoreOpponentField)
        {
            image.color = new Color(1f, 1f, 1f, 0f);

        }

        foreach (var image in scorePlayerField)
        {
            image.color = new Color(1f, 1f, 1f, 0f);
        }
        HideCanvas(roundScreen);
        HideCanvas(emotionScreen);
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

    public void ShowRound(int roundNumber)
    {
        roundPromptField.text = roundText + " " +roundNumber;
        ShowCanvas(roundScreen);
    }

    public void HideRound()
    {
        HideCanvas(roundScreen);
    }
    
    

    // Update is called once per frame
    void Update()
    {
        
    }
}

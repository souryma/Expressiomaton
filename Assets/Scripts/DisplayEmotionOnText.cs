using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayEmotionOnText : MonoBehaviour
{
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;

    private bool isEmotionActivated = true;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            isEmotionActivated = !isEmotionActivated;
        }

        if (!isEmotionActivated) return;
        text1.text = EmotionManager.GetEmotionString(EmotionManager.instance.GetPlayer1Emotion());
        text2.text = EmotionManager.GetEmotionString(EmotionManager.instance.GetPlayer2Emotion());
    }
}
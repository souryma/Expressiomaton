using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayEmotionOnText : MonoBehaviour
{
    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;

    // Update is called once per frame
    void Update()
    {
        text1.text = EmotionManager.GetEmotionString(EmotionManager.instance.GetPlayer1Emotion());
        text2.text = EmotionManager.GetEmotionString(EmotionManager.instance.GetPlayer2Emotion());
    }
}
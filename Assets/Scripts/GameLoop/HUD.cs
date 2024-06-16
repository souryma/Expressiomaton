using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

[Serializable]
public struct HUD
{
    public TextMeshProUGUI countDownText;

    public TextMeshProUGUI roundResult;

    public TextMeshProUGUI roundText;
    public TextMeshProUGUI keepNeutralText;

    public RawImage background;

    public TextMeshProUGUI emotionText;

    public Image emotionImage;

    public GameObject animHandler;

    public GameObject scorePlayer;
    public GameObject scoreOpponent;
    public CanvasGroup neutralityScore;
}

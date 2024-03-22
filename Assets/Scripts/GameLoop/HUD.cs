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

    public RawImage background;

    public TextMeshProUGUI emotionText;

    public RawImage emotionImage;

    public RawImage animHandler;
}

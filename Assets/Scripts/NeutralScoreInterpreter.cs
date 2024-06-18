using System;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;


public class NeutralScoreInterpreter : MonoBehaviour
{
    [SerializeField] 
    private Image imageToSetup;

    [SerializeField]
    private Slider neutralSlider;
    [SerializeField]
    private Image sliderFill;
    [SerializeField] 
    private bool isP1;
    
    [SerializeField]
    private Gradient gradient;

    private float lastInterpretedNeutralScore;

    private float decay = 16f;

    private float expDecay(float a, float b, float decay, float dt)
    {
        return b + (a - b) * math.exp(-decay * dt);
    }

    Color ColorFromGradient (float value)  // float between 0-1
    {
        return gradient.Evaluate(value);
    }
    private void Update()
    {
        float neutralScore = isP1 ? EmotionManager.instance.NeutralScoreP1 : EmotionManager.instance.NeutralScoreP2;
        if (neutralScore < 0.5f)
        {
            SetupUI(0f);
            
            return;
        }

        if (neutralScore > 0.9f)
        {
            
            SetupUI(1f);
         
            return;
        }
        
        // var neutralLevels = EmotionManager.instance.NeutralLevels;
       var interpretedScore = math.remap(EmotionManager.instance.neutralScoreMin, EmotionManager.instance.neutralScoreMax, 0f, 1f,
            neutralScore);

       lastInterpretedNeutralScore = expDecay(lastInterpretedNeutralScore, interpretedScore, decay, Time.deltaTime);
       // imageToSetup.color = ColorFromGradient(lastInterpretedNeutralScore);

       SetupUI(lastInterpretedNeutralScore);
    }

    private void SetupUI(float neutralScore)
    {
        neutralSlider.value = neutralScore;
        sliderFill.color = ColorFromGradient(neutralScore);
    }
}
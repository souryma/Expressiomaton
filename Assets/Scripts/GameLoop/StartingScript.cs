using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StartingScript : MonoBehaviour
{
    [SerializeField] private Camera startCamera;

    [SerializeField] private CanvasGroup p1Ui;
    [SerializeField] private CanvasGroup p2Ui;
    [SerializeField] private RoundManagerNew roundManagerNew;
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        p1Ui.alpha = 0;
        p2Ui.alpha = 0;
        yield return new WaitForSeconds(2f);
        startCamera.depth = -1;
        startCamera.gameObject.SetActive(false);
        p1Ui.DOFade(1f, 1f);
        p2Ui.DOFade(1f, 1f);
        yield return new WaitForSeconds(1f);
        roundManagerNew.LaunchGame();
    }
    
}

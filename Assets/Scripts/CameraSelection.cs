using UnityEngine;

public class CameraSelection : MonoBehaviour
{
    [SerializeField] private string _nextSceneName;
    
    public void ConfirmCameraSelection()
    {
        WebcamManager.instance.ConfirmCameraSelection();
        ScenesManager.instance.LoadScene(_nextSceneName);
    }
}

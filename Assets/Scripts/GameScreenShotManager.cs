using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
[RequireComponent(typeof(GameEventListener))]
public class GameScreenShotManager : MonoBehaviour
{
  [SerializeField] private string m_screenshotFolder;
  [SerializeField] private int timeBeforeScreenShotIsInvalid;
  private string m_lastScreenShotName;
  private Texture2D m_lastScreenShot;

  private DateTime timeLastScreenTaken;
  
  public string LastScreenShotName {
      get
      {
          DateTime dateTime = timeLastScreenTaken;
          if (dateTime.AddSeconds(timeBeforeScreenShotIsInvalid)  > DateTime.Now)
          {
              return m_lastScreenShotName;
          }

          return string.Empty;
      }
      
  }
  private void Start(){
      try
      {
          if (!Directory.Exists(m_screenshotFolder))
          {
              Directory.CreateDirectory(m_screenshotFolder);
          }

      }
      catch (IOException ex)
      {
          Debug.Log(ex.ToString());
      }
  }
  
  public void TakeScreenShot()
  {
    StartCoroutine(ScreenShotTaker());
  }
  

  private IEnumerator ScreenShotTaker()
  {
    yield return new WaitForEndOfFrame();
    m_lastScreenShot = ScreenCapture.CaptureScreenshotAsTexture();
    string l_screenShotName = m_screenshotFolder+"screenshot " + System.DateTime.Now.ToString("MM-dd-yy (HH-mm-ss)") 
        + ".png";
    m_lastScreenShotName = l_screenShotName;
    timeLastScreenTaken = DateTime.Now;
    ScreenCapture.CaptureScreenshot(l_screenShotName);
    Debug.Log("Screenshot taken");
    // Emailer.SendEmail();
  }

}

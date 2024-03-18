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
  
  public void TakeScreenShot()
  {
    StartCoroutine(ScreenShotTaker());
  }
  

  private IEnumerator ScreenShotTaker()
  {
      try
      {
          if (!Directory.Exists(GetFolderPath()))
          {
              Directory.CreateDirectory(GetFolderPath());
          }

      }
      catch (IOException ex)
      {
          Debug.Log(ex.ToString());
      }

      yield return new WaitForEndOfFrame();
      // = new Texture2D();
      Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
      SavePictureToGallery(texture);
      Debug.Log("Screenshot taken");
      // Emailer.SendEmail();
  }

  private void SavePictureToGallery( Texture2D texture2D )
  {
      string filename = "MugShot " + DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss-f");
      string pathToFolder = GetFolderPath();
      byte[] bytes = texture2D.EncodeToPNG();
      timeLastScreenTaken = DateTime.Now;
        
      string path = Path.Combine(pathToFolder, filename );
      if( !filename.EndsWith( ".png" ) )
          path += ".png";
      
      m_lastScreenShotName = path;
      // Debug.Log( "Saving to: " + path );
      SaveImage(path, bytes);

  }

  private void SaveImage(string path, byte[] bytes)
  {
      File.WriteAllBytes( path, bytes );
      // yield return 0;
  }
  private string GetFolderPath()
  {
#if UNITY_EDITOR
      return System.Environment.GetFolderPath( System.Environment.SpecialFolder.DesktopDirectory ) +m_screenshotFolder;
  
#else
        return Application.persistentDataPath + m_screenshotFolder;
#endif
  }

}

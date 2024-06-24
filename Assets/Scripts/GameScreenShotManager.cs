using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Windows.WebCam;

[RequireComponent(typeof(GameEventListener))]
public class GameScreenShotManager : MonoBehaviour
{
  [SerializeField] private string m_screenshotFolder;
  [SerializeField] private int timeBeforeScreenShotIsInvalid;
  private List<string> m_sessionScreenShots = new List<string>();

  private DateTime timeLastScreenTaken;
  
  public List<string> sessionScreenShotNames {
      get
      {
          DateTime dateTime = timeLastScreenTaken;
          if (dateTime.AddSeconds(timeBeforeScreenShotIsInvalid) > DateTime.Now)
          {
              // List<string> l_session = new List<string>(m_sessionScreenShots);
              // m_sessionScreenShots = new List<string>();
              return m_sessionScreenShots;
          }

          return new List<string>();
      }
          
          
      // }
      // set => m_sessionScreenShots = value;
  }


  private IEnumerator ScreenShotTaker()
  {
      yield return new WaitForEndOfFrame();
      
      // = new Texture2D();
     string path = PathCreator();
      ScreenCapture.CaptureScreenshot(path);
      m_sessionScreenShots.Add(path);
      timeLastScreenTaken = DateTime.Now;
      // SavePictureToGallery(texture);
      Debug.Log("Screenshot taken");
  }
  private IEnumerator WebcamScreenShotTaker(WebCamTexture webCamTexture)
  {
  
      yield return new WaitForEndOfFrame(); 

      // it's a rare case where the Unity doco is pretty clear,
      // http://docs.unity3d.com/ScriptReference/WaitForEndOfFrame.html
      // be sure to scroll down to the SECOND long example on that doco page 

      Texture2D photo = new Texture2D(webCamTexture.width, webCamTexture.height);
      photo.SetPixels(webCamTexture.GetPixels());
      photo.Apply();
      string path = PathCreator();

      //Encode to a PNG
      byte[] bytes = photo.EncodeToPNG();
      File.WriteAllBytes(path, bytes);
          m_sessionScreenShots.Add(path);
      timeLastScreenTaken = DateTime.Now;
      Debug.Log("Screenshot taken");
  }

  private string PathCreator()
  {
      string filename = "Shot-o-maton " + DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss-f") + ".png";
      string pathToFolder = GetFolderPath();
      string path = Path.Combine(pathToFolder, filename );
      return path;
  }

  private string GetFolderPath()
  {
   
          return Application.dataPath + m_screenshotFolder;
  }

  private void Start()
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

      Application.quitting += OnApplicationQuit;
  }
  
  private void OnApplicationQuit()
  {
      CleaningScreens();
      Directory.Delete(GetFolderPath());
  }
  
    
  public void TakeScreenShot()
  {
      StartCoroutine(ScreenShotTaker());
  }
  public void TakeWebCamScreenShot()
  {
      if (WebcamManager.instance.isCameraSetup)
      {
          StartCoroutine(WebcamScreenShotTaker(WebcamManager.instance.Webcam1));
      }
  }

  public void CleaningScreens()
  {
      foreach (var path in m_sessionScreenShots)
      {
          if(File.Exists(path))
              File.Delete(path);
      }
#if UNITY_EDITOR
      UnityEditor.AssetDatabase.Refresh();
#endif
      m_sessionScreenShots = new List<string>();
  }
  
  
}

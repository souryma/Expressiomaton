using System;
using UnityEngine;
using FaceRecognitionSystem;

public class ImageSource : MonoBehaviour, IImageProvider {

    public Texture2D Image;

    public Color32 [ ] ImgData { 
        get {
            return _currentRawData;
        } set { 
        }
    }
    public int Width {
        get {
            return Image.width;
        }
        set {
        }
    }
    public int Height {
        get {
            return Image.height;
        }
        set {
        }
    }

    public ImageProviderReadyEvent Ready = new ImageProviderReadyEvent( );

    private void Start( ) {
        if ( Image != null ) {
            _currentRawData = new Color32 [ Image.GetPixels32( ).Length ];
            Ready.Invoke( this );
        }
    }

    private void Update( ) {
        if ( Image != null ) {
            Array.Copy( Image.GetPixels32( ), _currentRawData, _currentRawData.Length );
        }
    }

    private Color32[] _currentRawData = null;
}

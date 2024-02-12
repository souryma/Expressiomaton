using UnityEngine;
using UnityEngine.Android;
using FaceRecognitionSystem;
using UnityEngine.UI;

public class WebCamSource : MonoBehaviour, IImageProvider {
    public int CameraIndex;

    public Vector2 Resolution = new Vector2( 640, 480 );

    public ImageProviderReadyEvent Ready = new ImageProviderReadyEvent( );

    public Color32 [ ] ImgData {
        get {
            return _webcam?.GetPixels32( );
        }
        set {

        }
    }

    public int Width {
        get {
            return _webcam.width;
        }
        set {
        }
    }

    public int Height {
        get {
            return _webcam.height;
        }
        set {
        }
    }

    private void Start( ) {
#if PLATFORM_ANDROID && !UNITY_EDITOR
        if ( !Permission.HasUserAuthorizedPermission( Permission.Camera ) ) {
            Permission.RequestUserPermission( Permission.Camera );
        }
#endif
    }

    private void Update( ) {
#if PLATFORM_ANDROID && !UNITY_EDITOR
        if ( Permission.HasUserAuthorizedPermission( Permission.Camera ) ) {
            tryInit( );
        }
#else
        tryInit( );
#endif
    }

    private void tryInit( ) {
        if ( !_inited ) {
            var availableCameras = WebCamTexture.devices;
            if ( ( CameraIndex >= 0 ) && ( CameraIndex < availableCameras.Length ) ) {
                var device = availableCameras[ CameraIndex ];
                _webcam = new WebCamTexture( device.name, ( int )Resolution.x, ( int )Resolution.y );
                _webcam.Play( );

                Ready.Invoke( this );
                _inited = true;
            }
        }
    }

    private void OnDestroy( ) {
        _webcam?.Stop( );
    }

    private WebCamTexture _webcam = null;
    private bool _inited = false;
}

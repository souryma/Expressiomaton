using UnityEngine;
using UnityEngine.Video;
using FaceRecognitionSystem;

public class VideoSource : MonoBehaviour, IImageProvider {
    public Color32 [ ] ImgData {
        get {
            return getRawData( );
        }
        set {

        }
    }
    public int Width { get; set; }
    public int Height { get; set; }

    public VideoPlayer Player;

    public ImageProviderReadyEvent Ready = new ImageProviderReadyEvent( );

    private void Start( ) {

    }

    private void OnDestroy( ) {
        Player.Stop( );
    }
    private void Update( ) {
        if ( Player.isPlaying ) {
            if ( !_inited ) {
                _inited = true;
                this.Width = Player.targetTexture.width;
                this.Height = Player.targetTexture.height;
                _tmpTexture = new Texture2D( this.Width, this.Height, TextureFormat.RGBA32, false );
                this.Ready.Invoke( this );
            }
        }
    }

    private Color32 [ ] getRawData( ) {
        var old_rt = RenderTexture.active;
        RenderTexture.active = Player.targetTexture;

        _tmpTexture.ReadPixels( new Rect( 0, 0, this.Width, this.Height ), 0, 0 );
        _tmpTexture.Apply( );

        RenderTexture.active = old_rt;
        var result = _tmpTexture.GetPixels32( );
        return result;
    }
    private bool _inited = false;
    private Texture2D _tmpTexture = null;
}

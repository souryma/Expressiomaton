using System.Linq;

namespace OpenCvSharp.Demo
{
	using System;
	using UnityEngine;
	using System.Collections.Generic;
	using UnityEngine.UI;
	using OpenCvSharp;

	public class FaceDetectorScene : WebCamera
	{
		public TextAsset faces;
		public TextAsset eyes;
		public TextAsset shapes;
		public GameObject Surface2;
		private FaceProcessorLive<Texture2D> processor;

		/// <summary>
		/// Default initializer for MonoBehavior sub-classes
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			base.forceFrontalCamera = true; // we work with frontal cams here, let's force it for macOS s MacBook doesn't state frontal cam correctly

			byte[] shapeDat = shapes.bytes;
			if (shapeDat.Length == 0)
			{
				string errorMessage =
					"In order to have Face Landmarks working you must download special pre-trained shape predictor " +
					"available for free via DLib library website and replace a placeholder file located at " +
					"\"OpenCV+Unity/Assets/Resources/shape_predictor_68_face_landmarks.bytes\"\n\n" +
					"Without shape predictor demo will only detect face rects.";

#if UNITY_EDITOR
				// // query user to download the proper shape predictor
				// if (UnityEditor.EditorUtility.DisplayDialog("Shape predictor data missing", errorMessage, "Download", "OK, process with face rects only"))
				// 	Application.OpenURL("http://dlib.net/files/shape_predictor_68_face_landmarks.dat.bz2");
#else
             UnityEngine.Debug.Log(errorMessage);
#endif
			}

			processor = new FaceProcessorLive<Texture2D>();
			processor.Initialize(faces.text, eyes.text, shapes.bytes);

			// data stabilizer - affects face rects, face landmarks etc.
			processor.DataStabilizer.Enabled = true;        // enable stabilizer
			processor.DataStabilizer.Threshold = 2.0;       // threshold value in pixels
			processor.DataStabilizer.SamplesCount = 2;      // how many samples do we need to compute stable data

			// performance data - some tricks to make it work faster
			processor.Performance.Downscale = 256;          // processed image is pre-scaled down to N px by long side
			processor.Performance.SkipRate = 0;             // we actually process only each Nth frame (and every frame for skipRate = 0)
		}

		/// <summary>
		/// Per-frame video capture processor
		/// </summary>
		protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
		{
	
			// detect everything we're interested in
			Texture2D texture2D = new Texture2D(input.width / 3, input.height, TextureFormat.RGBA32, false);
			texture2D.SetPixels(input.GetPixels(input.width/3, 0, input.width/3, input.height));
			// texture2D.ReadPixels(new UnityEngine.Rect(input.width/3, 0, input.width/3, input.height), 0,0);
			// Graphics.Blit(input, receiver, new Vector2(1f, 1f), new Vector2(input.width/3, 0f),0,0 );
			texture2D.Apply();
			processor.ProcessTexture(texture2D, TextureParameters);

			// mark detected objects
			// processor.MarkDetected();
	
			
			
			// processor.Image now holds data we'd like to visualize
			output = Unity.MatToTexture(processor.Image, output);   // if output is valid texture it's buffer will be re-used, otherwise it will be re-created
			if (processor.Faces.Count != 0)
			{
				var rect = processor.Faces.Last();
				Texture2D texture2D2 = new Texture2D(rect.Region.Width, rect.Region.Height, TextureFormat.RGBA32, false);
				// Debug.Log(rect.Region.Left);
				// Debug.Log(rect.Region.Width);
				Debug.Log(rect.Region.Y + " " + rect.Region.Height);
				int why =output.height-rect.Region.Y -  rect.Region.Height;
				texture2D2.SetPixels(output.GetPixels(rect.Region.X, why, rect.Region.Width, rect
					.Region.Height)); 
				texture2D2.Apply();
				Surface2.GetComponent<RawImage>().texture = texture2D2;
			}
			return true;
		}
	}
}
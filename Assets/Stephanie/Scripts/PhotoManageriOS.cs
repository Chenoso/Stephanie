using UnityEngine;
using System.Collections;
using Prime31;



namespace Prime31
{
	public class PhotoManageriOS : MonoBehaviourGUI
	{
#if UNITY_IOS
		public GameObject testPlane;
		private string imagePath;


		void Start()
		{
			// Listen to image picker event so we can load the image into a texture later
			EtceteraManager.imagePickerChoseImageEvent += imagePickerChoseImage;
		}


		void OnDisable()
		{
			// Stop listening to the image picker event
			EtceteraManager.imagePickerChoseImageEvent -= imagePickerChoseImage;
		}


		void OnGUI()
		{
			beginColumn();




			if( GUILayout.Button( "Prompt for Photo" ) )
			{
				EtceteraBinding.promptForPhoto( 0.25f, PhotoPromptType.CameraAndAlbum );
			}


			if( GUILayout.Button( "Prompt for Multiple Photos" ) )
			{
				EtceteraBinding.promptForMultiplePhotos( 3, 0.25f );
			}


			if( GUILayout.Button( "Load Photo Texture" ) )
			{
				if( imagePath == null )
				{
					var buttons = new string[] { "OK" };
					EtceteraBinding.showAlertWithTitleMessageAndButtons( "Load Photo Texture Error", "You have to choose a photo before loading", buttons );
					return;
				}

				// No need to resize because we asked for an image scaled from the picker but this is how we sould do it if we wanted to
				// Resize the image so that we dont end up trying to load a gigantic image
				//EtceteraBinding.resizeImageAtPath( imagePath, 256, 256 );

				// Add 'file://' to the imagePath so that it is accessible via the WWW class
				StartCoroutine( EtceteraManager.textureFromFileAtPath( "file://" + imagePath, textureLoaded, textureLoadFailed ) );
			}


			if( GUILayout.Button( "Save Photo to Album" ) )
			{
				if( imagePath == null )
				{
					var buttons = new string[] { "OK" };
					EtceteraBinding.showAlertWithTitleMessageAndButtons( "Load Photo Texture Error", "You have to choose a photo before loading", buttons );
					return;
				}

				EtceteraBinding.saveImageToPhotoAlbum( imagePath );
			}


			if( GUILayout.Button( "Get Image Size" ) )
			{
				if( imagePath == null )
				{
					var buttons = new string[] { "OK" };
					EtceteraBinding.showAlertWithTitleMessageAndButtons( "Error Getting Image Size", "You have to choose a photo before checking it's size", buttons );
					return;
				}

				var size = EtceteraBinding.getImageSize( imagePath );
				Debug.Log( "image size: " + size );
			}


			endColumn();


			// Next scene button
			if( bottomRightButton( "Next" ) )
			{
				Application.LoadLevel( "EtceteraTestSceneThree" );
			}
		}


		void imagePickerChoseImage( string imagePath )
		{
			this.imagePath = imagePath;
		}


		public IEnumerator hideActivityView()
		{
			yield return new WaitForSeconds( 2.0f );
			EtceteraBinding.hideActivityView();
		}


		// Texture loading delegates
		public void textureLoaded( Texture2D texture )
		{
			testPlane.GetComponent<Renderer>().material.mainTexture = texture;
		}


		public void textureLoadFailed( string error )
		{
			var buttons = new string[] { "OK" };
			EtceteraBinding.showAlertWithTitleMessageAndButtons( "Error Loading Texture.  Did you choose a photo first?", error, buttons );
			Debug.Log( "textureLoadFailed: " + error );
		}

#endif
	}

}

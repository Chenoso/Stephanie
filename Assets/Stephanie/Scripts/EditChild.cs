using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;

public class EditChild : MonoBehaviour {
	
	public UITexture photoUITexture;
	public SelectChildConfig selectChildConfig;
	public UIInput nameUIInput;
	public LoadingFeedback loadingSystem;
	public GameObject confirmButton;
	public CurrenChild currentChild;
	
	WebCamTexture webcamTexture;
	string myURL;
	string fileName;
	string memoryDay;
	string memoryMonth;
	string memoryYear;

	int indexToEdit;
	
	void LoadConfigs(){
		myURL = Config.masterURL;
		fileName = Config.user + ".txt";
		
		myURL += "?webfilename=" + fileName +
			"&webusername=" + Config.masterUser +
				"&webpassword=" + Config.masterPass;
		
		//NGUITools.SetActive (confirmButton, false);

		nameUIInput.value = Config.currentChildName;
		
		#if UNITY_EDITOR
		webcamTexture = new WebCamTexture ();
		photoUITexture.mainTexture = webcamTexture;
		webcamTexture.Play ();
		#endif
		#if UNITY_IOS
		PromptForPhoto ();
		#endif
	}
	
	public void OpenEditChild ()
	{
		Vector3 newPosX = new Vector3 (0, 0, 0);
		transform.localPosition = newPosX;

		LoadConfigs ();
	}
	
	public void CloseEditChild ()
	{
		Vector3 newPosX = new Vector3 (900, 3000, 0);
		transform.localPosition = newPosX;
		
		#if UNITY_EDITOR
		webcamTexture.Stop ();
		webcamTexture = null;
		#endif

		currentChild.showCurrentChild ();
		loadingSystem.CloseLoading ();
		//NGUITools.SetActive (confirmButton, false);
		photoUITexture.mainTexture = null;
	}
	
	public void TakePicture ()
	{
		#if UNITY_EDITOR
		Texture2D photo = new Texture2D (webcamTexture.width, webcamTexture.height);
		photo.SetPixels (webcamTexture.GetPixels ());
		photo.Apply ();
		
		photoUITexture.mainTexture = photo;
		#endif
		
		#if UNITY_IOS
		PromptForPhoto ();
		#endif
		
		//NGUITools.SetActive (confirmButton, true);
	}
	
	public void UploadNewChild ()
	{
		Debug.Log ("Starting upload");
		
		loadingSystem.ShowLoadingUpload ();
		StartCoroutine ("UploadChildNames");
	}
	
	public IEnumerator UploadChildNames ()
	{
		Debug.Log ("Starting uploading names...");
		
		List<ArrayWrapper> childNamesList = Config.childNames;

		for (int i = 0; i < childNamesList.Count; i++) {
			if(childNamesList[i].array[1] == Config.currentChildName){
				childNamesList[i].array[1] = nameUIInput.value;
				indexToEdit = i;
				break;
			}
		}
		
		ES2Web web = new ES2Web (myURL + "&tag=names");
		
		yield return StartCoroutine (web.Upload (childNamesList));
		
		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);
		}
		
		if (web.isDone) {
			Debug.Log ("Edited names uploaded!");
			StartCoroutine ("UploadChildPhotos");
		}
	}
	
	public IEnumerator UploadChildPhotos ()
	{
		Debug.Log ("Starting uploading photos...");
		
		List<Texture2D> childPhotoList = Config.childTextures2D;
		Texture2D texture = photoUITexture.mainTexture as Texture2D;
		childPhotoList[indexToEdit] = texture;
		
		ES2Web web = new ES2Web (myURL + "&tag=photos");
		
		yield return StartCoroutine (web.Upload (childPhotoList));
		
		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);
		}
		
		if (web.isDone) {
			Debug.Log ("Edited photos uploaded!");
			currentChild.UpdateData(Config.childNames[indexToEdit].array[1], Config.childTextures2D[indexToEdit]);
			CloseEditChild ();
			selectChildConfig.UpdateData ();
		}
	}
	
	
	
	#if UNITY_IOS
	private string imagePath;
	
	void PromptForPhoto(){
		EtceteraBinding.promptForPhoto (0.5f, PhotoPromptType.CameraAndAlbum);
	}
	
	public void LoadPhotoImage(){
		//Load Image
		if (imagePath == null) {
			var buttons = new string[] { "OK" };
			EtceteraBinding.showAlertWithTitleMessageAndButtons ("Load Photo Texture Error", "You have to choose a photo before loading", buttons);
			return;
		}
		
		// No need to resize because we asked for an image scaled from the picker but this is how we sould do it if we wanted to
		// Resize the image so that we dont end up trying to load a gigantic image
		//EtceteraBinding.resizeImageAtPath( imagePath, 256, 256 );
		
		// Add 'file://' to the imagePath so that it is accessible via the WWW class
		StartCoroutine (EtceteraManager.textureFromFileAtPath ("file://" + imagePath, textureLoaded, textureLoadFailed));
	}
	
	void SavePhotoImage(){
		//Save image
		if (imagePath == null) {
			var buttons = new string[] { "OK" };
			EtceteraBinding.showAlertWithTitleMessageAndButtons ("Load Photo Texture Error", "You have to choose a photo before loading", buttons);
			return;
		}
		
		EtceteraBinding.saveImageToPhotoAlbum (imagePath);
	}
	
	// Texture loading delegates
	public void textureLoaded( Texture2D texture )
	{
		photoUITexture.mainTexture = texture;
		//SavePhotoImage ();
		StartCoroutine ("SnapPhotos");
	}
	
	public IEnumerator SnapPhotos()
	{
		yield return new WaitForSeconds( 1.0f );
		//photoUITexture.
	}
	
	
	
	
	public void textureLoadFailed( string error )
	{
		var buttons = new string[] { "OK" };
		EtceteraBinding.showAlertWithTitleMessageAndButtons( "Error Loading Texture.  Did you choose a photo first?", error, buttons );
		Debug.Log( "textureLoadFailed: " + error );
	}
	
	
	void OnEnable()
	{
		// Listen to all events for illustration purposes
		EtceteraManager.dismissingViewControllerEvent += dismissingViewControllerEvent;
		EtceteraManager.imagePickerCancelledEvent += imagePickerCancelled;
		EtceteraManager.imagePickerChoseImageEvent += imagePickerChoseImage;
		EtceteraManager.saveImageToPhotoAlbumSucceededEvent += saveImageToPhotoAlbumSucceededEvent;
		EtceteraManager.saveImageToPhotoAlbumFailedEvent += saveImageToPhotoAlbumFailedEvent;
		EtceteraManager.alertButtonClickedEvent += alertButtonClicked;
		
		EtceteraManager.promptCancelledEvent += promptCancelled;
		EtceteraManager.singleFieldPromptTextEnteredEvent += singleFieldPromptTextEntered;
		EtceteraManager.twoFieldPromptTextEnteredEvent += twoFieldPromptTextEntered;
		
		EtceteraManager.remoteRegistrationSucceededEvent += remoteRegistrationSucceeded;
		EtceteraManager.remoteRegistrationFailedEvent += remoteRegistrationFailed;
		EtceteraManager.pushIORegistrationCompletedEvent += pushIORegistrationCompletedEvent;
		EtceteraManager.urbanAirshipRegistrationSucceededEvent += urbanAirshipRegistrationSucceeded;
		EtceteraManager.urbanAirshipRegistrationFailedEvent += urbanAirshipRegistrationFailed;
		EtceteraManager.remoteNotificationReceivedEvent += remoteNotificationReceived;
		EtceteraManager.remoteNotificationReceivedAtLaunchEvent += remoteNotificationReceivedAtLaunch;
		
		EtceteraManager.localNotificationWasReceivedAtLaunchEvent += localNotificationWasReceivedAtLaunchEvent;
		EtceteraManager.localNotificationWasReceivedEvent += localNotificationWasReceivedEvent;
		
		EtceteraManager.mailComposerFinishedEvent += mailComposerFinished;
		EtceteraManager.smsComposerFinishedEvent += smsComposerFinished;
	}
	
	void OnDisable()
	{
		// Remove all event handlers
		EtceteraManager.dismissingViewControllerEvent += dismissingViewControllerEvent;
		EtceteraManager.imagePickerCancelledEvent -= imagePickerCancelled;
		EtceteraManager.imagePickerChoseImageEvent -= imagePickerChoseImage;
		EtceteraManager.saveImageToPhotoAlbumSucceededEvent -= saveImageToPhotoAlbumSucceededEvent;
		EtceteraManager.saveImageToPhotoAlbumFailedEvent -= saveImageToPhotoAlbumFailedEvent;
		EtceteraManager.alertButtonClickedEvent -= alertButtonClicked;
		
		EtceteraManager.promptCancelledEvent -= promptCancelled;
		EtceteraManager.singleFieldPromptTextEnteredEvent -= singleFieldPromptTextEntered;
		EtceteraManager.twoFieldPromptTextEnteredEvent -= twoFieldPromptTextEntered;
		
		EtceteraManager.remoteRegistrationSucceededEvent -= remoteRegistrationSucceeded;
		EtceteraManager.remoteRegistrationFailedEvent -= remoteRegistrationFailed;
		EtceteraManager.pushIORegistrationCompletedEvent -= pushIORegistrationCompletedEvent;
		EtceteraManager.urbanAirshipRegistrationSucceededEvent -= urbanAirshipRegistrationSucceeded;
		EtceteraManager.urbanAirshipRegistrationFailedEvent -= urbanAirshipRegistrationFailed;
		EtceteraManager.remoteNotificationReceivedAtLaunchEvent -= remoteNotificationReceivedAtLaunch;
		
		EtceteraManager.localNotificationWasReceivedAtLaunchEvent -= localNotificationWasReceivedAtLaunchEvent;
		EtceteraManager.localNotificationWasReceivedEvent -= localNotificationWasReceivedEvent;
		
		EtceteraManager.mailComposerFinishedEvent -= mailComposerFinished;
		EtceteraManager.smsComposerFinishedEvent -= smsComposerFinished;
	}
	
	
	void dismissingViewControllerEvent()
	{
		Debug.Log( "dismissingViewControllerEvent" );
	}
	
	
	void imagePickerCancelled()
	{
		//CloseEditChild ();
		Debug.Log( "imagePickerCancelled" );
	}
	
	
	void imagePickerChoseImage( string imagePath )
	{
		this.imagePath = imagePath;
		LoadPhotoImage ();
		//NGUITools.SetActive (confirmButton, true);
		Debug.Log( "image picker chose image: " + imagePath );
	}
	
	
	void saveImageToPhotoAlbumSucceededEvent()
	{
		Debug.Log( "saveImageToPhotoAlbumSucceededEvent" );
	}
	
	
	void saveImageToPhotoAlbumFailedEvent( string error )
	{
		CloseEditChild ();
		Debug.Log( "saveImageToPhotoAlbumFailedEvent: " + error );
	}
	
	
	void alertButtonClicked( string text )
	{
		Debug.Log( "alert button clicked: " + text );
	}
	
	
	void promptCancelled()
	{
		Debug.Log( "promptCancelled" );
	}
	
	
	void singleFieldPromptTextEntered( string text )
	{
		Debug.Log( "field : " + text );
	}
	
	
	void twoFieldPromptTextEntered( string textOne, string textTwo )
	{
		Debug.Log( "field one: " + textOne + ", field two: " + textTwo );
	}
	
	
	void remoteRegistrationSucceeded( string deviceToken )
	{
		Debug.Log( "remoteRegistrationSucceeded with deviceToken: " + deviceToken );
	}
	
	
	void remoteRegistrationFailed( string error )
	{
		Debug.Log( "remoteRegistrationFailed : " + error );
	}
	
	
	void pushIORegistrationCompletedEvent( string error )
	{
		if( error != null )
			Debug.Log( "pushIORegistrationCompletedEvent failed with error: " + error );
		else
			Debug.Log( "pushIORegistrationCompletedEvent successful" );
	}
	
	
	void urbanAirshipRegistrationSucceeded()
	{
		Debug.Log( "urbanAirshipRegistrationSucceeded" );
	}
	
	
	void urbanAirshipRegistrationFailed( string error )
	{
		Debug.Log( "urbanAirshipRegistrationFailed : " + error );
	}
	
	
	void remoteNotificationReceived( IDictionary notification )
	{
		Debug.Log( "remoteNotificationReceived" );
		Prime31.Utils.logObject( notification );
	}
	
	
	void remoteNotificationReceivedAtLaunch( IDictionary notification )
	{
		Debug.Log( "remoteNotificationReceivedAtLaunch" );
		Prime31.Utils.logObject( notification );
	}
	
	
	void localNotificationWasReceivedEvent( IDictionary notification )
	{
		Debug.Log( "localNotificationWasReceivedEvent" );
		Prime31.Utils.logObject( notification );
	}
	
	
	void localNotificationWasReceivedAtLaunchEvent( IDictionary notification )
	{
		Debug.Log( "localNotificationWasReceivedAtLaunchEvent" );
		Prime31.Utils.logObject( notification );
	}
	
	
	void mailComposerFinished( string result )
	{
		Debug.Log( "mailComposerFinished : " + result );
	}
	
	
	void smsComposerFinished( string result )
	{
		Debug.Log( "smsComposerFinished : " + result );
	}
	
	#endif
	
}

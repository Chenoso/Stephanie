﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31;

public class AddMemory : MonoBehaviour
{

	public UITexture photoUITexture;
	public UIInput titleUILabel;
	public UIInput descriptionUILabel;
	public LoadingFeedback loadingSystem;
	public MemoriesConfig memoriesConfig;
	public GameObject confirmButton;

	WebCamTexture webcamTexture;
	string myURL;
	string fileName;
	int totalMemories;
	string memoryDay;
	string memoryMonth;
	string memoryYear;


	void LoadConfigs(){
		webcamTexture = new WebCamTexture (480, 320, 30);
		
		myURL = Config.masterURL;
		fileName = Config.currentChild;
		totalMemories = Config.currentChildDayMemoriesCount;

		Debug.Log ("Total memories: " + totalMemories);
		
		myURL += "?webfilename=" + fileName +
			"&webusername=" + Config.masterUser +
				"&webpassword=" + Config.masterPass;
		
		memoryDay = Config.currentDay;
		memoryMonth = Config.currentMonth;
		memoryYear = Config.currentYear;

		NGUITools.SetActive (confirmButton, false);

		titleUILabel.value = "";
		descriptionUILabel.value = "";

		photoUITexture.mainTexture = null;
		
#if UNITY_EDITOR
		webcamTexture = new WebCamTexture ();
		photoUITexture.mainTexture = webcamTexture;
		webcamTexture.Play ();
#endif
#if UNITY_IOS
		PromptForPhoto ();
#endif
	}

	public void OpenNewMemory ()
	{
		Vector3 newPos = new Vector3 (0, 0, 0);
		transform.localPosition = newPos;

		LoadConfigs ();
	}

	public void CloseNewMemory ()
	{
		Vector3 newPos = new Vector3 (1800, 1500, 0);
		transform.localPosition = newPos;

#if UNITY_EDITOR
		webcamTexture.Stop ();
		webcamTexture = null;
#endif

		NGUITools.SetActive (confirmButton, false);
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
		NGUITools.SetActive (confirmButton, true);
	}

	/*
	 * Add new memory
	 * 
	 * 
	 * */

	public void AddPhotoAndText ()
	{
		Debug.Log ("#######################");
		Debug.Log ("Starting upload memory!");

		loadingSystem.ShowLoadingUpload ();
		StartCoroutine ("UploadDate");
	}

	public IEnumerator UploadDate ()
	{
		Debug.Log ("Uploading date...");

		Config.currentChildDatesCalendarList.Add (ArrayWrapper.Create (new string[]{
			memoryDay,
			memoryMonth,
			memoryYear
		}));
		
		ES2Web web = new ES2Web (myURL + "&tag=memoriesDates");
		
		yield return StartCoroutine (web.Upload (Config.currentChildDatesCalendarList));
		
		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);
		}
		
		if (web.isDone) {
			Debug.Log ("New memoryDate uploaded!");
			StartCoroutine ("UploadText");
		}
	}
	
	public IEnumerator UploadText ()
	{
		Debug.Log ("Starting uploading text...");
		string memoryTime = System.DateTime.Now.ToString ("HH:mm");
		Config.currentChildDayMemoriesList.Add (ArrayWrapper.Create (new string[]{
			titleUILabel.value,
			descriptionUILabel.value,
			memoryDay,
			memoryMonth,
			memoryYear,
			memoryTime,
		}));
		
		ES2Web web = new ES2Web (myURL + 
			"&tag=" + 
			memoryDay + 
			"_" + 
			memoryMonth + 
			"_" + 
			memoryYear + 
			"_text");
		
		yield return StartCoroutine (web.Upload (Config.currentChildDayMemoriesList));
		
		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);
		}
		
		if (web.isDone) {
			Debug.Log ("New text uploaded!");
			StartCoroutine ("UploadPhoto");
		}
	}
	
	public IEnumerator UploadPhoto ()
	{
		Debug.Log ("Starting uploading photos...");
		Texture2D memoryPhoto = photoUITexture.mainTexture as Texture2D;
		Config.currentChildDayMemoriesPhotoList.Add (memoryPhoto);
		
		webProgress = new ES2Web (myURL + 
			"&tag=" + 
			memoryDay + 
			"_" + 
			memoryMonth + 
			"_" + 
			memoryYear + 
			"_photo");

		canCheckProgress = true;
		yield return StartCoroutine (webProgress.Upload (Config.currentChildDayMemoriesPhotoList));
		
		if (webProgress.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (webProgress.errorCode + ":" + webProgress.error);
		}
		
		if (webProgress.isDone) {
			Debug.Log ("New photo uploaded!");
			StartCoroutine ("UploadPhotoThumb");
		}
	}

	private ES2Web webProgress;
	private bool canCheckProgress;

	void Update(){
		//if (canCheckProgress)
			//Debug.Log (webProgress.www.uploadProgress);
	}
	
	public IEnumerator UploadPhotoThumb ()
	{
		Debug.Log ("Starting uploading thumb...");
		//Texture2D memoryThumb = new Texture2D (photoUITexture.width, photoUITexture.height, TextureFormat.PVRTC_RGBA2, false);
		//memoryThumb = photoUITexture.mainTexture as Texture2D;
		Texture2D memoryThumb = photoUITexture.mainTexture as Texture2D;
		Config.currentChildDayMemoriesThumbList.Add (memoryThumb);
		
		ES2Web web = new ES2Web (myURL + 
			"&tag=" + 
			memoryDay + 
			"_" + 
			memoryMonth + 
			"_" + 
			memoryYear + 
			"_thumb");
		
		yield return StartCoroutine (web.Upload (Config.currentChildDayMemoriesThumbList));
		
		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);
		}
		
		if (web.isDone) {
			Debug.Log ("New thumb uploaded!");
			StartCoroutine ("UploadMemoryCount");
		}
	}
	
	public IEnumerator UploadMemoryCount ()
	{
		Debug.Log ("Starting uploading count...");
		totalMemories += 1;
		ES2Web web = new ES2Web (myURL + 
			"&tag=" + 
			memoryDay + 
			"_" + 
			memoryMonth + 
			"_" + 
			memoryYear + 
			"_count");

		yield return StartCoroutine (web.Upload (totalMemories));
		
		if (web.isError) {
			Debug.LogError (web.errorCode + ":" + web.error);
		}
		
		if (web.isDone) {
			Debug.Log ("New count uploaded!");
			CloseNewMemory();
			loadingSystem.CloseLoading ();
			memoriesConfig.UpdateData();
		}

		photoUITexture.mainTexture = null;
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
		CloseNewMemory ();
		Debug.Log( "imagePickerCancelled" );
	}
	
	
	void imagePickerChoseImage( string imagePath )
	{
		this.imagePath = imagePath;
		LoadPhotoImage ();
		NGUITools.SetActive (confirmButton, true);
		Debug.Log( "image picker chose image: " + imagePath );
	}
	
	
	void saveImageToPhotoAlbumSucceededEvent()
	{
		Debug.Log( "saveImageToPhotoAlbumSucceededEvent" );
	}
	
	
	void saveImageToPhotoAlbumFailedEvent( string error )
	{
		CloseNewMemory ();
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

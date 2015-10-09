using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AddMemory : MonoBehaviour
{

	public UITexture photoUITexture;
	public UIInput titleUILabel;
	public UIInput descriptionUILabel;
	public LoadingFeedback loadingSystem;
	public MemoriesConfig memoriesConfig;

	WebCamTexture webcamTexture;
	string myURL;
	string fileName;
	int totalMemories;
	string memoryDay;
	string memoryMonth;
	string memoryYear;


	void LoadConfigs(){
		webcamTexture = new WebCamTexture ();
		
		myURL = Config.masterURL;
		fileName = Config.currentChild;
		totalMemories = Config.currentChildDayMemoriesCount;
		
		myURL += "?webfilename=" + fileName +
			"&webusername=" + Config.masterUser +
				"&webpassword=" + Config.masterPass;
		
		memoryDay = Config.currentDay;
		memoryMonth = Config.currentMonth;
		memoryYear = Config.currentYear;

		photoUITexture.mainTexture = webcamTexture;
		webcamTexture.Play ();
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

		webcamTexture.Stop ();
		webcamTexture = null;
	}

	public void TakePicture ()
	{
		Texture2D photo = new Texture2D (webcamTexture.width, webcamTexture.height);
		photo.SetPixels (webcamTexture.GetPixels ());
		photo.Apply ();
		
		photoUITexture.mainTexture = photo;
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
		Debug.Log (memoryDay);
		Debug.Log (memoryMonth);
		Debug.Log (memoryYear);

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
		
		ES2Web web = new ES2Web (myURL + 
			"&tag=" + 
			memoryDay + 
			"_" + 
			memoryMonth + 
			"_" + 
			memoryYear + 
			"_photo");

		yield return StartCoroutine (web.Upload (Config.currentChildDayMemoriesPhotoList));
		
		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);
		}
		
		if (web.isDone) {
			Debug.Log ("New photo uploaded!");
			StartCoroutine ("UploadPhotoThumb");
		}
	}
	
	public IEnumerator UploadPhotoThumb ()
	{
		Debug.Log ("Starting uploading thumb...");
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
	}

}

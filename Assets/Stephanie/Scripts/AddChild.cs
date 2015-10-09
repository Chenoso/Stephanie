using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AddChild : MonoBehaviour
{

	public UITexture photoUITexture;
	public SelectChildConfig selectChildConfig;
	public UIInput nameUIInput;
	public LoadingFeedback loadingSystem;

	WebCamTexture webcamTexture;
	string myURL;
	string fileName;
	string memoryDay;
	string memoryMonth;
	string memoryYear;


	void LoadConfigs(){
		myURL = Config.masterURL;
		fileName = "childList.txt";
		
		myURL += "?webfilename=" + fileName +
			"&webusername=" + Config.masterUser +
				"&webpassword=" + Config.masterPass;

		webcamTexture = new WebCamTexture ();
		photoUITexture.mainTexture = webcamTexture;
		webcamTexture.Play ();
	}

	public void OpenAddNewChild ()
	{
		Vector3 newPosX = new Vector3 (0, 0, 0);
		transform.localPosition = newPosX;
	
		LoadConfigs ();
	}

	private void CloseAddNewChild ()
	{
		Vector3 newPosX = new Vector3 (900, 1500, 0);
		transform.localPosition = newPosX;

		webcamTexture.Stop ();
		webcamTexture = null;

		loadingSystem.CloseLoading ();
	}

	public void TakePicture ()
	{
		Texture2D photo = new Texture2D (webcamTexture.width, webcamTexture.height);
		photo.SetPixels (webcamTexture.GetPixels ());
		photo.Apply ();
		
		photoUITexture.mainTexture = photo;
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

		List<string> childNamesList = Config.childNames;
		childNamesList.Add (nameUIInput.value);

		ES2Web web = new ES2Web (myURL + "&tag=names");
		
		yield return StartCoroutine (web.Upload (childNamesList));
		
		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);
		}

		if (web.isDone) {
			Debug.Log ("New names uploaded!");
			StartCoroutine ("UploadChildPhotos");
		}
	}

	public IEnumerator UploadChildPhotos ()
	{
		Debug.Log ("Starting uploading photos...");
		
		List<Texture2D> childPhotoList = Config.childTextures2D;
		Texture2D texture = photoUITexture.mainTexture as Texture2D;
		childPhotoList.Add (texture);
		
		ES2Web web = new ES2Web (myURL + "&tag=photos");
		
		yield return StartCoroutine (web.Upload (childPhotoList));
		
		if (web.isError) {
			// Enter your own code to handle errors here.
			Debug.LogError (web.errorCode + ":" + web.error);
		}
		
		if (web.isDone) {
			Debug.Log ("New photos uploaded!");
			CloseAddNewChild ();
			selectChildConfig.UpdateData ();
		}
	}
}

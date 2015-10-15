using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectChildConfig : MonoBehaviour
{
	
	public GameObject child01;
	public GameObject child02;
	public GameObject child03;
	public GameObject child04;
	public LoadingFeedback loadingSystem;
	public CalendarPopUp calendarPanel;
	public AddChild addChildContainer;
	public List<GameObject> childListGO = new List<GameObject> ();

	string myURL;
	string fileName;
	
	// Use this for initialization
	void Start ()
	{
		LoadConfigs ();
	}

	void LoadConfigs(){
		childListGO.Add (child01);
		childListGO.Add (child02);
		childListGO.Add (child03);
		childListGO.Add (child04);
	
		for (int i = 0; i < 4; i++) {

			GameObject currentChild = childListGO [i];

			EventDelegate.Add(currentChild.transform.FindChild("ArrowButton").GetComponent<UIButton>().onClick, 
			                  delegate () { 
				this.OnClickCurrentChild(currentChild.transform.FindChild("Label_KidName").GetComponent<UILabel>().text);
			});

			EventDelegate.Add(childListGO [i].transform.FindChild("Button").GetComponent<UIButton>().onClick, 
			                  delegate () { 
				addChildContainer.OpenAddNewChild();
				});

			NGUITools.SetActive (childListGO [i].transform.FindChild ("ArrowButton").gameObject, false);
		}

		myURL = Config.masterURL;
		fileName = "childList.txt";

		myURL += "?webfilename=" + fileName +
			"&webusername=" + Config.masterUser +
				"&webpassword=" + Config.masterPass;

		loadingSystem.ShowLoadingDownload ();
		StartCoroutine ("DownloadListOfChildsNames");
	}

	public IEnumerator DownloadListOfChildsNames ()
	{
		Debug.Log ("Init download - Childs names...");
		ES2Web web = new ES2Web (myURL + "&tag=names");
		yield return StartCoroutine (web.Download ());
		
		if (web.isError) {
			Debug.LogError (web.errorCode + ":" + web.error);
			loadingSystem.CloseLoading();

			//TODO
			//Make user go back to login if error

		} else {
			List<string> childNamesList = web.LoadList<string> ("names");
			Config.childNames = childNamesList;
			Debug.Log ("Finished downloading names...");
			StartCoroutine ("DownloadListOfChildsPhotos");
		}
	}

	public IEnumerator DownloadListOfChildsPhotos ()
	{
		Debug.Log ("Init download - Childs photos...");
		ES2Web web = new ES2Web (myURL + "&tag=photos");
		yield return StartCoroutine (web.Download ());
		
		if (web.isError) {
			Debug.LogError (web.errorCode + ":" + web.error);
			loadingSystem.CloseLoading();
			
			//TODO
			//Make user go back to login if error
			
		} else {
			Debug.Log ("Finished downloading photos...");
			List<Texture2D> childPhotosList = web.LoadList<Texture2D> ("photos");
			Config.childTextures2D = childPhotosList;

			for (int i = 0; i < childPhotosList.Count; i++) {
				childListGO [i].GetComponentInChildren<UITexture> ().mainTexture = childPhotosList [i];
				NGUITools.SetActive (childListGO [i].transform.Find ("ArrowButton").gameObject, true);
				NGUITools.SetActive (childListGO [i].transform.Find ("Button").gameObject, false);

				if (i < 3) {
					childListGO [i + 1].transform.localPosition = new Vector3 (0, 444 - (190 * (i + 1)), 0);
				}
			}

			for (int j = 0; j < Config.childNames.Count; j++) {
				childListGO [j].GetComponentInChildren<UILabel> ().text = Config.childNames [j];
			}

			loadingSystem.CloseLoading();
		}
	}

	public void UpdateData ()
	{
		loadingSystem.ShowLoadingDownload ();
		StartCoroutine ("DownloadListOfChildsNames");
	}
	
	public void OnClickBackToLogin ()
	{
		Application.LoadLevel ("0.Login");
	}
	
	/*
	 * Click current child
	 * 
	 * 
	 */ 

	public void OnClickCurrentChild(string childName){
		childName = childName.Replace (" ", string.Empty);
		Config.currentChild = childName + ".txt";
		Config.currentChildDayMemoriesCount = 0;
		Config.currentChildDayMemoriesList.Clear ();
		Config.currentChildDayMemoriesPhotoList.Clear ();
		Config.currentChildDayMemoriesThumbList.Clear ();
		Config.currentChildDatesCalendarList.Clear ();
		calendarPanel.ShowCalendar ();
	}
}

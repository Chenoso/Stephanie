﻿using UnityEngine;
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
		fileName = Config.user + ".txt";

		myURL += "?webfilename=" + fileName +
			"&webusername=" + Config.masterUser +
				"&webpassword=" + Config.masterPass;

		loadingSystem.ShowLoadingDownload ();
		StartCoroutine ("DownloadListOfChildsNames");
	}

	public IEnumerator DownloadListOfChildsNames ()
	{
		Debug.Log ("fileName: " + fileName);

		Debug.Log ("Init download - Childs names...");
		ES2Web web = new ES2Web (myURL + "&tag=names");

		yield return StartCoroutine (web.Download ());

		
		if (web.isError) {
			Debug.LogError (web.errorCode + ":" + web.error);
			loadingSystem.CloseLoading();

			//TODO
			//Make user go back to login if error
			childListGO [0].transform.localPosition = new Vector3 (0, 444, 0);

		} else {
			List<ArrayWrapper> childNamesList = web.LoadList<ArrayWrapper> ("names");
			//List<string> childNamesList = web.LoadList<string> ("names");
			Config.childNames = childNamesList;
			Debug.Log ("Finished downloading names...");
			StartCoroutine ("DownloadListOfChildsPhotos");
		}
	}

	private ES2Web web2;
	private bool canCheck;

	void Update(){
		//if (canCheck)
			//Debug.Log (web2.www.progress);
	}

	public IEnumerator DownloadListOfChildsPhotos ()
	{
		Debug.Log ("Init download - Childs photos...");
		web2 = new ES2Web (myURL + "&tag=photos");

		canCheck = true;
		yield return StartCoroutine (web2.Download ());
		
		if (web2.isError) {
			Debug.LogError (web2.errorCode + ":" + web2.error);
			loadingSystem.CloseLoading();
			
			//TODO
			//Make user go back to login if error
			
		} else {
			Debug.Log ("Finished downloading photos...");
			List<Texture2D> childPhotosList = web2.LoadList<Texture2D> ("photos");
			Config.childTextures2D = childPhotosList;

			for (int i = 0; i < childPhotosList.Count; i++) {
				childListGO [i].GetComponentInChildren<UITexture> ().mainTexture = childPhotosList [i];
				childListGO [i].GetComponentInChildren<UILabel> ().text = Config.childNames [i].array [1];

				NGUITools.SetActive (childListGO [i].transform.Find ("ArrowButton").gameObject, true);
				NGUITools.SetActive (childListGO [i].transform.Find ("Button").gameObject, false);

				if (i < 4) {
					childListGO [i].transform.localPosition = new Vector3 (0, 444 - (190 * i), 0);
				}
			}

			if(childPhotosList.Count < 4){
				childListGO [childPhotosList.Count].transform.localPosition = new Vector3 (0, 444 - (190 * childPhotosList.Count), 0);
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

	public void OnClickCurrentChild(string _childName){

		for (int i = 0; i < Config.childNames.Count; i++) {
			if(_childName == Config.childNames [i].array [1]){
				Config.currentChildName = _childName;

				string childName = Config.childNames [i].array [0] + ".txt";
				Config.currentChild = childName.Replace (" ", string.Empty);
			}
		}

		Config.currentChildDayMemoriesCount = 0;
		Config.currentChildDayMemoriesList.Clear ();
		Config.currentChildDayMemoriesPhotoList.Clear ();
		Config.currentChildDayMemoriesThumbList.Clear ();
		Config.currentChildDatesCalendarList.Clear ();

		for (int i = 0; i < Config.childTextures2D.Count; i++) {
			if(Config.currentChildName == Config.childNames [i].array[1]){
				calendarPanel.ShowCalendar (Config.childNames [i].array[1], Config.childTextures2D [i]);
				break;
				return;
			}
		}
	}
}

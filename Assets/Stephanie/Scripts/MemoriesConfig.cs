using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MemoriesConfig : MonoBehaviour
{
	
	public Memory currentMemory;
	public GameObject masterParent;
	public LoadingFeedback loadingSystem;
	public List<Memory> memoryList = new List<Memory> ();
	public CalendarPopUp calendarPopup;
	public MemoryFull memoryFull;
	string myURL;
	string fileName;
	string memoryDay;
	string memoryMonth;
	string memoryYear;
	
	public void OpenMemories (string day, string month, string year)
	{
		Vector3 newPos = new Vector3 (0, 0, 0);
		transform.localPosition = newPos;

		Config.currentDay = day;
		Config.currentMonth = month;
		Config.currentYear = year;

		LoadConfigs ();
	}

	public void CloseMemories ()
	{
		Vector3 newPos = new Vector3 (1800, 0, 0);
		transform.localPosition = newPos;

		for (int i = 0; i < memoryList.Count; i++) {
			Destroy (memoryList [i].gameObject);
		}
		memoryList.Clear ();

		calendarPopup.UpdateData ();
	}

	
	/*
	 * Load memory size
	 * 
	 */

	void LoadConfigs ()
	{
		loadingSystem.ShowLoadingDownload ();

		Config.currentChildDayMemoriesCount = 0;
		Config.currentChildDayMemoriesList.Clear ();
		Config.currentChildDayMemoriesPhotoList.Clear ();
		Config.currentChildDayMemoriesThumbList.Clear ();

		myURL = Config.masterURL;
		fileName = Config.currentChild;

		Debug.Log ("#####################################");
		Debug.Log (Config.currentChild);
		Debug.Log ("#####################################");
		
		myURL += "?webfilename=" + fileName +
			"&webusername=" + Config.masterUser +
			"&webpassword=" + Config.masterPass;
		
		memoryDay = Config.currentDay;
		memoryMonth = Config.currentMonth;
		memoryYear = Config.currentYear;

		Debug.Log ("Starting download count!");
		
		StartCoroutine ("DownloadMemoryCount");
	}
	
	public IEnumerator DownloadMemoryCount ()
	{
		Debug.Log ("Initing downloading count...");
		ES2Web web = new ES2Web (myURL + 
			"&tag=" + 
			memoryDay + 
			"_" + 
			memoryMonth + 
			"_" + 
			memoryYear + 
			"_count");
		
		yield return StartCoroutine (web.Download ());
		
		if (web.isError) {
			Debug.LogError (web.errorCode + ":" + web.error);
			loadingSystem.CloseLoading ();
			
			//TODO
			//Make user go back to login if error
			
		} else {

			Debug.Log ("Total #: " + Config.currentChildDayMemoriesCount);

			Config.currentChildDayMemoriesCount = web.Load<int> (memoryDay + "_" + memoryMonth + "_" + memoryYear + "_count");
			Debug.Log ("Finished memory from: " + memoryDay + "_" + memoryMonth + "_" + memoryYear);
			Debug.Log ("Total memories: " + Config.currentChildDayMemoriesCount);

			CreateMemoriesList ();
		}
	}

	void CreateMemoriesList ()
	{
		for (int i = 0; i < Config.currentChildDayMemoriesCount; i++) {
			Memory newMemory = Instantiate (currentMemory, new Vector3 (0, -900, 0), Quaternion.identity) as Memory;
			newMemory.transform.parent = masterParent.transform;
			memoryList.Add (newMemory);

			EventDelegate.Add(newMemory.transform.FindChild("ArrowButton").GetComponent<UIButton>().onClick, 
			                  delegate () {memoryFull.UpdateData(newMemory);});
		}

		LoadPhotoAndText ();
	}


	/*
	 * Download Memory
	 * 
	 */

	public void LoadPhotoAndText ()
	{
		Debug.Log ("#######################");
		Debug.Log ("Starting download memory!");
		
		StartCoroutine ("DownloadMemoryText");
	}

	public IEnumerator DownloadMemoryText ()
	{
		Debug.Log ("Initing downloading text...");
		ES2Web web = new ES2Web (myURL + 
			"&tag=" + 
			memoryDay + 
			"_" + 
			memoryMonth + 
			"_" + 
			memoryYear + 
			"_text");
		
		yield return StartCoroutine (web.Download ());
		
		if (web.isError) {
			Debug.LogError (web.errorCode + ":" + web.error);
			loadingSystem.CloseLoading ();
			
			//TODO
			//Make user go back to login if error
			
		} else {
			Config.currentChildDayMemoriesList = web.LoadList<ArrayWrapper> (
				memoryDay + 
				"_" + 
				memoryMonth + 
				"_" + 
				memoryYear + 
				"_text");

			for (int i = 0; i < Config.currentChildDayMemoriesCount; i++) {
				memoryList [i].CreateMemoryText (
					Config.currentChildDayMemoriesList [i].array [0],
					Config.currentChildDayMemoriesList [i].array [1],
					Config.currentChildDayMemoriesList [i].array [2],
					Config.currentChildDayMemoriesList [i].array [3],
					Config.currentChildDayMemoriesList [i].array [4],
					Config.currentChildDayMemoriesList [i].array [5]);
			}

			Debug.Log ("Finished downloading memory text...");
			StartCoroutine ("DownloadMemoryThumb");
		}
	}
	
	public IEnumerator DownloadMemoryThumb ()
	{
		Debug.Log ("Initing downloading thumb...");
		ES2Web web = new ES2Web (myURL + 
			"&tag=" + 
			memoryDay + 
			"_" + 
			memoryMonth + 
			"_" + 
			memoryYear + 
			"_thumb");
		
		yield return StartCoroutine (web.Download ());
		
		if (web.isError) {
			Debug.LogError (web.errorCode + ":" + web.error);
			loadingSystem.CloseLoading ();
			
			//TODO
			//Make user go back to login if error
			
		} else {
			Debug.Log ("Finished downloading thumbs...");
			Config.currentChildDayMemoriesThumbList = web.LoadList<Texture2D>(
				memoryDay + 
				"_" + 
				memoryMonth + 
				"_" + 
				memoryYear + 
				"_thumb");

			for (int i = 0; i < Config.currentChildDayMemoriesCount; i++) {
				memoryList [i].CreateMemoryPhoto (Config.currentChildDayMemoriesThumbList[i]);

				Vector3 newPos = new Vector3 (0, 444 - (i * 190), 0);
				memoryList [i].transform.localPosition = newPos;
			}

			loadingSystem.CloseLoading ();
		}
	}

	public void UpdateData ()
	{
		loadingSystem.ShowLoadingDownload ();
		LoadConfigs ();
	}
	

}



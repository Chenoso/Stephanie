using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MemoriesConfig : MonoBehaviour
{
	
	public Memory currentMemory;
	public LoadingFeedback loadingSystem;
	public CalendarPopUp calendarPopup;
	public MemoryFull memoryFull;
	public UIScrollView scrollView;
	public UITable scrollTableView;
	public List<Memory> memoryList;
	public UILabel currentDay;
	public bool cameFromEditPhoto;
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


		if (int.Parse(day) < 10) {
			day = "0" + day;
		}
		if (int.Parse(month) < 10) {
			month = "0" + month;
		}

		//currentDay.text = day + "/" + month + "/" + year;

		Vector3 newPos2 = new Vector3 (900, 0, 0);
		calendarPopup.transform.localPosition = newPos2;

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

		Vector3 newPos2 = new Vector3 (0, 0, 0);
		calendarPopup.transform.localPosition = newPos2;
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

		Debug.Log ("@MEMORY DAY: " + memoryDay);
		Debug.Log ("Starting download count!");

		if (!cameFromEditPhoto) {
			for (int j = 0; j < memoryList.Count; j++) {
				Destroy (memoryList [j].gameObject);
			}
			memoryList.Clear ();
		}
		
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
		if (!cameFromEditPhoto) {
			memoryList = new List<Memory> ();

			for (int i = 0; i < Config.currentChildDayMemoriesCount; i++) {
				Memory newMemory = Instantiate (currentMemory, new Vector3 (0, -900, 0), Quaternion.identity) as Memory;
				newMemory.transform.parent = scrollTableView.transform;
				newMemory.transform.localScale = new Vector3 (1, 1, 1);
				newMemory.GetComponent<UIDragScrollView> ().scrollView = scrollView;
				memoryList.Add (newMemory);

				EventDelegate.Add (newMemory.GetComponent<UIButton> ().onClick, delegate () {
					memoryFull.UpdateData (newMemory);
				});
			}
		}

		NGUITools.ImmediatelyCreateDrawCalls(scrollTableView.gameObject);
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
					i,
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
			Config.currentChildDayMemoriesThumbList.Clear();

			Config.currentChildDayMemoriesThumbList = web.LoadList<Texture2D>(
				memoryDay + 
				"_" + 
				memoryMonth + 
				"_" + 
				memoryYear + 
				"_thumb");

			for (int i = 0; i < Config.currentChildDayMemoriesCount; i++) {
				memoryList [i].CreateMemoryPhoto (Config.currentChildDayMemoriesThumbList[i]);

				//Vector3 newPos = new Vector3 (0, 530 - (i * 780), 0);
				//memoryList[i].transform.localPosition = newPos;
			}
			web.www.Dispose();
			web = null;
			Resources.UnloadUnusedAssets();

			StartCoroutine("AdjustLayout");
		}
	}

	private IEnumerator AdjustLayout(){
		yield return new WaitForSeconds (0.1f);
		scrollTableView.repositionNow = true;
		scrollTableView.Reposition();
		scrollView.ResetPosition();
		loadingSystem.CloseLoading ();

		cameFromEditPhoto = false;
	}

	public void UpdateData ()
	{
		loadingSystem.ShowLoadingDownload ();
		LoadConfigs ();
	}
	

}



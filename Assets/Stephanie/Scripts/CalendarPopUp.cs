using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public class CalendarPopUp : MonoBehaviour
{
	public GameObject childPanel;
	public UIWidget dayLabelPrefab;
	public UIWidget widgetContainer;
	public UILabel HeaderLabel;         //The label used to show the Month
	public LoadingFeedback loadingSystem;
	public MemoriesConfig memoriesConfig;
	public string calendarMonth;
	public string calendarYear;
	public CurrenChild currentChild;

	private List<UIWidget> dayLabelsList;         //Holds 42 labels - need 42 it's more right
	private List<string> monthsBR = new List<string> ();
	private int monthCounter = DateTime.Now.Month - 1;
	private int yearCounter = 0;
	private DateTime iMonth;
	private DateTime curDisplay;

	string myURL;
	string fileName;

	bool isActive;

	Boolean firstTimeOpeningCalendar = true;

	void Update(){
		if (isActive) {
			Vector3 newPos = new Vector3 (0, 0, 0);
			transform.localPosition = newPos;
		} else {
			Vector3 newPos = new Vector3 (900, 0, 0);
			transform.localPosition = newPos;
		}
	}

	void LoadConfigs(){
		monthsBR.Add ("Janeiro");
		monthsBR.Add ("Fevereiro");
		monthsBR.Add ("Março");
		monthsBR.Add ("Abril");
		monthsBR.Add ("Maio");
		monthsBR.Add ("Junho");
		monthsBR.Add ("Julho");
		monthsBR.Add ("Agosto");
		monthsBR.Add ("Setembro");
		monthsBR.Add ("Outubro");
		monthsBR.Add ("Novembro");
		monthsBR.Add ("Dezembro");
		
		CreateDays ();
		
		clearLabels ();
		CreateMonths (2015); 
		
		iMonth = new DateTime (DateTime.Now.Year, DateTime.Now.Month, 1);
		//iMonth = new DateTime (2015,1,1);
		
		CreateCalendar (iMonth);
		CheckCurrentDayLayout ();
		//nextMonth ();
	}

	void CreateDays ()
	{
		dayLabelsList = new List<UIWidget> ();

		for (int i = 0; i < 6; i++) {
			for (int j = 0; j < 7; j++) {
				Vector3 newPos = new Vector3 (-300 + (j * 100), -150 + (i * -100), 1);
				Vector3 newSize = new Vector3 (1, 1, 1);
				UIWidget tempDaylabel = Instantiate (dayLabelPrefab, new Vector3 (0, 0, 0), Quaternion.identity) as UIWidget;
				tempDaylabel.GetComponent<BoxCollider>().enabled = false;
				dayLabelsList.Add (tempDaylabel);
				tempDaylabel.transform.parent = widgetContainer.transform;
				tempDaylabel.transform.localPosition = newPos;
				tempDaylabel.transform.localScale = newSize;



				EventDelegate.Add(tempDaylabel.GetComponent<UIButton>().onClick, delegate () { memoriesConfig.OpenMemories(
						tempDaylabel.GetComponentInChildren<UILabel>().text,
						calendarMonth,
						calendarYear
						);});
			}
		}
	}
	
	void CreateMonths (int year)
	{
		//Months = new string[12];
		
		for (int i = 0; i < 12; ++i) {
			iMonth = new DateTime (year, i + 1, 1);
			//Months[i] = iMonth.ToString ("MMMM");
		}
		HeaderLabel.text = monthsBR [DateTime.Now.Month - 1] + " " + DateTime.Now.Year;
	}
	
	void CreateCalendar (DateTime month)
	{
		curDisplay = month;
		
		int curDays = GetDays (curDisplay.DayOfWeek);
		int index = 0;

		if (curDays > 0)
			index = (curDays - 1);
		else
			index = curDays;

		while (curDisplay.Month == iMonth.Month) {
			dayLabelsList [index].GetComponentInChildren<UILabel> ().text = curDisplay.Day.ToString ();
			dayLabelsList [index].GetComponentInChildren<BoxCollider> ().enabled = true;
			curDisplay = curDisplay.AddDays (1);
			index++;
		}
	}
	
	private int GetDays (DayOfWeek day)
	{
		switch (day) {
		case DayOfWeek.Sunday:
			return 1;
		case DayOfWeek.Monday:
			return 2;
		case DayOfWeek.Tuesday:
			return 3;
		case DayOfWeek.Wednesday:
			return 4;
		case DayOfWeek.Thursday:
			return 5;
		case DayOfWeek.Friday:
			return 6;
		case DayOfWeek.Saturday:
			return 7;
		//case DayOfWeek.Sunday: return 7;
		default:
			throw new Exception ("Unexpected DayOfWeek: " + day);
		}
	}
	
	public void nextMonth ()
	{
		monthCounter++;
		if (monthCounter > 11) {
			monthCounter = 0;
			yearCounter++;
		}
		//HeaderLabel.text = iMonth.Month - 1 + " " + (DateTime.Now.Year + yearCounter);
		HeaderLabel.text = monthsBR [monthCounter] + " " + (DateTime.Now.Year + yearCounter);
		//yearCounter = 0;
		clearLabels ();
		CheckCurrentDayLayout ();
		iMonth = iMonth.AddMonths (1);
		CreateCalendar (iMonth);
		ShowDatesWithMemories ();
	}
	
	public void previousMonth ()
	{
		monthCounter--;
		if (monthCounter < 0) {
			monthCounter = 11;
			yearCounter--;
		}
		
		//HeaderLabel.text = iMonth.Month - 1 + " " + (DateTime.Now.Year + yearCounter);
		HeaderLabel.text = monthsBR [monthCounter] + " " + (DateTime.Now.Year + yearCounter);
		//yearCounter = 0;
		clearLabels ();
		CheckCurrentDayLayout ();
		iMonth = iMonth.AddMonths (-1);
		CreateCalendar (iMonth);
		ShowDatesWithMemories ();
	}

	private void CheckCurrentDayLayout(){
		calendarMonth = (monthCounter + 1).ToString();
		calendarYear = (DateTime.Now.Year + yearCounter).ToString();

		if ((monthCounter + 1) == DateTime.Now.Month) {
			for (int i = 0; i < dayLabelsList.Count; i++) {
				if(dayLabelsList[i].GetComponentInChildren<UILabel>().text != ""){
					if(int.Parse(dayLabelsList[i].GetComponentInChildren<UILabel>().text) > DateTime.Now.Day){
						//dayLabelsList[i].GetComponentInChildren<UILabel>().color = Color.gray;
					}
				}
			}
		}
	}
	
	/*clears all the day labels*/
	
	void clearLabels ()
	{
		for (int x = 0; x < dayLabelsList.Count; x++) {
			dayLabelsList [x].GetComponentInChildren<UILabel> ().text = "";
			dayLabelsList [x].GetComponentInChildren<UISprite> ().alpha = 0;
		}
	}

	public void ShowCalendar (string childName, Texture2D childPhoto)
	{
		isActive = true;

		Vector3 newPos = new Vector3 (0, 0, 0);
		transform.localPosition = newPos;

		Vector3 newPos2 = new Vector3 (-900, 0, 0);
		childPanel.transform.localPosition = newPos2;

		if (firstTimeOpeningCalendar) {
			firstTimeOpeningCalendar = false;
			LoadConfigs ();
		}

		myURL = Config.masterURL;
		fileName = Config.currentChild;
		
		myURL += "?webfilename=" + fileName +
			"&webusername=" + Config.masterUser +
				"&webpassword=" + Config.masterPass;

		currentChild.UpdateData (childName, childPhoto);
		UpdateData ();

	}

	public void CloseCalendar ()
	{
		isActive = false;

		Vector3 newPos = new Vector3 (900, 0, 0);
		transform.localPosition = newPos;

		Vector3 newPos2 = new Vector3 (0, 0, 0);
		childPanel.transform.localPosition = newPos2;

		currentChild.closeCurrentChild ();
	}

	public void UpdateData(){
		StartCoroutine ("DownloadDatesMemories");
	}

	public IEnumerator DownloadDatesMemories ()
	{
		loadingSystem.ShowLoadingDownload ();
		Debug.Log ("Initing downloading dateMemories...");
		ES2Web web = new ES2Web (myURL + "&tag=memoriesDates");
		
		yield return StartCoroutine (web.Download ());
		
		if (web.isError) {
			Debug.LogError (web.errorCode + ":" + web.error);
			loadingSystem.CloseLoading ();
			
			//TODO
			//Make user go back to login if error
			
		} else {
			loadingSystem.CloseLoading();
			Config.currentChildDatesCalendarList.Clear();
			Config.currentChildDatesCalendarList = web.LoadList<ArrayWrapper> ("memoriesDates");
			
			Debug.Log ("Finished downloading memory text...");
		}

		ShowDatesWithMemories ();
	}

	private void ShowDatesWithMemories ()
	{
		for (int x = 0; x < dayLabelsList.Count; x++) {
			dayLabelsList [x].GetComponentInChildren<UISprite> ().alpha = 0;
		}

		for (int i = 0; i < dayLabelsList.Count; i++) {
			for (int j = 0; j < Config.currentChildDatesCalendarList.Count; j++) {
				//Check if Year is same
				if (calendarYear == Config.currentChildDatesCalendarList [j].array [2]) {
					//Check if Month is same
					if (calendarMonth == Config.currentChildDatesCalendarList [j].array [1]) {
						//Check if Days exists
						if (dayLabelsList [i].GetComponentInChildren<UILabel> ().text == Config.currentChildDatesCalendarList [j].array [0]) {
							//Show square;
							dayLabelsList [i].GetComponentInChildren<UISprite> ().alpha = 0.2f;
							Debug.Log( "Entrou Aqui o/////");
						}
					}
				}
			}
		}
	}
}




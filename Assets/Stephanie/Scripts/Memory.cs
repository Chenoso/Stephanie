using UnityEngine;
using System.Collections;

public class Memory : MonoBehaviour {

	public UILabel memoryTitle;
	public UILabel memoryDateTime;
	public UITexture memoryPhoto;
	public Texture2D currentTexture2D;

	public string currentTitle;
	public string currentDescription;
	public string currentDateTime;


	public int memoryIndex;
	public string memoryDay;
	public string memoryMonth;
	public string memoryYear;
	public string memoryTime;

	public void CreateMemoryText(int index, string title, string description, string day, string month, string year, string time){
		if (int.Parse(day) < 10) {
			day = "0" + day;
		}
		if (int.Parse(month) < 10) {
			month = "0" + month;
		}

		currentTitle = title;
		currentDescription = description;

		memoryTitle.text = title;
		memoryDateTime.text = 
			day + "/" +
			month + "/" + 
			year + " - " + 
			time;

		currentDateTime = memoryDateTime.text;

		memoryIndex = index;
		memoryDay = day;
		memoryMonth = month;
		memoryYear = year;
		memoryTime = time;
	}
	public void CreateMemoryPhoto(Texture2D photo){
		DestroyImmediate (memoryPhoto.mainTexture, true);

		memoryPhoto.mainTexture = photo;
		currentTexture2D = photo;

		memoryPhoto.MakePixelPerfect ();
		memoryPhoto.keepAspectRatio = UIWidget.AspectRatioSource.BasedOnWidth;
		memoryPhoto.width = 750;

		transform.GetComponent<UIWidget> ().SetDimensions (750, memoryPhoto.height);

		Resources.UnloadUnusedAssets ();
		System.GC.Collect ();
	}
}

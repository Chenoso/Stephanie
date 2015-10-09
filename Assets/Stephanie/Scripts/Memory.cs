using UnityEngine;
using System.Collections;

public class Memory : MonoBehaviour {

	public UILabel memoryTitle;
	public UILabel memoryDateTime;
	public UITexture memoryPhoto;

	public string currentTitle;
	public string currentDescription;
	public string currentDateTime;
	public Texture2D currentTexture2D;

	public void CreateMemoryText(string title, string description, string day, string month, string year, string time){
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
	}
	public void CreateMemoryPhoto(Texture2D photo){
		memoryPhoto.mainTexture = photo;
		currentTexture2D = photo;
	}
}

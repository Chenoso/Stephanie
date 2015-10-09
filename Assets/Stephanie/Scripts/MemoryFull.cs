using UnityEngine;
using System.Collections;

public class MemoryFull : MonoBehaviour {

	public UILabel title;
	public UILabel description;
	public UITexture myPhoto;
	public UILabel dateTime;

	public UISprite blackBG;
	public UISprite blackBG2;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OpenMemoryFull(){
		Vector3 newPos = new Vector3 (0, 0, 0);
		transform.localPosition = newPos;

	}

	public void CloseMemoryFull(){
		Vector3 newPos = new Vector3 (2700, 0, 0);
		transform.localPosition = newPos;

	}

	public void UpdateData(Memory currentMemory){
		OpenMemoryFull ();

		title.text = currentMemory.currentTitle;
		description.text = currentMemory.currentDescription;
		myPhoto.mainTexture = currentMemory.currentTexture2D;
		dateTime.text = currentMemory.currentDateTime;
	}

	public void MakeBlackBackground(){
		Vector3 newPos = new Vector3 (0, 0, 0);
		blackBG.transform.localPosition = newPos;
		blackBG2.transform.localPosition = newPos;
	}

	public void MakeWhiteBackground(){
		Vector3 newPos = new Vector3 (0, 1500, 0);
		blackBG.transform.localPosition = newPos;
		blackBG2.transform.localPosition = newPos;
	}
}

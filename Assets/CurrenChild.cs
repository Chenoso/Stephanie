using UnityEngine;
using System.Collections;

public class CurrenChild : MonoBehaviour {

	public UITexture childPhotoTexture;
	public UILabel childNameLabel;

	string currentName;
	bool hasChanged;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateData(string childName, Texture2D childPhoto){
		Vector3 newPos = new Vector3 (0, 0, 0);
		transform.localPosition = newPos;

		childPhotoTexture.mainTexture = childPhoto;
		childNameLabel.text = childName;

		currentName = childName;
		hasChanged = false;
	}

	public void showCurrentChild(){
		Vector3 newPos = new Vector3 (0, 0, 0);
		transform.localPosition = newPos;

		if (hasChanged) {
			childNameLabel.text = currentName;
			hasChanged = false;
		}
	}

	public void closeCurrentChild(){
		Vector3 newPos = new Vector3 (900, -300, 0);
		transform.localPosition = newPos;
	}

	public void onChangeEditMode(string newName){
		if(newName != currentName){
			hasChanged = true;
		}
		childNameLabel.text = newName;
	}
}

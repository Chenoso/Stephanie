using UnityEngine;
using System.Collections;

public class LoadingFeedback : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	
	public void ShowLoadingDownload ()
	{
		Vector3 newPosX = new Vector3 (0, 0, 0);
		transform.localPosition = newPosX;
		GetComponentInChildren<UILabel> ().text = "Fazendo download de dados...";
	}

	public void ShowLoadingUpload()
	{
		Vector3 newPosX = new Vector3 (0, 0, 0);
		transform.localPosition = newPosX;
		GetComponentInChildren<UILabel> ().text = "Enviado dados para o servidor...";
	}

	public void CloseLoading ()
	{
		Vector3 newPosX = new Vector3 (-900, 0, 0);
		transform.localPosition = newPosX;
	}

}

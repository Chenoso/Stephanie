using UnityEngine;
using System.Collections;

public class LoginSystem : MonoBehaviour {

	private string URL = "http://clientes.cheny.com.br/stephanie/check_score.php"; 
	private string hash = "hashcode";

	private string formNick = ""; 
	private string formPassword = ""; 

	public UIInput userInput;
	public UIInput userPass;

	public UIWidget loadingContainer;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClickLogin(){
		ShowLoading();
		formNick = userInput.value;
		formPassword = userPass.value;
		StartCoroutine("TryToLogin");
	}

	private void ShowLoading(){
		Vector3 newPosX = new Vector3(0,0,0);
		loadingContainer.transform.localPosition = newPosX;
	}

	private void CloseLoading(){
		Vector3 newPosX = new Vector3(900,0,0);
		loadingContainer.transform.localPosition = newPosX;
	}

	public IEnumerator TryToLogin(){
		Debug.Log("Trying to login...");

		WWWForm form = new WWWForm();
		form.AddField("myform_hash", hash);
		form.AddField("myform_nick", formNick);
		form.AddField("myform_pass", formPassword);

		WWW w = new WWW(URL, form);
		yield return w;

		if(w.error != null){
			Debug.Log(w.error);
			//TODO
			//Fazer erro de login/senha/banco/conexão
		}else{
			Debug.Log("Connected to database");
			Debug.Log(w.text);
			if(w.text.ToString() == "Login or password cant be empty."){
				//TODO
				//Fazer erro de login/senha/banco/conexão
				w.Dispose();
			}else{
				w.Dispose();
				LoadSelectChildScene();
			}
		}

		CloseLoading();
		formNick = ""; //just clean our variables
		formPassword = "";
	}

	private void LoadSelectChildScene(){
		Application.LoadLevel ("1.SelectChild");
	}
}


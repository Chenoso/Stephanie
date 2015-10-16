using UnityEngine;
using System.Net;
using System.Collections;
using System.IO;

public class LoginSystem : MonoBehaviour
{

	private string URL = "http://clientes.cheny.com.br/stephanie/check_score.php";
	private string hash = "hashcode";
	private string formNick = "";
	private string formPassword = "";
	public UIInput userInput;
	public UIInput userPass;
	public UIWidget loadingContainer;
	public UILabel feedbackLabel;
	bool hasInternet;

	// Use this for initialization
	void Start ()
	{
		string HtmlText = GetHtmlFromUri ("http://google.com");
		if (HtmlText == "") {
			Debug.Log("No internet");
			//No connection
		} else if (!HtmlText.Contains ("schema.org/WebPage")) {
			Debug.Log("No internet");
			//Redirecting since the beginning of googles html contains that 
			//phrase and it was not found
		} else {
			Debug.Log("Has internet");
			hasInternet = true;
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void OnClickLogin ()
	{
		if (hasInternet) {
			ShowLoading ();
			formNick = userInput.value;
			formPassword = userPass.value;
			StartCoroutine ("TryToLogin");
		} else {
			feedbackLabel.text = "Erro: Você não está conectado à internet";
		}
	}

	private void ShowLoading ()
	{
		Vector3 newPosX = new Vector3 (0, 0, 0);
		loadingContainer.transform.localPosition = newPosX;
	}

	private void CloseLoading ()
	{
		Vector3 newPosX = new Vector3 (900, 0, 0);
		loadingContainer.transform.localPosition = newPosX;
	}

	public IEnumerator TryToLogin ()
	{
		Debug.Log ("Trying to login...");

		WWWForm form = new WWWForm ();
		form.AddField ("myform_hash", hash);
		form.AddField ("myform_nick", formNick);
		form.AddField ("myform_pass", formPassword);

		WWW w = new WWW (URL, form);
		yield return w;

		if (w.error != null) {
			Debug.Log (w.error);
			//TODO
			//Fazer erro de login/senha/banco/conexão
		} else {
			Debug.Log ("Connected to database");
			Debug.Log (w.text);
			if (w.text.ToString () == "Login or password cant be empty. ") {
				feedbackLabel.text = "Erro: O usuário ou senha não podem estar em branco";
				w.Dispose ();

			} else if (w.text.ToString () == "Nick or password is wrong. ") {
				feedbackLabel.text = "Erro: Usuário ou senha inválidos";
				w.Dispose ();
			} else if (w.text.ToString () == "Data invalid - cant find name. ") {
				feedbackLabel.text = "Erro: Usuário não existe";
				w.Dispose ();
			} else if (w.text.ToString () == "OK ") {
				w.Dispose ();
				Config.user = formNick;
				LoadSelectChildScene ();
			} else {
				feedbackLabel.text = "Ocorreu um erro, tente novamente mais tarde";
				w.Dispose ();
			}
		}

		CloseLoading ();
		formNick = ""; //just clean our variables
		formPassword = "";
	}

	private void LoadSelectChildScene ()
	{
		Application.LoadLevel ("1.SelectChild");
	}

	public string GetHtmlFromUri (string resource)
	{
		string html = string.Empty;
		HttpWebRequest req = (HttpWebRequest)WebRequest.Create (resource);
		try {
			using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse()) {
				bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
				if (isSuccess) {
					using (StreamReader reader = new StreamReader(resp.GetResponseStream())) {
						//We are limiting the array to 80 so we don't have
						//to parse the entire html document feel free to 
						//adjust (probably stay under 300)
						char[] cs = new char[80];
						reader.Read (cs, 0, cs.Length);
						foreach (char ch in cs) {
							html += ch;
						}
					}
				}
			}
		} catch {
			return "";
		}
		return html;
	}

}


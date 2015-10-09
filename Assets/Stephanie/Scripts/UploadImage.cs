using UnityEngine;
using System.Collections;

public class UploadImage : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartUploadingText(){
		Debug.Log("Starting upload text");
		StartCoroutine("UploadText");
	}

	public void StartUploading(){
		Debug.Log("Starting upload");
		StartCoroutine("UploadTexture2D");
	}

	public void StartDownloading(){
		Debug.Log("Starting download");
		StartCoroutine("DownloadTexture2D");
	}

	public void StartDownloadingText(){
		Debug.Log("Starting download text");
		StartCoroutine("DownloadText");
	}
	
	public IEnumerator UploadTexture2D()
	{
		Debug.Log("Uploaded initiated");
		// Create a URL and add parameters to the end of it.
		string myURL = "http://clientes.cheny.com.br/stephanie/es2.php";
		myURL += "?webfilename=image.png&webusername=cheny&webpassword=chenyrox";
		
		// Create our ES2Web object.
		Texture2D texture = GetComponent<UITexture>().mainTexture as Texture2D;

		ES2Web web = new ES2Web(myURL);
		yield return StartCoroutine( web.UploadImage(texture) );
		
		if(web.isError)
		{
			// Enter your own code to handle errors here.
			Debug.LogError(web.errorCode + ":" + web.error);
		}
	}

	public IEnumerator DownloadTexture2D()
	{
		// Create a URL and add parameters to the end of it.
		string myURL = "http://clientes.cheny.com.br/stephanie/es2.php";
		myURL += "?webfilename=image.png&webusername=cheny&webpassword=chenyrox";
		
		// Create our ES2Web object.
		ES2Web web = new ES2Web(myURL);
		
		// Start downloading our data and wait for it to finish.
		yield return StartCoroutine(web.Download());
		
		if(web.isError)
		{
			// Enter your own code to handle errors here.
			Debug.LogError(web.errorCode + ":" + web.error);
		}
		else
		{
			// We could save our data to a local file and load from that.
			//web.SaveToFile("myFile.txt");
			
			// Or we could just load directly from the ES2Web object.
			//this.GetComponent<MeshFilter>().mesh = web.Load<Mesh>(tag);
			GetComponent<UITexture>().mainTexture = web.LoadImage();
		}
	}

	public IEnumerator UploadText()
	{
		Debug.Log("Uploading initiated");
		// Create a URL and add parameters to the end of it.
		string myURL = "http://clientes.cheny.com.br/stephanie/es2.php";
		myURL += "?webfilename=text.txt&webusername=cheny&webpassword=chenyrox";
		
		// Create our ES2Web object.
		string textUploaded = "Sei fazer upload, editado";
		
		ES2Web web = new ES2Web(myURL + "&tag=filho1texto1234");
		yield return StartCoroutine(web.Upload(textUploaded));
		
		if(web.isError)
		{
			// Enter your own code to handle errors here.
			Debug.LogError(web.errorCode + ":" + web.error);
		}
	}

	public IEnumerator DownloadText()
	{
		// Create a URL and add parameters to the end of it.
		string myURL = "http://clientes.cheny.com.br/stephanie/es2.php";
		myURL += "?webfilename=text.txt&webusername=cheny&webpassword=chenyrox";
		
		// Create our ES2Web object.
		ES2Web web = new ES2Web(myURL + "&tag=texto1234");
		
		// Start downloading our data and wait for it to finish.
		yield return StartCoroutine(web.Download());
		
		if(web.isError)
		{
			// Enter your own code to handle errors here.
			Debug.LogError(web.errorCode + ":" + web.error);
		}
		else
		{
			// We could save our data to a local file and load from that.
			//web.SaveToFile("myFile.txt");
			
			// Or we could just load directly from the ES2Web object.
			//this.GetComponent<MeshFilter>().mesh = web.Load<Mesh>(tag);
			GetComponent<UILabel>().text = web.Load<string>("texto1234");
		}
	}
	
}

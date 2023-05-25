using System;
using System.Text;
using System.IO;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;

public class ImageSpawner : MonoBehaviour {
	
	public GameObject prefab;
	public Transform canvas;
	public Animator animator;
	public TMP_InputField inputField;

	private bool pending = false;

	void Start() {

	}

	void Update() {
		animator.SetBool("Wait", pending);
	}

	public void sendRequest() {
		if (pending || inputField.text.Length == 0) {
			return;
		}
		pending = true;
		StartCoroutine(Request(inputField.text));
	}

	private class RequestContent {
		public string prompt;
		public RequestContent(string p) {
			prompt = p;
		}
	}
	private class ResponseContent {
		public string fileName;
	}

	private IEnumerator Request(string prompt) {
		string tunedPrompt = "A photography of " + prompt.ToLower();
		RequestContent obj = new RequestContent(tunedPrompt);
		string data = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
		byte[] bodyRaw = Encoding.UTF8.GetBytes(data);

		using (UnityWebRequest request = new UnityWebRequest("http://127.0.0.1:5000/diffuse")) {
			request.SetRequestHeader("Content-Type", "application/json");
			request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
			request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
			request.method = UnityWebRequest.kHttpVerbPOST;
			yield return request.SendWebRequest();

			if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError) {
				Debug.Log(request.error);
				pending = false;
			}
			else {
				string responseJson = request.downloadHandler.text;
				ResponseContent responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<ResponseContent>(responseJson);
				string result = responseObject.fileName;
				GameObject newImage = Instantiate(prefab, canvas);
				newImage.GetComponent<ImageObject>().updateImage(result, prompt);
				inputField.text = "";
				pending = false;
			}
		}
	}
}

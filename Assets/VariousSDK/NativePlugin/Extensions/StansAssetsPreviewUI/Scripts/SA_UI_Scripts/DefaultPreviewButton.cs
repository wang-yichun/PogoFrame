using UnityEngine;
using System;
using System.Collections;

public class DefaultPreviewButton : MonoBehaviour {

	public event Action ActionClick =  delegate {};

	public Texture normalTexture;
	public Texture pressedTexture;
	public Texture disabledTexture;


	public Texture selectedTexture;
	private Texture normalTex;

	public AudioClip sound;
	public AudioClip disabledsound;


	private bool IsDisabled = false;


	void Awake() {
		if(GetComponent<AudioSource>() == null) {
			gameObject.AddComponent<AudioSource>();
			GetComponent<AudioSource>().clip = sound;
			GetComponent<AudioSource>().Stop();
		}

		GetComponent<Renderer>().material =  new Material(GetComponent<Renderer>().material);
		normalTex = normalTexture;
	}


	public void Select() {
		normalTexture = selectedTexture;
		OnTimeoutPress();

	}

	public void Unselect() {
		normalTexture = normalTex;
		OnTimeoutPress();
	}

	public void DisabledButton() {
		if(disabledTexture != null) {
			GetComponent<Renderer>().material.mainTexture = disabledTexture;
		}
		IsDisabled = true;
	}

	public void EnabledButton() {
		if(disabledTexture != null) {
			GetComponent<Renderer>().material.mainTexture = normalTexture;
		}
		IsDisabled = false;
	}



	public string text {
		get {
			TextMesh mesh  = gameObject.GetComponentInChildren<TextMesh>();
			return mesh.text;
		}

		set {
			TextMesh[] meshes  = gameObject.GetComponentsInChildren<TextMesh>();
			foreach(TextMesh mesh in meshes) {
				mesh.text = value;
			}
		}
	}


	void Update() {

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Input.GetMouseButtonDown(0)){

			
			if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
				if(hit.transform.gameObject == gameObject) {
					OnClick();
				}
			}
		} 

	}


	protected virtual void OnClick() {
		if(IsDisabled) {
			GetComponent<AudioSource>().PlayOneShot(disabledsound);
			return;
		} 
		GetComponent<AudioSource>().PlayOneShot(sound);
		GetComponent<Renderer>().material.mainTexture = pressedTexture;
		ActionClick();
		CancelInvoke("OnTimeoutPress");
		Invoke("OnTimeoutPress", 0.1f);
	}

	private void OnTimeoutPress() {
		GetComponent<Renderer>().material.mainTexture = normalTexture;
	}
}

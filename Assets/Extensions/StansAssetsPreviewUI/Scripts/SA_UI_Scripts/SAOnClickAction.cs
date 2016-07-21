using UnityEngine;
using System.Collections;

public abstract class SAOnClickAction : MonoBehaviour {

	void Awake() {
		DefaultPreviewButton btn = GetComponent<DefaultPreviewButton>();
		if(btn != null) {
			btn.ActionClick += OnClick;
		}
	}
	
	protected abstract void OnClick();
}


using UnityEngine;
using System;
using System.Collections;

public class AN_PlusShareListener : MonoBehaviour {
	public delegate void PlusShareCallback(AN_PlusShareResult result);
	private PlusShareCallback builderCallback = delegate {};

	public void AttachBuilderCallback(PlusShareCallback callback) {
		builderCallback = callback;
	}
	
	private void OnPlusShareCallback(string res) {
		AN_PlusShareResult result = new AN_PlusShareResult(Boolean.Parse(res));
		builderCallback(result);
	}
}

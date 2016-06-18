#if !UNITY_WEBPLAYER
using System.IO;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

[System.Serializable]
[ExecuteInEditMode]
public partial class RageCamera : MonoBehaviour {

    [SerializeField]private Camera _camera;
	private bool _started;

    public void OnEnable() {
		if (_started) return;
		_camera = GetComponent<Camera>();
		_camera.transparencySortMode = TransparencySortMode.Orthographic;
		_started = true;
	}

}

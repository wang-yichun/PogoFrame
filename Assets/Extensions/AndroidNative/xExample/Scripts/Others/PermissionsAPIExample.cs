using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PermissionsAPIExample : MonoBehaviour {


	void Awake() {
		PermissionsManager.ActionPermissionsRequestCompleted += HandleActionPermissionsRequestCompleted;
	}



	public void CheckPermission() {
		Debug.Log("CheckPermission");

		bool val = PermissionsManager.IsPermissionGranted(AN_ManifestPermission.WRITE_EXTERNAL_STORAGE);
		Debug.Log(val);


		val = PermissionsManager.IsPermissionGranted(AN_ManifestPermission.INTERNET);
		Debug.Log(val);
	}

	public void RequestPermission() {
		PermissionsManager.Instance.RequestPermissions(AN_ManifestPermission.WRITE_EXTERNAL_STORAGE, AN_ManifestPermission.CAMERA);
	}



	void HandleActionPermissionsRequestCompleted (AN_GrantPermissionsResult res) {
		Debug.Log("HandleActionPermissionsRequestCompleted");
		
		foreach(KeyValuePair<AN_ManifestPermission, AN_PermissionState> pair in res.RequestedPermissionsState) {
			Debug.Log(pair.Key.GetFullName() + " / " + pair.Value.ToString());
		}
		
	}
}

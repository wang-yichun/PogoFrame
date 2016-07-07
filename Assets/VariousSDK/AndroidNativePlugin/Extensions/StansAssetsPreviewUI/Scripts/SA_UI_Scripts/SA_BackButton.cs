using UnityEngine;
using System.Collections;

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#else
using UnityEngine.SceneManagement;
#endif

public class SA_BackButton : DefaultPreviewButton {

	public static string firstLevel = string.Empty;

	void Start() {

		if(firstLevel != string.Empty) {
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
		if(firstLevel == string.Empty) {

			firstLevel = LevelName;


		}
	}


	void FixedUpdate() {
		if(LevelName.Equals(firstLevel)) {
			GetComponent<Renderer>().enabled = false;
			GetComponent<Collider>().enabled = false;
		} else {
			GetComponent<Renderer>().enabled = true;
			GetComponent<Collider>().enabled = true;
		}
	}

	protected override void OnClick() {
		base.OnClick();
		GoBack();
	}

	private void GoBack() {
		SALevelLoader.instance.LoadLevel(firstLevel);
	}

	public string LevelName {
		get {
			#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			return Application.loadedLevelName;
			#else
			return SceneManager.GetActiveScene().name;
			#endif
		}
	}
}

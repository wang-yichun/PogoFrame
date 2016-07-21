using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#else
using UnityEngine.SceneManagement;
#endif


//Attach the script to the empty gameobject on your sceneS
public class AndroidAdMobBannerInterstitial : MonoBehaviour {


	public string InterstitialUnityId;


	// --------------------------------------
	// Unity Events
	// --------------------------------------
	
	void Awake() {

		if(AndroidAdMobController.instance.IsInited) {
			if(!AndroidAdMobController.instance.InterstisialUnitId.Equals(InterstitialUnityId)) {
				AndroidAdMobController.instance.SetInterstisialsUnitID(InterstitialUnityId);
			} 
		} else {
			AndroidAdMobController.instance.Init(InterstitialUnityId);
		}


	}

	void Start() {
		ShowBanner();
	}




	// --------------------------------------
	// PUBLIC METHODS
	// --------------------------------------

	public void ShowBanner() {
		AndroidAdMobController.instance.StartInterstitialAd();
	}



	// --------------------------------------
	// GET / SET
	// --------------------------------------



	public string sceneBannerId {
		get {
			#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			return Application.loadedLevelName + "_" + this.gameObject.name;
			#else
			return SceneManager.GetActiveScene().name + "_" + this.gameObject.name;
			#endif
		}
	}

	
}

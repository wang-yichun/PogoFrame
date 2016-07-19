namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System;

	#if UNITY_ADS
	using UnityEngine.Advertisements;
	#endif

	public class Ad_UnityAds : IPogoAdsXCommon
	{

		const string RewardedZoneId = "rewardedVideo";

		#region IPogoAdsXCommon implementation

		public string Key {
			get;
			set;
		}

		public string IOSAppId {
			get;
			set;
		}

		public string AndroidAppId {
			get;
			set;
		}

		public void Init ()
		{
			#if UNITY_ADS

			#if UNITY_IOS
			Advertisement.Initialize (IOSAppId);
			#elif UNITY_ANDROID
			Advertisement.Initialize (AndroidAppId);
			#endif

			#endif
		}

		public bool IsReady ()
		{
			#if UNITY_ADS

			return Advertisement.IsReady ();

			#else
			return false;
			#endif
		}

		public void Show ()
		{
			#if UNITY_ADS

			var options = new ShowOptions {
				resultCallback = HandleShowResult
			};
			Advertisement.Show (RewardedZoneId, options);

			#endif
		}

		public Action<string, PogoAdsxShowResult> ShowResultCallback {
			get;
			set;
		}

		#endregion


		#if UNITY_ADS
		private void HandleShowResult (ShowResult result)
		{
			switch (result) {
			case ShowResult.Finished:
				Debug.Log ("The ad was successfully shown.");
				ShowResultCallback.Invoke (Key, PogoAdsxShowResult.Finished);
				break;
			case ShowResult.Skipped:
				Debug.Log ("The ad was skipped before reaching the end.");
				ShowResultCallback.Invoke (Key, PogoAdsxShowResult.Skipped);
				break;
			case ShowResult.Failed:
				Debug.LogError ("The ad failed to be shown.");
				ShowResultCallback.Invoke (Key, PogoAdsxShowResult.Failed);
				break;
			}
		}
		#endif
	}

}
namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System;

	#if SDK_Chartboost
	using ChartboostSDK;
	#endif

	public class Ad_Chartboost : IPogoAdsXCommon
	{
		public string IOSAppSign {
			get;
			set;
		}

		public string AndroidAppSign {
			get;
			set;
		}

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
			#if SDK_Chartboost

			#if UNITY_IPHONE
			Chartboost.CreateWithAppId (IOSAppId, IOSAppSign);
			#elif UNITY_ANDROID
			Chartboost.CreateWithAppId (AndroidAppId, AndroidAppSign);
			#endif

			Debug.Log ("Ad_Chartboost - Is Initialized: " + Chartboost.isInitialized ());

			Chartboost.didInitialize += didInitialize;
			Chartboost.didDismissRewardedVideo += didDismissRewardedVideo;
			Chartboost.didCloseRewardedVideo += didCloseRewardedVideo;
			Chartboost.didClickRewardedVideo += didClickRewardedVideo;
			Chartboost.didCacheRewardedVideo += didCacheRewardedVideo;
			Chartboost.didFailToLoadRewardedVideo += didFailToLoadRewardedVideo;
			Chartboost.didCompleteRewardedVideo += didCompleteRewardedVideo;

			Chartboost.setAutoCacheAds (true);
			Chartboost.setMediation (CBMediation.AdMob, "1.0");

			#endif
		}

		public bool IsReady ()
		{
			#if SDK_Chartboost

			return Chartboost.hasRewardedVideo (CBLocation.Default);

			#else
			return false;
			#endif
		}

		public void Show ()
		{
			#if SDK_Chartboost
			Chartboost.showRewardedVideo (CBLocation.Default);
			#endif
		}

		public Action<string, PogoAdsxShowResult> ShowResultCallback {
			get;
			set;
		}

		#endregion

		#if SDK_Chartboost

		void didInitialize (bool status)
		{
			Debug.LogFormat ("Chartboost didInitialize: {0}", status);
			Chartboost.cacheRewardedVideo (CBLocation.Default);
		}

		void didCacheRewardedVideo (CBLocation location)
		{
			Debug.LogFormat ("Chartboost didCacheRewardedVideo: {0}", location);
		}

		void didCloseRewardedVideo (CBLocation location)
		{
			Debug.LogFormat ("Chartboost didCloseRewardedVideo: {0}", location);
		}

		void didClickRewardedVideo (CBLocation location)
		{
			Debug.LogFormat ("Chartboost didClickRewardedVideo: {0}", location);
		}

		void didDismissRewardedVideo (CBLocation location)
		{
			Debug.LogFormat ("Chartboost didDismissRewardedVideo: {0}", location);
			ShowResultCallback.Invoke (Key, PogoAdsxShowResult.Skipped);
		}

		void didFailToLoadRewardedVideo (CBLocation location, CBImpressionError error)
		{
			Debug.LogFormat ("Chartboost didFailToLoadRewardedVideo: {0} | {1}", location, error);
			ShowResultCallback.Invoke (Key, PogoAdsxShowResult.Failed);
		}

		void didCompleteRewardedVideo (CBLocation location, int reward)
		{
			Debug.LogFormat ("Chartboost didCompleteRewardedVideo: {0} | {1}", location, reward);
			ShowResultCallback.Invoke (Key, PogoAdsxShowResult.Finished);
		}
		#endif

		//		#if UNITY_ADS
		//		private void HandleShowResult (ShowResult result)
		//		{
		//			switch (result) {
		//			case ShowResult.Finished:
		//				Debug.Log ("The ad was successfully shown.");
		//				ShowResultCallback.Invoke (Key, PogoAdsxShowResult.Finished);
		//				break;
		//			case ShowResult.Skipped:
		//				Debug.Log ("The ad was skipped before reaching the end.");
		//				ShowResultCallback.Invoke (Key, PogoAdsxShowResult.Skipped);
		//				break;
		//			case ShowResult.Failed:
		//				Debug.LogError ("The ad failed to be shown.");
		//				ShowResultCallback.Invoke (Key, PogoAdsxShowResult.Failed);
		//				break;
		//			}
		//		}
		//		#endif
	}

}
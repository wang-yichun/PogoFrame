namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System;
	using UniRx;

	#if SDK_JOYINGMOBI
	using pogorock.Joying;
	using UnityEngine.Advertisements;
	#endif


	public class Ad_JoyingAds : IPogoAdsXCommon
	{

		//	const string RewardedZoneId = "rewardedVideo";
		public string IOSAppKey {
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
			#if SDK_JOYINGMOBI

			#if UNITY_IOS
			Debug.Log ("joying init");
			Joying_Utility.OnVideoPlayCallback_IsFinishPlay += OnVideoPlayCallback_IsFinishPlay;
			Joying_Utility.OnVideoPlayCallback_IsLegal += OnVideoPlayCallback_IsLegal;
			Joying_Utility.OnVideoPlayCallback_HasCanPlay += OnVideoPlayCallback_HasCanPlay;
			Joying_Utility.Instance.InitAppID (IOSAppId, IOSAppKey);
			Observable.Timer (TimeSpan.FromSeconds (1)).Subscribe (_ => {
				Debug.Log ("joying has can play video ");
				Joying_Utility.Instance.VideoHasCanPlayVideo ();
			});
			#elif UNITY_ANDROID
		
			#endif

			#endif
		}

		private bool isReady;

		public bool IsReady ()
		{
			#if SDK_JOYINGMOBI

			return isReady;

			#else
			return false;
			#endif
		}

		public void Show ()
		{
			#if SDK_JOYINGMOBI
			Joying_Utility.Instance.VideoPlay_FullScreen ();
			Observable.Timer (TimeSpan.FromSeconds (1)).Subscribe (_ => {
				Debug.Log ("joying has can play video ");
				Joying_Utility.Instance.VideoHasCanPlayVideo ();
			});
			#endif
		}

		public Action<string, PogoAdsxShowResult> ShowResultCallback {
			get;
			set;
		}

		#endregion

		void OnVideoPlayCallback_IsFinishPlay (bool isFinishPlay)
		{
			Debug.Log ("joying finish callback=" + isFinishPlay);
			ShowResultCallback.Invoke (Key, PogoAdsxShowResult.Finished);
		}

		void OnVideoPlayCallback_IsLegal (bool isLegal)
		{
			Debug.Log ("joying isLegal callback=" + isLegal);
			ShowResultCallback.Invoke (Key, PogoAdsxShowResult.Failed);
		}

		void OnVideoPlayCallback_HasCanPlay (bool HasCanPlay)
		{
			Debug.Log ("joying has video=" + HasCanPlay);
			isReady = true;
		}
	}

}
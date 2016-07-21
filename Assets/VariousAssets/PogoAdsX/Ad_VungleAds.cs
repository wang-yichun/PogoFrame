using Newtonsoft.Json;
using System.Collections.Generic;

namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System;

	public class Ad_VungleAds : IPogoAdsXCommon
	{
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
			
			#if SDK_Vungle && (UNITY_IOS||UNITY_ANDROID)
			Vungle.onAdFinishedEvent += Vungle_onAdFinishedEvent;
			Vungle.adPlayableEvent += Vungle_adPlayableEvent;
			Vungle.init (AndroidAppId, IOSAppId);
			#endif
		}

		#if SDK_Vungle
		void Vungle_adPlayableEvent (bool obj)
		{
			isReady = obj;
		}

		void Vungle_onAdFinishedEvent (AdFinishedEventArgs obj)
		{
			if (obj.IsCompletedView) {
				ShowResultCallback.Invoke (Key, PogoAdsxShowResult.Finished);
			} else {
				ShowResultCallback.Invoke (Key, PogoAdsxShowResult.Failed);
			}
		}
		#endif

		private bool isReady;

		public bool IsReady ()
		{
			#if SDK_Vungle

		
			return isReady;
			#else
			return false;
			#endif
		}

		public void Show ()
		{
			#if SDK_Vungle

			Dictionary<string, object> options = new Dictionary<string, object> ();
			Vungle.playAdWithOptions (options);

			#endif
		}

		public Action<string, PogoAdsxShowResult> ShowResultCallback {
			get;
			set;
		}

		#endregion


	}

}
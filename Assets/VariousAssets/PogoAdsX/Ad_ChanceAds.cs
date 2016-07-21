namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System;


	#if SDK_CHANCEAD
	using pogorock.ChanceAd;
	#endif

	using UniRx;

	public class Ad_ChanceAds : IPogoAdsXCommon
	{

		#if SDK_CHANCEAD
		public string Chance_PublisherID {
			get { 
				return Chance_Utility.Instance.chance_publisherID;
			}
			set { 
				Chance_Utility.Instance.chance_publisherID = value;
			}
		}

		public string Chance_PlacementId {
			get { 
				return Chance_Utility.Instance.chance_placementID;
			}
			set { 
				Chance_Utility.Instance.chance_placementID = value;
			}
		}
		#else
		public string Chance_PublisherID {
			get;
			set;
		}
		public string Chance_PlacementId {
			get;
			set;
		}
		#endif

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
			#if SDK_CHANCEAD

			#if UNITY_IOS
			Chance_Utility.Instance.OnVideoPlayCompleteCallBack_IsComplete += OnPlayCompletedCallback;
			Chance_Utility.Instance.OnVideoHasVideoCallback_IsHas += OnHasVideoCallback;
			Chance_Utility.Instance.Init ();

			Observable.Timer (TimeSpan.FromSeconds (1)).Subscribe (_ => {
				Chance_Utility.Instance.QueryVideoAD ();
			});
			#endif

			#endif
		}

		private bool isReady;

		public bool IsReady ()
		{
			#if SDK_CHANCEAD
			Debug.Log ("调用IsReady返回" + isReady);
			return isReady;

			#else
			return false;
			#endif
		}

		public void Show ()
		{
			#if SDK_CHANCEAD
			Debug.Log ("执行show");
			Chance_Utility.Instance.PlayVideoAD ();
			Chance_Utility.Instance.QueryVideoAD ();
			#endif
		}

		public Action<string, PogoAdsxShowResult> ShowResultCallback {
			get;
			set;
		}

		#endregion

		#if SDK_CHANCEAD
		public void OnPlayCompletedCallback (bool isCompleted)
		{
			Debug.Log ("OnPlayCompletedCallback: " + isCompleted);
			if (isCompleted) {
				ShowResultCallback.Invoke (Key, PogoAdsxShowResult.Finished);
			} else {
				ShowResultCallback.Invoke (Key, PogoAdsxShowResult.Failed);
			}
		}

		public void OnHasVideoCallback (bool hasVideo)
		{
			Debug.Log ("OnHasVideoCallback: " + hasVideo);
			isReady = hasVideo;
		}
		#endif
	}

}
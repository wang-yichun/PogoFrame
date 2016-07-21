namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using Newtonsoft.Json;
	using System.Linq;

	public class PogoAdsX : MonoBehaviour
	{
		private static PogoAdsX instance;

		public static PogoAdsX Instance {
			get {
				if (instance == null) {
					PogoAdsX[] ads = GameObject.FindObjectsOfType<PogoAdsX> ();
					for (int i = 0; i < ads.Length; i++) {
						PogoAdsX ad = ads [i];
						Destroy (ad.gameObject);
					}
					GameObject go = new GameObject ("PogoAdsX");
					instance = go.AddComponent<PogoAdsX> ();

				}
				return instance;
			}
		}

		public void Init ()
		{
			TextAsset textAsset = Resources.Load<TextAsset> ("PogoAdsXManagerConfig");
			List<PogoAdInfo> adList = JsonConvert.DeserializeObject<List<PogoAdInfo>> (textAsset.text);
			Ads = new Dictionary<string, IPogoAdsXCommon> ();

			for (int i = 0; i < adList.Count; i++) {
				PogoAdInfo info = adList [i];
				if (info.Enable) {
					switch (info.Key) {
					case "UnityAds":
						#if UNITY_ADS
						{
							IPogoAdsXCommon ipac = new Ad_UnityAds () {
								Key = info.Key,
								IOSAppId = info.iOSValue,
								AndroidAppId = info.androidValue,
								ShowResultCallback = OnShowResult
							};
							ipac.Init ();
							Ads.Add (info.Key, ipac);
						}
						#endif
						break;
					case "Chartboost":
						#if SDK_Chartboost
						{
							if (info.IsTest) {
								IPogoAdsXCommon ipac = new Ad_Chartboost () {
									Key = info.Key,
									IOSAppId = "4f21c409cd1cb2fb7000001b",
									AndroidAppId = "4f7b433509b6025804000002",
									IOSAppSign = "92e2de2fd7070327bdeb54c15a5295309c6fcd2d",
									AndroidAppSign = "dd2d41b69ac01b80f443f5b6cf06096d457f82bd",
									ShowResultCallback = OnShowResult
								};

								ipac.Init ();
								Ads.Add (info.Key, ipac);
							} else {
								IPogoAdsXCommon ipac = new Ad_Chartboost () {
									Key = info.Key,
									IOSAppId = info.iOSValue,
									AndroidAppId = info.androidValue,
									IOSAppSign = info.Params ["ios_app_signature"],
									AndroidAppSign = info.Params ["android_app_signature"],
									ShowResultCallback = OnShowResult
								};

								ipac.Init ();
								Ads.Add (info.Key, ipac);
							}
						}
						#endif
						break;
					case "ChanceAd":
						#if UNITY_IOS
						#if SDK_CHANCEAD
						{
							if (info.IsTest) {
								IPogoAdsXCommon ipac = new Ad_ChanceAds () {
									Key = info.Key,
									IOSAppId = info.iOSValue,
									AndroidAppId = info.androidValue,
									ShowResultCallback = OnShowResult,
									Chance_PublisherID = "100032-4CE817-ABA2-5B48-14D009296720",
									Chance_PlacementId = "100032o7ryik"
								};
								ipac.Init ();
								Ads.Add (info.Key, ipac);
							} else {
								IPogoAdsXCommon ipac = new Ad_ChanceAds () {
									Key = info.Key,
									IOSAppId = info.iOSValue,
									AndroidAppId = info.androidValue,
									ShowResultCallback = OnShowResult,
									Chance_PublisherID = info.Params ["publisher_id"],
									Chance_PlacementId = info.Params ["ios_placement_id"]
								};
								ipac.Init ();
								Ads.Add (info.Key, ipac);
							}
						}
						#endif
						#endif
						break;
					case "JoyingMobi":
						#if UNITY_IOS
						#if SDK_JOYINGMOBI
						{
							IPogoAdsXCommon ipac = new Ad_JoyingAds () {
								Key = info.Key,
								IOSAppId = info.iOSValue,
								AndroidAppId = info.androidValue,
								IOSAppKey = info.Params ["ios_app_key"],
								ShowResultCallback = OnShowResult,
							};
							ipac.Init ();
							Ads.Add (info.Key, ipac);
						}
						#endif
						#endif
						break;
					case "Vungle":
						#if UNITY_IOS||UNITY_ANDROID
						#if SDK_Vungle
						{
							IPogoAdsXCommon ipac = new Ad_VungleAds () {
								Key = info.Key,
								IOSAppId = info.iOSValue,
								AndroidAppId = info.androidValue,
								ShowResultCallback = OnShowResult,
							};
							ipac.Init ();
							Ads.Add (info.Key, ipac);
						}
						#endif
						#endif
						break;
					default:
						break;
					}
				}
			}
		}

		public Dictionary<string, IPogoAdsXCommon> Ads;

		public void OnShowResult (string key, PogoAdsxShowResult result)
		{
			Debug.Log (string.Format ("key: {0}, result: {1}", key, result));
		}

		/// <summary>
		/// 广告已准备好(至少有一个)
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public string IsReady ()
		{
			if (Ads != null) {
				foreach (var kvp in Ads) {
					if (kvp.Value.IsReady ()) {
						return kvp.Key;
					}
				}
			}
			return null;
		}

		public bool Show (string key)
		{
			if (string.IsNullOrEmpty (key) == false) {
				if (Ads.ContainsKey (key)) {
					if (Ads [key].IsReady ()) {
						Ads [key].Show ();
						return true;
					}
				}
			}
			return false;
		}
	}
}

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
						IPogoAdsXCommon ipac = new Ad_UnityAds () {
							IOSAppId = info.iOSValue,
							AndroidAppId = info.androidValue,
							ShowResultCallback = OnShowResult
						};
						ipac.Init ();
						Ads.Add (info.Key, ipac);
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
			Debug.Log (string.Format ("key: {0}, result: {1}.{2}", key, result, result.ToString ()));
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

		public void Show (string key)
		{
			Ads [key].Show ();
		}
	}
}
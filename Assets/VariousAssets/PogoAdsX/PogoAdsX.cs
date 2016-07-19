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

		public void loadConfigFile ()
		{
			TextAsset textAsset = Resources.Load<TextAsset> ("PogoAdsXManagerConfig");
			List<PogoAdInfo> adList = JsonConvert.DeserializeObject<List<PogoAdInfo>> (textAsset.text);
			Ads = adList.ToDictionary<PogoAdInfo, string> (info => info.Key);

			Debug.Log (JsonConvert.SerializeObject (Ads, Formatting.Indented));
		}

		public Dictionary<string, PogoAdInfo> Ads;
	}
}
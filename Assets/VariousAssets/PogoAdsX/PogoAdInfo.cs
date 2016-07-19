namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using Newtonsoft.Json;

	[JsonObject (MemberSerialization.OptIn)]
	public class PogoAdInfo
	{
		public readonly static string ios_key = "ios_app_id";
		public readonly static string android_key = "android_app_id";

		[JsonProperty]
		public string Key;
		[JsonProperty]
		public string Title;
		[JsonProperty]
		public bool Enable;
		[JsonProperty]
		public Dictionary<string, string> Params;
		[JsonProperty]
		public bool SetInEditor;

		public PogoAdInfo ()
		{
			Title = string.Empty;
			Enable = false;
			Params = new Dictionary<string, string> ();
		}

		public string iOSValue {
			get {
				string value;
				Params.TryGetValue (ios_key, out value);
				return value;
			}
		}

		public string androidValue {
			get {
				string value;
				Params.TryGetValue (android_key, out value);
				return value;
			}
		}
	}
}
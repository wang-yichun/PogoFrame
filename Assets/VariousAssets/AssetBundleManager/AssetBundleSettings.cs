namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using System;
	using AssetBundles;
	using Newtonsoft.Json;
	using System.IO;

	#if UNITY_EDITOR
	using UnityEditor;

	[InitializeOnLoad]
	#endif

	[JsonObject (MemberSerialization.OptIn)]
	public class AssetBundleSettings : ScriptableObject
	{
		public static string logPrefix {
			get {
				if (EditorGUIUtility.isProSkin) {
					return "<b><color=#00ffaa>[AssetBundleManager] </color></b>"; 
				} else {
					return "<b><color=#007744>[AssetBundleManager] </color></b>"; 
				}
			}
		}

		public static readonly string assetName = "AssetBundleSettings";
		public static readonly string fullPath = "Assets/Resources/AssetBundleSettings.asset";

		private static AssetBundleSettings instance = null;

		public static AssetBundleSettings Instance {
			get {
				if (instance == null) {
					instance = Resources.Load<AssetBundleSettings> (assetName);
					if (instance == null) {
						instance = CreateInstance<AssetBundleSettings> ();

						#if UNITY_EDITOR
						if (!Directory.Exists ("Assets/Resources")) {
							Directory.CreateDirectory ("Assets/Resources");
						}
						AssetDatabase.CreateAsset (instance, fullPath);
						#endif
					}
				}
				return instance;
			}
		}
		#if UNITY_EDITOR
		[MenuItem ("PogoTools/AssetBundle Settings #%a")]
		public static void Execute ()
		{
			Selection.activeObject = AssetBundleSettings.Instance;
		}
		#endif

		// 定义数据载体
		[JsonProperty]
		public List<AssetBundleUrl_Loading> loadingUrls;
		[JsonProperty]
		public List<AssetBundleUrl_Export> exportUrls;

		#region static function

		public static void SetBaseDownloadingURL (AssetBundleUrl_Loading url)
		{
			string full_url = GetFullUrl (url);
			AssetBundleManager.SetBaseDownloadingURL (url.UrlId, full_url);
		}

		public static string GetFullUrl (AssetBundleUrl_Loading url)
		{
			string full_url;
			if (url.IsLocal) {
				full_url = string.Format (
					"{0}/{1}/{2}/",
					AssetBundleManager.GetStreamingAssetsPath (),
					Utility.GetPlatformName (),
					url.UrlId
				);
			} else {
				full_url = string.Format (
					"{0}/{1}/{2}/",
					url.Url,
					Utility.GetPlatformName (),
					url.UrlId
				);
			}
			return full_url;
		}

		#endregion
	}

	[Serializable]
	public class AssetBundleUrl
	{
		public bool Enable;
		public string UrlId;
		public string Title;
	}

	[Serializable]
	public class AssetBundleUrl_Loading : AssetBundleUrl
	{
		public bool IsLocal;
		public bool Simulation;
		public string Url;
	}

	[Serializable]
	public class AssetBundleUrl_Export : AssetBundleUrl
	{
		public bool Clear;
		public List<string> Urls;

		[Space (10)]
		public bool targetStandalone;
		public bool targetIOS;
		public bool targetAndroid;

		[Space (10)]
		public ExportFilterMode FilterMode;
		public List<string> PathFilters;
	}

	public enum ExportFilterMode
	{
		IGNORE,
		OPT_IN,
		OPT_OUT,
		NONE
	}
}
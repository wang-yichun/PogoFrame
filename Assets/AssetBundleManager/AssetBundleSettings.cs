namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using Newtonsoft.Json;
	using System;

	#if UNITY_EDITOR
	using UnityEditor;

	[InitializeOnLoad]
	#endif

	public class AssetBundleSettings : ScriptableObject
	{
		public static readonly string assetName = "AssetBundleSettings";
		public static readonly string fullPath = "Assets/AssetBundleManager/Resources/AssetBundleSettings.asset";

		private static AssetBundleSettings instance = null;

		public static AssetBundleSettings Instance {
			get {
				if (instance == null) {
					instance = Resources.Load<AssetBundleSettings> (assetName);
					if (instance == null) {
						instance = CreateInstance<AssetBundleSettings> ();

						#if UNITY_EDITOR
						AssetDatabase.CreateAsset (instance, fullPath);
						#endif
					}
				}
				return instance;
			}
		}
		#if UNITY_EDITOR
		[MenuItem ("PogoTools/AssetBundle Settings(NEW) #%a")]
		public static void Execute ()
		{
			Selection.activeObject = AssetBundleSettings.Instance;
		}
		#endif

		// 定义数据载体
		public List<AssetBundleUrl_Loading> loadingUrls;
		public List<AssetBundleUrl_Export> exportUrls;
	}

	[Serializable]
	public class AssetBundleUrl
	{
		public bool Enable;
		public string UrlId;
		public string Url;
		public string Title;
	}

	[Serializable]
	public class AssetBundleUrl_Loading : AssetBundleUrl
	{
		public bool IsLocal;
	}


	[Serializable]
	public class AssetBundleUrl_Export : AssetBundleUrl
	{
		public bool Clear;
	}
}
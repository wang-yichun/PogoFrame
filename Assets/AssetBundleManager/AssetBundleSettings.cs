namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using Newtonsoft.Json;
	using System;
	using AssetBundles;

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

	public class AssetBundleSettingLoader
	{

		public IEnumerator InitializeUseSettings ()
		{
			for (int i = 0; i < AssetBundleSettings.Instance.loadingUrls.Count; i++) {
				AssetBundleUrl_Loading url = AssetBundleSettings.Instance.loadingUrls [i];
				if (url.Enable) {


					if (url.IsLocal) {
						string full_url = string.Format (
							                  "{0}/{1}/{2}",
							                  AssetBundleManager.GetStreamingAssetsPath (),
							                  Utility.GetPlatformName (),
							                  url.UrlId
						                  );
						AssetBundleManager.SetBaseDownloadingURL (url.UrlId, full_url);
					} else {
						string full_url = string.Format (
							                  "{0}/{1}/{2}",
							                  url.Url,
							                  Utility.GetPlatformName (),
							                  url.UrlId
						                  );
						AssetBundleManager.SetBaseDownloadingURL (url.UrlId, url.Url);
					}

					var request = AssetBundleManager.Initialize (url.UrlId, url.UrlId);
					if (request != null) {
						yield return StartCoroutine (request);

						Debug.Log ("[Loading Completed] UrlId: " + url.UrlId + "\n" + GetBaseDownloadingURL [url.UrlId]);
					}
				}
			}
		}
	}
}
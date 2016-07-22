namespace pogorock
{
	using UnityEngine;
	using UnityEditor;
	using System.IO;
	using AssetBundles;
	using Newtonsoft.Json;
	using System;
	using System.IO;
	using System.Text;
	using UniRx;

	public partial class AssetBundleSettingsEditor : Editor
	{
		public string default_absc_filename = "asset_bundle_settings";

		StringBuilder httpServerInfoText = new StringBuilder ();

		bool importOverride = false;

		public void SettingHandle ()
		{
			AssetBundleSettings settings = target as AssetBundleSettings;

			EditorGUILayout.Space ();

//			EditorGUILayout.HelpBox (readme.text, MessageType.None, true);
			if (GUILayout.Button ("ReadMe.txt", GetBlueTextStyle ())) {
				UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal (GetSysRootPath () + "ReadMe.bytes", 1);
			}

			EditorGUILayout.Space ();
			EditorGUILayout.BeginVertical ("box");
			GUILayout.Label ("导入导出数据: ");

			if (GUILayout.Button ("Export configs ...")) {
				string path = EditorUtility.SaveFilePanel ("Save", Application.dataPath, default_absc_filename, "config");
				if (string.IsNullOrEmpty (path) == false) {
					using (FileStream fs = new FileStream (@"" + path, FileMode.CreateNew)) {
						StreamWriter sw = new StreamWriter (fs);
						string json_str = JsonConvert.SerializeObject (settings, Formatting.Indented);
						sw.Write (json_str);
						sw.Close ();
					}
				}
			}


			EditorGUILayout.BeginHorizontal ();
			importOverride = GUILayout.Toggle (importOverride, "覆盖", GUILayout.Width (50f));

			if (GUILayout.Button ("Import configs ...")) {

				string path = EditorUtility.OpenFilePanelWithFilters ("Load", Application.dataPath, new string[] {
					"AssetBundleSettings Config",
					"config"
				});
				if (string.IsNullOrEmpty (path) == false) {
					using (FileStream fs = new FileStream (@"" + path, FileMode.Open)) {
						StreamReader sr = new StreamReader (fs);
						string json_str = sr.ReadToEnd ();
						sr.Close ();
						AssetBundleSettings tempSettings = JsonConvert.DeserializeObject<AssetBundleSettings> (json_str);
						if (importOverride) {
							settings.loadingUrls = tempSettings.loadingUrls;
							settings.exportUrls = tempSettings.exportUrls;
						} else {
							settings.loadingUrls.AddRange (tempSettings.loadingUrls);
							settings.exportUrls.AddRange (tempSettings.exportUrls);
						}

						AssetDatabase.Refresh ();
					}
				}
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();

			EditorGUILayout.Space ();

			EditorGUILayout.BeginVertical ("box");
			EditorGUILayout.BeginHorizontal ();

			if (LaunchAssetBundleServer.IsRunning ()) {
				GUILayout.Label (AssetBundleSettingsEditor.gizmo_enable);
				GUILayout.Label ("服务器已开启");
				if (GUILayout.Button ("关闭 HTTP 服务")) {
					LaunchAssetBundleServer.KillRunningAssetBundleServer ();
				}
			} else {
				GUILayout.Label (AssetBundleSettingsEditor.gizmo_disable);
				GUILayout.Label ("服务器已关闭");
				if (GUILayout.Button ("开启 HTTP 服务")) {
					LaunchAssetBundleServer.Run ();
				}
			}

			EditorGUILayout.EndHorizontal ();

			GUILayout.Label ("Server PID: " + LaunchAssetBundleServer.Instance.m_ServerPID);
			GUILayout.Label ("Server Url: " + httpServerUrl);

			EditorGUILayout.EndVertical ();
		}

		public static string httpServerUrl;

		public static string GetHTTPServerUrl ()
		{
			string assetBundleManagerResourcesDirectory = "Assets/Resources";
			string assetBundleUrlPath = Path.Combine (assetBundleManagerResourcesDirectory, "AssetBundleServerURL.bytes");
			Directory.CreateDirectory (assetBundleManagerResourcesDirectory);
			string url = File.ReadAllText (assetBundleUrlPath);
			return url;
		}
	}

}
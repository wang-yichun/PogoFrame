namespace pogorock
{
	using UnityEngine;
	using UnityEditor;
	using System.IO;
	using AssetBundles;
	using Newtonsoft.Json;
	using System;
	using System.IO;

	public partial class AssetBundleSettingsEditor : Editor
	{
		public string default_absc_filename = "asset_bundle_settings";

		public static TextAsset readme;

		public void SettingHandle ()
		{
			AssetBundleSettings settings = target as AssetBundleSettings;

			EditorGUILayout.Space ();

			EditorGUILayout.HelpBox (readme.text, MessageType.None, true);

			EditorGUILayout.Space ();
			EditorGUILayout.BeginVertical ("box");
			if (GUILayout.Button ("Save to .config files ...")) {
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

			if (GUILayout.Button ("Load from configs files ...")) {

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
						settings.loadingUrls = tempSettings.loadingUrls;
						settings.exportUrls = tempSettings.exportUrls;
					}
				}
			}
			EditorGUILayout.EndVertical ();
		}

	}

}
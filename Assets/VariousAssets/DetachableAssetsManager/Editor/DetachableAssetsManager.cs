namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using Newtonsoft.Json;
	using System.IO;

	public class DetachableAssetsManagerWindow : EditorWindow
	{
		[MenuItem ("PogoTools/Detachable Assets Manager #%d")]
		static void AddWindow ()
		{
			//创建窗口
			Rect rect = new Rect (0, 0, 500, 500);
			DetachableAssetsManagerWindow window = (DetachableAssetsManagerWindow)EditorWindow.GetWindowWithRect (
				                                       typeof(DetachableAssetsManagerWindow),
				                                       rect,
				                                       true,
				                                       "Detachable Assets Manager(可拆卸资源管理器)"
			                                       );	
			window.Show ();
		}

		void Awake ()
		{
			tryCreateASampleConfig ();
			loadConfig ();
		}

		void OnGUI ()
		{
			Debug.Log ("OnGUI()");
		}

		List<DetachableAssetInfo> ConfigList;

		private static readonly string ConfigFilePath = "Assets/VariousAssets/DetachableAssetsManager/Editor";
		private static readonly string ConfigFileName = "DetachableAssetsManagerConfig.txt";

		private static string ConfigFileFullPath {
			get {
				return Path.Combine (ConfigFilePath, ConfigFileName);
			}
		}

		void tryCreateASampleConfig ()
		{
			if (File.Exists (ConfigFileFullPath) == false) {
				ConfigList = new List<DetachableAssetInfo> ();
				ConfigList.Add (new DetachableAssetInfo ());
				using (StreamWriter sw = File.CreateText (ConfigFileFullPath)) {
					string text = JsonConvert.SerializeObject (ConfigList, Formatting.Indented);
					sw.Write (text);
				}
			}
		}

		void loadConfig ()
		{
			string text = File.ReadAllText (ConfigFileFullPath);
			ConfigList = JsonConvert.DeserializeObject<List<DetachableAssetInfo>> (text);
		}
	}
}
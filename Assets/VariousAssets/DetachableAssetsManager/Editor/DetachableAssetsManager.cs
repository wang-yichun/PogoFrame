namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using Newtonsoft.Json;
	using System.IO;

	public partial class DetachableAssetsManagerWindow : EditorWindow
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

		Texture2D gizmo_opened;
		Texture2D gizmo_closed;

		void Awake ()
		{
			gizmo_opened = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_opened.psd");
			gizmo_closed = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_closed.psd");
			
			tryCreateASampleConfig ();
			loadConfig ();
		}

		public static string GetSysRootPath ()
		{
			return "Assets/VariousAssets/DetachableAssetsManager/";
		}

		void OnGUI ()
		{
			for (int i = 0; i < ConfigList.Count; i++) {
				var info = ConfigList [i];
				InfoItemLayout (info);
			}
		}

		#region config file

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

		#endregion

		void DoDetach (DetachableAssetInfo info)
		{
			Debug.Log ("Detach " + info.Name + ".");
		}

		void DoIntegrate (DetachableAssetInfo info)
		{
			Debug.Log ("Integrate " + info.Name + ".");
			DAM_FilesCopy.copyDirectory (info.DevDataPathRoot, info.AssetsPathRoot);
		}
	}
}
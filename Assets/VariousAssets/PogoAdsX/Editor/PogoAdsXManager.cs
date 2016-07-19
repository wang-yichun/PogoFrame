namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using Newtonsoft.Json;
	using System.IO;
	using UniRx;
	using System.Linq;

	public partial class PogoAdsXManager : EditorWindow
	{
		[MenuItem ("PogoTools/PogoAdsX Manager #%x")]
		static void AddWindow ()
		{
			//创建窗口
			Rect rect = new Rect (0, 0, 700, 600);
			PogoAdsXManager window = (PogoAdsXManager)EditorWindow.GetWindowWithRect (
				                         typeof(PogoAdsXManager),
				                         rect,
				                         true,
				                         "PogoAdsX Manager(蹦石广告管理器)"
			                         );	
			window.Show ();
		}

		Texture2D gizmo_title_banner;
		Texture2D gizmo_enabled;
		Texture2D gizmo_disabled;
		Texture2D gizmo_unready;
		Texture2D gizmo_enter;
		Texture2D gizmo_cmd_c;
		Texture2D gizmo_detail;
		Texture2D gizmo_add;
		Texture2D gizmo_del;
		Texture2D gizmo_ok_nobackup;

		void Awake ()
		{
			gizmo_enabled = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_enabled.psd");
			gizmo_disabled = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_disabled.psd");
			gizmo_unready = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_unready.psd");
			gizmo_title_banner = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/title_banner.psd");
			gizmo_enter = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_enter.psd");
			gizmo_cmd_c = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_cmd_c.psd");
			gizmo_detail = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_detail.psd");
			gizmo_ok_nobackup = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_ok_nobackup.psd");
			gizmo_add = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_add.psd");
			gizmo_del = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_del.psd");
		}

		public static string GetSysRootPath ()
		{
			return "Assets/VariousAssets/PogoAdsX/";
		}

		Vector2 scrollPos;

		void OnGUI ()
		{
			if (ConfigList == null) {
				tryCreateASampleConfig ();
				loadConfig ();
			}

			GUI.DrawTexture (new Rect (0f, 0f, 700f, 74f), gizmo_title_banner);
			GUILayout.Space (50f);

			EditorGUILayout.BeginHorizontal (GUILayout.Width (100f));
			if (GUILayout.Button ("Config文件")) {
				UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal (ConfigFileFullPath, 1);
			}
			EditorGUILayout.EndHorizontal ();
//
			scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
			for (int i = 0; i < ConfigList.Count; i++) {
				var info = ConfigList [i];
				InfoItemLayout (info);
			}
			EditorGUILayout.EndScrollView ();
//
//

			if (GUI.changed) {
				saveConfig ();
			}
		}

		#region config file

		List<PogoAdInfo> ConfigList;

		private static readonly string ConfigFilePath = "Assets/VariousAssets/PogoAdsX/Resourcs";
		private static readonly string ConfigFileName = "PogoAdsXManagerConfig.txt";

		private static string ConfigFileFullPath {
			get {
				return Path.Combine (ConfigFilePath, ConfigFileName);
			}
		}

		void tryCreateASampleConfig ()
		{
			if (Directory.Exists (ConfigFilePath) == false) {
				Directory.CreateDirectory (ConfigFilePath);
			}
			if (File.Exists (ConfigFileFullPath) == false) {
				ConfigList = new List<PogoAdInfo> ();
				ConfigList.Add (new PogoAdInfo ());
				using (StreamWriter sw = File.CreateText (ConfigFileFullPath)) {
					string text = JsonConvert.SerializeObject (ConfigList, Formatting.Indented);
					sw.Write (text);
				}
			}
		}

		void loadConfig ()
		{
			string text = File.ReadAllText (ConfigFileFullPath);
			ConfigList = JsonConvert.DeserializeObject<List<PogoAdInfo>> (text);
		}

		void saveConfig ()
		{
			string text = JsonConvert.SerializeObject (ConfigList, Formatting.Indented);
			File.WriteAllText (ConfigFileFullPath, text);
		}

		#endregion


	}
}
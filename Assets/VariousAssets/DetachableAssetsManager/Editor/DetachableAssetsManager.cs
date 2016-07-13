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

		Texture2D gizmo_title_banner;
		Texture2D gizmo_integrated;
		Texture2D gizmo_detached;
		Texture2D gizmo_unready;

		void Awake ()
		{
			gizmo_integrated = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_integrated.psd");
			gizmo_detached = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_detached.psd");
			gizmo_unready = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_unready.psd");
			gizmo_title_banner = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/title_banner.psd");
		}

		public static string GetSysRootPath ()
		{
			return "Assets/VariousAssets/DetachableAssetsManager/";
		}

		Vector2 scrollPos;

		void OnGUI ()
		{
			if (ConfigList == null) {
				tryCreateASampleConfig ();
				loadConfig ();
			}

			GUI.DrawTexture (new Rect (0f, 0f, 500f, 74f), gizmo_title_banner);
			GUILayout.Space (60f);
			scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
			for (int i = 0; i < ConfigList.Count; i++) {
				var info = ConfigList [i];
				InfoItemLayout (info);
			}
			EditorGUILayout.EndScrollView ();
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
			Debug.Log ("开始拆卸: " + info.Name + ".");

			// 删除资源文件
			if (Directory.Exists (info.AssetsPathRoot)) {
				Directory.Delete (info.AssetsPathRoot, true);
			} else {
				Debug.LogFormat ("在位置: {0} 已经不存在资源,请检查是否已经删掉了这个位置的资源?", info.AssetsPathRoot);
			}
			if (Directory.Exists (info.AssetsPathRoot + ".meta")) {
				File.Delete (info.AssetsPathRoot + ".meta");
			}

			// 删除预定义 symbol
			SymbolHelper.DeleteSymbol (info.Symbol);
			Debug.LogFormat ("从 Scripting Define Symbols 删除了: {0}", info.Symbol);

			AssetDatabase.Refresh ();

			Debug.Log ("拆卸结束: " + info.Name + ".");
		}

		void DoIntegrate (DetachableAssetInfo info)
		{
			Debug.Log ("开始集成: " + info.Name + ".");

			// 拷贝资源文件
			DAM_FilesCopy.copyDirectory (info.DevDataPathRoot, info.AssetsPathRoot);
			Debug.Log ("原存放位置 -> " + info.DevDataPathRoot + "\n项目中位置 -> " + info.AssetsPathRoot);

			// 添加预定义 symbol
			SymbolHelper.AddNewSymbol (info.Symbol);
			Debug.LogFormat ("向 Scripting Define Symbols 加入了: {0}", info.Symbol);

			AssetDatabase.Refresh ();

			Debug.Log ("集成结束: " + info.Name + ".");
		}
	}
}
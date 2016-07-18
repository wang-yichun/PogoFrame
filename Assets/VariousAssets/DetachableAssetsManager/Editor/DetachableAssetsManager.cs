namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using Newtonsoft.Json;
	using System.IO;
	using UniRx;

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
		Texture2D gizmo_enter;
		Texture2D gizmo_ok_nobackup;

		void Awake ()
		{
			gizmo_integrated = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_integrated.psd");
			gizmo_detached = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_detached.psd");
			gizmo_unready = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_unready.psd");
			gizmo_title_banner = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/title_banner.psd");
			gizmo_enter = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_enter.psd");
			gizmo_ok_nobackup = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/gizmo_ok_nobackup.psd");
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
			GUILayout.Space (50f);

			EditorGUILayout.BeginHorizontal (GUILayout.Width (100f));
			if (GUILayout.Button ("Config文件")) {
				UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal (ConfigFileFullPath, 1);
			}
			EditorGUILayout.EndHorizontal ();

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

			if (Directory.Exists (info.DevDataPathRoot) == false) {
				if (EditorUtility.DisplayDialog ("拆卸没有副本的项", "该资源没有备份,拆卸后将无法还原!!\n 请确认拆卸副本", "确认拆卸", "取消操作")) {
				} else {
					Debug.Log ("取消拆卸: " + info.Name + ".");
					return;
				}
			}

			// 删除资源文件
			if (Directory.Exists (info.AssetsPathRoot + ".meta")) {
				File.Delete (info.AssetsPathRoot + ".meta");
			}
			if (Directory.Exists (info.AssetsPathRoot)) {
				Directory.Delete (info.AssetsPathRoot, true);
			} else {
				Debug.LogFormat ("在位置: {0} 已经不存在资源,请检查是否已经删掉了这个位置的资源?", info.AssetsPathRoot);
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


		void DoCopy (DetachableAssetInfo info)
		{
			Debug.Log ("开始备份拆卸副本: " + info.Name + ".");

			// 拷贝资源文件
			DAM_FilesCopy.copyDirectory (info.AssetsPathRoot, info.DevDataPathRoot);
			Debug.Log ("项目中位置 -> " + info.AssetsPathRoot + "\n原备份位置 -> " + info.DevDataPathRoot);

			Debug.Log ("备份结束: " + info.Name + ".");
		}

		void DoDelete (DetachableAssetInfo info)
		{
			Debug.Log ("开始删除拆卸副本: " + info.Name + ".");
			if (Directory.Exists (info.DevDataPathRoot)) {
				if (EditorUtility.DisplayDialog ("删除拆卸副本", "请确认删除拆卸副本", "确认删除", "取消操作")) {
					Directory.Delete (info.DevDataPathRoot, true);

					EditorUtility.DisplayDialog ("已删除拆卸副本", "请注意备份拆卸副本,否则无法使用集成功能", "确认"); 
				}
			} else {
				Debug.Log ("未发现拆卸副本.");
			}
			Debug.Log ("删除结束: " + info.Name + ".");
		}

		void DoClean (DetachableAssetInfo info)
		{
			Debug.Log ("在拆卸副本中开始清理 \".DS_Store\", \".meta\" 等文件: " + info.Name + ".");
			DAM_FilesCopy.cleanDirectory (info.DevDataPathRoot);
			Debug.Log ("清理结束: " + info.Name + ".");
		}

		void DoExportPackage (DetachableAssetInfo info)
		{
			Debug.Log ("开始将资源重新打包: " + info.Name + ".");

//			string detachable_assets_path = new DirectoryInfo (info.DevDataPathRoot).Parent.Name;
//			string pachage_full_path = Path.Combine (detachable_assets_path, string.Format ("{0} ({1}).unitypackage", info.Name, info.Version));

			Observable.NextFrame ().Subscribe (_ => {
				AssetDatabase.ExportPackage (info.AssetsPathRoot, string.Format ("{0} ({1}).unitypackage", info.Name, info.Version), ExportPackageOptions.Recurse);
				Debug.Log ("打包结束: " + info.Name + ".");
			});
		}
	}
}
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

	public partial class AssetLabelManager : EditorWindow
	{
		[MenuItem ("PogoTools/Asset Labels Manager #%t")]
		public static void AddWindow ()
		{
			//创建窗口
			Rect rect = new Rect (0, 0, 700f, 800);
			AssetLabelManager window = (AssetLabelManager)EditorWindow.GetWindowWithRect (
				                           typeof(AssetLabelManager),
				                           rect,
				                           true,
				                           "Asset Labels Manager(资源标签管理器)"
			                           );	
			window.Show ();

			window.Init ();
			instance = window;
		}

		void Init ()
		{
			assetBundleNames = AssetDatabase.GetAllAssetBundleNames ();
			this.assetsDic = null;
			pathFilters = null;
			filterMode = ExportFilterMode.NONE;
		}


		private static AssetLabelManager instance;

		public static AssetLabelManager Instance {
			get {
				return instance;
			}
		}

		Texture2D gizmo_title_banner;

		void Awake ()
		{
			gizmo_title_banner = AssetDatabase.LoadAssetAtPath<Texture2D> (GetSysRootPath () + "Gizmos/title_banner.psd");
		}

		public static string GetSysRootPath ()
		{
			return "Assets/VariousAssets/AssetBundleManager/";
		}

		Vector2 scrollPosition;

		public string[] assetBundleNames;
		public Dictionary<string,string[]> assetsDic;

		public static List<string> pathFilters;
		public static ExportFilterMode filterMode;
		public static string urlId;

		bool selected_in_export_panel (string assetPath)
		{
			string name = assetPath;

			if (filterMode == ExportFilterMode.OPT_IN) {
				foreach (var filter in pathFilters) {
					if (name.StartsWith (filter)) {
						return true;
					}
				}
			} else if (filterMode == ExportFilterMode.OPT_OUT) {
				bool selected = true;
				foreach (var filter in pathFilters) {
					if (name.StartsWith (filter)) {
						selected = false;
					}
				}
				if (selected) {
					return true;
				}
			} else if (filterMode == ExportFilterMode.IGNORE) {
				return true;
			}
			return false;
		}

		void ExpendAll ()
		{
			assetsDic = new Dictionary<string, string[]> ();

			for (int i = 0; i < assetBundleNames.Length; i++) {
				string assetBundleName = assetBundleNames [i];
				string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle (assetBundleName);
				assetsDic.Add (assetBundleName, assetPaths);
			}
		}

		void OnGUI ()
		{
			GUI.DrawTexture (new Rect (0f, 0f, 700f, 74f), gizmo_title_banner);
			GUILayout.Space (60f);
			scrollPosition = EditorGUILayout.BeginScrollView (scrollPosition);
			EditorGUILayout.BeginVertical ("box");
			if (GUILayout.Button ("刷新")) {
				Init ();
			}
			if (GUILayout.Button ("查看所有")) {
				ExpendAll ();
			}

			if (filterMode != ExportFilterMode.NONE && string.IsNullOrEmpty (urlId) == false) {
				GUILayout.Label (string.Format ("蓝色部分是 UrlId 为 {0} 的资源.", urlId), GetAssetBundleNameStyle (Color.blue)); 
			}

			GUIStyle s = GetAssetBundleNameStyle (Color.black);

			GUIStyle s2 = GetAssetNameStyle (Color.black);
			GUIStyle s2_blue = GetAssetBundleNameStyle (Color.blue);

			if (assetBundleNames != null) {
				for (int i = 0; i < assetBundleNames.Length; i++) {
					string assetBundleName = assetBundleNames [i];

					if (assetsDic == null) {
						assetsDic = new Dictionary<string, string[]> ();
					}

					string abn_preStr = "+ ";
					if (assetsDic.ContainsKey (assetBundleName)) {
						abn_preStr = "- ";
					}
					if (GUILayout.Button (abn_preStr + assetBundleName, s)) {
						if (assetsDic.ContainsKey (assetBundleName)) {
							assetsDic.Remove (assetBundleName);
						} else {
							string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle (assetBundleName);
							assetsDic.Add (assetBundleName, assetPaths);
						}
					}

					if (assetsDic != null && assetsDic.ContainsKey (assetBundleName)) {
						string[] assetPaths = assetsDic [assetBundleName];
						for (int j = 0; j < assetPaths.Length; j++) {
							string assetPath = assetPaths [j];
							string preStr = "    ├  ";
							if (j == assetPaths.Length - 1) {
								preStr = "    └  ";
							}
							GUIStyle style = selected_in_export_panel (assetPath) ? s2_blue : s2;
							if (GUILayout.Button (preStr + assetPath, style)) {
								var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object> (assetPath);
								Selection.activeObject = asset;
							}
						}
					}
				}
			}

			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndScrollView ();
		}

		GUIStyle GetAssetBundleNameStyle (Color color)
		{
			GUIStyle s = new GUIStyle ();
			s.padding = new RectOffset (10, 10, 2, 2);
			s.alignment = TextAnchor.MiddleLeft;
			s.normal = new GUIStyleState () {
				textColor = color
			};
			s.fontStyle = FontStyle.Bold;
			return s;
		}

		GUIStyle GetAssetNameStyle (Color color)
		{
			GUIStyle s = new GUIStyle ();
			s.padding = new RectOffset (10, 10, 2, 2);
			s.alignment = TextAnchor.MiddleLeft;
			s.normal = new GUIStyleState () {
				textColor = color
			};
			return s;
		}
	}
}
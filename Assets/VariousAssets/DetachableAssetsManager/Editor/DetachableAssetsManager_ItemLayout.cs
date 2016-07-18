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

		void InfoItemLayout (DetachableAssetInfo info)
		{
			EditorGUILayout.BeginVertical ("box");
			EditorGUILayout.BeginHorizontal ();


			bool enable = false;
			if (SymbolHelper.ExistSymbol (info.Symbol)) {
				enable = true;
			}
			EditorGUILayout.Space ();
			EditorGUILayout.BeginVertical (GUILayout.Width (100f));
			EditorGUILayout.Space ();
			if (enable) {
				if (Directory.Exists (info.DevDataPathRoot)) {
					GUILayout.Label (gizmo_integrated);
				} else {
					GUILayout.Label (gizmo_ok_nobackup);
				}
			} else {
				if (Directory.Exists (info.DevDataPathRoot)) {
					GUILayout.Label (gizmo_detached);
				} else {
					GUILayout.Label (gizmo_unready);
				}
			}
			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("集成")) {
				DoIntegrate (info);
			}
			if (GUILayout.Button ("拆卸")) {
				DoDetach (info);
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("备份")) {
				DoCopy (info);
			}
			if (GUILayout.Button ("删除")) {
				DoDelete (info);
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("清理")) {
				DoClean (info);
			}
			if (GUILayout.Button ("打包")) {
				DoExportPackage (info);
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();

			EditorGUILayout.Space ();

			fixedInfoContent (info, enable);

			EditorGUILayout.EndHorizontal ();

			multiPaths (info);

			EditorGUILayout.EndVertical ();
		}

		void fixedInfoContent (DetachableAssetInfo info, bool enable)
		{
			EditorGUILayout.BeginVertical (GUILayout.Width (350f));

			string tip_after_title = string.Empty;
			if (enable == false) {
				if (Directory.Exists (info.DevDataPathRoot) == false) {
					tip_after_title = "(未准备)";
				} else {
					tip_after_title = "(已拆卸)";
				}
			}

			string title = string.Format ("{0} ({1}) {2}", info.Name, info.Description, tip_after_title);
			GUILayout.Label (title, EditorStyles.largeLabel);
			GUILayout.Label ("    版本: " + info.Version ?? "(with no version)");

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("    官网: " + info.Url);
			EditorGUILayout.BeginVertical (GUILayout.Width (50f));
			if (GUILayout.Button (gizmo_enter)) {
				Application.OpenURL (string.Format (info.Url));
			}
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();

			GUILayout.Label ("    备份: " + info.DevDataPathRoot);

			if (info.isMultiPaths) {
//				if (info.rootsFolded = EditorGUILayout.ToggleLeft ("项目中位置: ", info.rootsFolded)) {
//					EditorGUI.indentLevel++;
//					EditorGUILayout.BeginVertical ("box");
//					for (int i = 0; i < info.AssetsPathRoots.Length; i++) {
//						EditorGUILayout.BeginHorizontal ();
//						EditorGUILayout.BeginVertical (GUILayout.Width (20f));
//						GUILayout.Toggle (info.AssetsPathRoots [i].integrate, "集成");
//						EditorGUILayout.EndVertical ();
//						EditorGUILayout.BeginVertical (GUILayout.Width (20f));
//						GUILayout.Toggle (info.AssetsPathRoots [i].integrate, "备份");
//						EditorGUILayout.EndVertical ();
//						GUILayout.Label ("      " + info.AssetsPathRoots [i].path);
//						EditorGUILayout.EndHorizontal ();
//					}
//					EditorGUILayout.EndVertical ();
//					EditorGUI.indentLevel--;
//				}
				if (info.AssetsPathRoots == null || info.AssetsPathRoots.Length == 0) {

					GUILayout.Label (string.Format ("    项目: (未定义)", info.AssetsPathRoots [0], info.AssetsPathRoots.Length));
				} else {

					EditorGUILayout.BeginHorizontal ();
					GUILayout.Label (string.Format ("    项目: {0}... (等{1}个位置)", info.AssetsPathRoots [0].path, info.AssetsPathRoots.Length));

					EditorGUILayout.BeginVertical (GUILayout.Width (50f));
					if (GUILayout.Button (gizmo_enter)) {
						info.rootsFolded = !info.rootsFolded;
					}
					EditorGUILayout.EndVertical ();
					EditorGUILayout.EndHorizontal ();
				}
			} else {
				GUILayout.Label ("    项目: " + info.AssetsPathRoot);
			}

			GUILayout.Label ("    定义: " + info.Symbol);
			EditorGUILayout.EndVertical ();
		}

		void multiPaths (DetachableAssetInfo info)
		{
			if (info.isMultiPaths && info.rootsFolded) {
//				if (info.rootsFolded = EditorGUILayout.ToggleLeft ("项目中位置: (详细)", info.rootsFolded)) {
				EditorGUI.indentLevel++;
				EditorGUILayout.BeginVertical ("box");
				for (int i = 0; i < info.AssetsPathRoots.Length; i++) {
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.BeginVertical (GUILayout.Width (20f));
					info.AssetsPathRoots [i].integrate = GUILayout.Toggle (info.AssetsPathRoots [i].integrate, "集成");
					EditorGUILayout.EndVertical ();
					EditorGUILayout.BeginVertical (GUILayout.Width (20f));
					info.AssetsPathRoots [i].backup = GUILayout.Toggle (info.AssetsPathRoots [i].backup, "备份");
					EditorGUILayout.EndVertical ();
					GUILayout.Label ("      " + info.AssetsPathRoots [i].path);
					EditorGUILayout.EndHorizontal ();
				}
				EditorGUILayout.EndVertical ();
				EditorGUI.indentLevel--;
//				}
			} 
		}
	}
}
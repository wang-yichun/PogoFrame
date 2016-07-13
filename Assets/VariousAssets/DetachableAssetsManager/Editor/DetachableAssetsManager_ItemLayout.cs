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
			EditorGUILayout.BeginHorizontal ("box");
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
			}
			if (GUILayout.Button ("打包")) {
				Debug.Log ("待开发.");
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();

			EditorGUILayout.Space ();

			fixedInfoContent (info, enable);

			EditorGUILayout.EndHorizontal ();
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
			GUILayout.Label ("    版本信息: " + info.Version ?? "(with no version)");

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("    官网: " + info.Url);
			EditorGUILayout.BeginVertical (GUILayout.Width (50f));
			if (GUILayout.Button (gizmo_enter)) {
				Application.OpenURL (string.Format (info.Url));
			}
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();

			GUILayout.Label ("    原存放位置: " + info.DevDataPathRoot);
			GUILayout.Label ("    项目中位置: " + info.AssetsPathRoot);
			GUILayout.Label ("    定义Symbol: " + info.Symbol);
			EditorGUILayout.EndVertical ();
		}
	}
}
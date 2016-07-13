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
				GUILayout.Label (gizmo_integrated);
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

			EditorGUILayout.EndVertical ();

			EditorGUILayout.Space ();

			fixedInfoContent (info, enable);

			EditorGUILayout.EndHorizontal ();
		}

		void fixedInfoContent (DetachableAssetInfo info, bool enable)
		{
			EditorGUILayout.BeginVertical (GUILayout.Width (360f));
			string title = string.Format ("{0} ({1}) {2}", info.Name, info.Description, enable ? string.Empty : " (已拆卸)");
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
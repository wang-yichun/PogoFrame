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

			EditorGUILayout.BeginVertical ();
			EditorGUILayout.Space ();
			if (enable) {
				GUILayout.Label (gizmo_opened, GUILayout.Width (80f), GUILayout.Height (30f));
			} else {
				GUILayout.Label (gizmo_closed, GUILayout.Width (80f), GUILayout.Height (30f));

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

			fixedInfoContent (info);

			EditorGUILayout.EndHorizontal ();
		}

		void fixedInfoContent (DetachableAssetInfo info)
		{
			EditorGUILayout.BeginVertical ();
			GUILayout.Label (info.Name, EditorStyles.boldLabel);
			GUILayout.Label ("    原 Assets 存放位置: " + info.DevDataPathRoot);
			GUILayout.Label ("    项目中位置: " + info.AssetsPathRoot);
			GUILayout.Label ("    预定义 Symbol: " + info.Symbol);
			EditorGUILayout.Space ();
			EditorGUILayout.EndVertical ();
		}
	}
}
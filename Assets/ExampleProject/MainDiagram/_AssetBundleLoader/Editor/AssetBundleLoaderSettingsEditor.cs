namespace pogorock
{
	using UnityEngine;
	using UnityEditor;
	using UnityEditorInternal;

	[CustomEditor (typeof(AssetBundleLoaderSettings))]
	public class AssetBundleSettingsEditor : Editor
	{
		private ReorderableList reorderableTargetPaths;

		void OnEnable ()
		{
			AssetBundleLoaderSettings settings = target as AssetBundleLoaderSettings;

			reorderableTargetPaths = new ReorderableList (serializedObject, serializedObject.FindProperty ("targetPaths"));
			reorderableTargetPaths.drawHeaderCallback = (Rect rect) => {
				EditorGUI.LabelField (rect, "从这里获取AssetBundles");
			};
			reorderableTargetPaths.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {

				rect.y += 2;
				AssetBundleLoaderTargetPath targetPath = settings.targetPaths [index];

				rect.width = 10;
				targetPath.enable = EditorGUI.Toggle (rect, targetPath.enable);

				rect.x += 20;
				rect.width = 300;
				rect.height = EditorGUIUtility.singleLineHeight;
				targetPath.targetPath = EditorGUI.DelayedTextField (rect, targetPath.targetPath);

				if ((Event.current.type == EventType.DragExited) && rect.Contains (Event.current.mousePosition)) {  
					//改变鼠标的外表  
					DragAndDrop.visualMode = DragAndDropVisualMode.Generic;  
					if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0) {  
						targetPath.targetPath = DragAndDrop.paths [0];  
					}  
				}  
			};
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			EditorGUILayout.BeginVertical ();
			reorderableTargetPaths.DoLayoutList ();

			EditorGUILayout.Space ();

			var _useStreamingAssets = serializedObject.FindProperty ("useStreamingAssets");
			_useStreamingAssets.boolValue = EditorGUILayout.ToggleLeft ("use StreamingAssets", _useStreamingAssets.boolValue);

			EditorGUILayout.EndVertical ();

			if (GUI.changed) {
				serializedObject.ApplyModifiedProperties ();
				EditorUtility.SetDirty (target);
			}
		}
	}
}
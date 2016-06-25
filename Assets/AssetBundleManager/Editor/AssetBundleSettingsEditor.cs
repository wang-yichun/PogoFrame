namespace pogorock
{
	using UnityEngine;
	using UnityEditor;

	//	using UnityEditorInternal;

	using Malee.Editor;

	[CustomEditor (typeof(AssetBundleSettings))]
	public class AssetBundleSettingsEditor : Editor
	{

		public static Texture2D gizmo_enable;
		public static Texture2D gizmo_disable;
		public static Texture2D gizmo_local;

		[InitializeOnLoad]
		public class Startup
		{
			static Startup ()
			{
				// 1.使用 EditorPrefs.GetXxx() 载入编辑器配置
				// 2.使用 AssetDatabase.LoadAssetAtPath() 加载一些 Texture2D
				// TODO:

				gizmo_enable = AssetDatabase.LoadAssetAtPath<Texture2D> ("Assets/AssetBundleManager/Gizmos/gizmo_enable.psd");
				gizmo_disable = AssetDatabase.LoadAssetAtPath<Texture2D> ("Assets/AssetBundleManager/Gizmos/gizmo_disable.psd");
				gizmo_local = AssetDatabase.LoadAssetAtPath<Texture2D> ("Assets/AssetBundleManager/Gizmos/gizmo_local.psd");
			}
		}

		private static string[] ToolbarHeaders = new string[] { "Loading", "Export" };
		private int toolbar_index;

		public ReorderableList loadingList;

		private void OnEnable ()
		{
			AssetBundleSettings settings = target as AssetBundleSettings;

			loadingList = new ReorderableList (serializedObject.FindProperty ("loadingUrls"));
			loadingList.drawHeaderCallback = (Rect rect) => {
				GUILayout.BeginArea (rect);
				GUILayout.Button ("header");
				GUILayout.EndArea ();
			};
//			loadingList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
//				EditorGUI.PropertyField (rect, loadingList.serializedProperty.GetArrayElementAtIndex (index), true);
//			};
//			loadingList.elementHeightCallback = (int index) => {
//				return EditorGUI.GetPropertyHeight (loadingList.serializedProperty.GetArrayElementAtIndex (index), GUIContent.none, true) + 4f;
//			};
		}

		public override void OnInspectorGUI ()
		{
			toolbar_index = GUILayout.Toolbar (toolbar_index, ToolbarHeaders, new GUILayoutOption[]{ GUILayout.Height (25f) });

			switch (toolbar_index) {
			case 0:
				LoadingHandle ();
				break;
			case 1:
				ExportHandle ();
				break;
			default:
				break;
			}

			serializedObject.ApplyModifiedProperties ();

			if (GUI.changed) {
				EditorUtility.SetDirty (target);
			}
		}

		public void LoadingHandle ()
		{
			loadingList.DoLayoutList ();
		}

		public void ExportHandle ()
		{
		}
	}

	[CustomPropertyDrawer (typeof(AssetBundleUrl_Loading))]
	public class AssetBundleUrlDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			Texture2D image = property.FindPropertyRelative ("Enable").boolValue ? AssetBundleSettingsEditor.gizmo_enable : AssetBundleSettingsEditor.gizmo_disable;
			bool is_local = property.FindPropertyRelative ("IsLocal").boolValue;

			GUIContent gc = new GUIContent (
				                string.Format (
					                "        {0}{1} - [{2}]",
					                is_local ? "  " : string.Empty,
					                property.FindPropertyRelative ("Title").stringValue,
					                property.FindPropertyRelative ("UrlId").stringValue
				                )
			                );
			GUI.Label (position, new GUIContent (image));

			if (is_local) {
				GUI.Label (position, new GUIContent (AssetBundleSettingsEditor.gizmo_local));
			}

			EditorGUI.PropertyField (position, property, gc, true);
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight (property, GUIContent.none, true) + 4;
		}
	}

}
namespace pogorock
{
	using UnityEngine;
	using UnityEditor;
	using System.IO;
	using AssetBundles;
	using System.Linq;
	using System.Text;
	using System;


	[CustomPropertyDrawer (typeof(AssetBundleUrl_Loading))]
	[CustomPropertyDrawer (typeof(AssetBundleUrl_Export))]
	public class AssetBundleUrlDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			Texture2D image = property.FindPropertyRelative ("Enable").boolValue ? AssetBundleSettingsEditor.gizmo_enable : AssetBundleSettingsEditor.gizmo_disable;

			bool is_local = false;
			bool clear = false;

			var s_is_local = property.FindPropertyRelative ("IsLocal");
			if (s_is_local != null) {
				is_local = s_is_local.boolValue;
			}

			var s_clear = property.FindPropertyRelative ("Clear");
			if (s_clear != null) {
				clear = s_clear.boolValue;
			}

			GUIContent gc = new GUIContent (
				                string.Format (
					                "        {0}{1} - [{2}]",
					                is_local || clear ? "  " : string.Empty,
					                property.FindPropertyRelative ("Title").stringValue,
					                property.FindPropertyRelative ("UrlId").stringValue
				                )
			                );
			GUI.Label (position, new GUIContent (image));

			if (is_local || clear) {
				GUI.Label (position, new GUIContent (AssetBundleSettingsEditor.gizmo_local));
			}

			EditorGUI.PropertyField (position, property, gc, true);

			Type t = GetPropertyType (property);
			if (t == typeof(AssetBundleUrl_Loading) && property.isExpanded) {
				UrlItemGUIMenu_Loading (position, property, label);
			}
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			float height = EditorGUI.GetPropertyHeight (property, GUIContent.none, true) + 10f;

			Type t = GetPropertyType (property);
			if (t == typeof(AssetBundleUrl_Loading) && property.isExpanded) {
				height += GetUrlItemGUIMenuHeight_Loading ();
			}

			return height;
		}

		public Type GetPropertyType (SerializedProperty property)
		{
			if (property.propertyPath.StartsWith ("loadingUrls")) {
				return typeof(AssetBundleUrl_Loading);
			} else if (property.propertyPath.StartsWith ("exportUrls")) {
				return typeof(AssetBundleUrl_Export);
			}
			return typeof(object);
		}

		public void UrlItemGUIMenu_Loading (Rect position, SerializedProperty property, GUIContent label)
		{
			Rect menuRect = new Rect (position.x, position.y + position.height - GetUrlItemGUIMenuHeight_Loading () - 8f, position.width, GetUrlItemGUIMenuHeight_Loading ());
			if (GUI.Button (menuRect, "加载")) {
				OnMenuButtonClicked (property);
			}
		}

		public float GetUrlItemGUIMenuHeight_Loading ()
		{
			return 20f;
		}

		public void OnMenuButtonClicked (SerializedProperty property)
		{
			Debug.Log ("加载: " + property.FindPropertyRelative ("UrlId").stringValue);

		}
	}
}
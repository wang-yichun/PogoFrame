namespace pogorock
{
	using UnityEngine;
	using UnityEditor;
	using System.IO;
	using AssetBundles;
	using System.Linq;
	using System.Text;
	using System;
	using Newtonsoft.Json;
	using UniRx;

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
			} else if (t == typeof(AssetBundleUrl_Export) && property.isExpanded) {
				UrlItemGUIMenu_Export (position, property, label);
			}
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			float height = EditorGUI.GetPropertyHeight (property, GUIContent.none, true) + 10f;

			Type t = GetPropertyType (property);
			if (t == typeof(AssetBundleUrl_Loading) && property.isExpanded) {
				height += GetUrlItemGUIMenuHeight_Loading ();
			} else if (t == typeof(AssetBundleUrl_Export) && property.isExpanded) {
				height += GetUrlItemGUIMenuHeight_Export ();
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
				OnMenuButtonClicked_Loading (property);
			}
		}

		public float GetUrlItemGUIMenuHeight_Loading ()
		{
			return 20f;
		}

		public void UrlItemGUIMenu_Export (Rect position, SerializedProperty property, GUIContent label)
		{
			Rect menuRect = new Rect (position.x, position.y + position.height - GetUrlItemGUIMenuHeight_Loading () - 8f, position.width, GetUrlItemGUIMenuHeight_Loading ());
			if (GUI.Button (menuRect, "发布")) {
				OnMenuButtonClicked_Export (property);
			}
		}

		public float GetUrlItemGUIMenuHeight_Export ()
		{
			return 20f;
		}

		public void OnMenuButtonClicked_Loading (SerializedProperty property)
		{
			string idx_str = property.displayName.Replace ("Element ", string.Empty);
			int idx = Convert.ToInt32 (idx_str);
			AssetBundleUrl_Loading url = AssetBundleSettings.Instance.loadingUrls [idx];

			Debug.Log ("加载: " + JsonConvert.SerializeObject (url, Formatting.Indented));
		}


		public void OnMenuButtonClicked_Export (SerializedProperty property)
		{
			string idx_str = property.displayName.Replace ("Element ", string.Empty);
			int idx = Convert.ToInt32 (idx_str);
			AssetBundleUrl_Export url = AssetBundleSettings.Instance.exportUrls [idx];

			Debug.Log ("发布: " + JsonConvert.SerializeObject (url, Formatting.Indented));

			Observable.NextFrame ().Subscribe (_ => {
				AssetBundleSettingsEditor.Instance.PublishAssetBundles (url, idx);
			});

		}
	}
}
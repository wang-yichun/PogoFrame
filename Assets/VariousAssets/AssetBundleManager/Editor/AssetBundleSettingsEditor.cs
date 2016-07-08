namespace pogorock
{
	using UnityEngine;
	using UnityEditor;
	using System.IO;
	using AssetBundles;
	using System.Text.RegularExpressions;
	using System.Linq;
	using Malee.Editor;
	using System.Text;
	using System;

	[CustomEditor (typeof(AssetBundleSettings))]
	public partial class AssetBundleSettingsEditor : Editor
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

				string root_path = GetSysRootPath ();

				toolbar_index = EditorPrefs.GetInt ("asset_bundle_settings_toolbar_index", 0);

				gizmo_enable = AssetDatabase.LoadAssetAtPath<Texture2D> (root_path + "Gizmos/gizmo_enable.psd");
				gizmo_disable = AssetDatabase.LoadAssetAtPath<Texture2D> (root_path + "Gizmos/gizmo_disable.psd");
				gizmo_local = AssetDatabase.LoadAssetAtPath<Texture2D> (root_path + "Gizmos/gizmo_local.psd");

				readme = AssetDatabase.LoadAssetAtPath<TextAsset> (root_path + "Editor/ReadMe.bytes");
			}
		}

		public static string GetSysRootPath ()
		{
			return "Assets/VariousAssets/AssetBundleManager/";
		}

		private static string[] ToolbarHeaders = new string[] { "Loading", "Export", "Settings" };
		private static int toolbar_index;

		public ReorderableList loadingList;
		public ReorderableList exportList;

		private void OnEnable ()
		{
			AssetBundleSettings settings = target as AssetBundleSettings;

			loadingList = new ReorderableList (serializedObject.FindProperty ("loadingUrls"));
			loadingList.drawHeaderCallback = (Rect rect) => {
				GUI.Label (rect, "从以下 url 读取资源");
			};

			exportList = new ReorderableList (serializedObject.FindProperty ("exportUrls")); 
			exportList.drawHeaderCallback = (Rect rect) => {
				GUI.Label (rect, "向以下 url 发布资源");
			};
		}

		private string ftp_url_pattern = @"ftp://([\s\S]*?)(:(\d+))?/([^\|]*)(\|([^\|]+))?(\|([^\|/]+))?";

		public void PublishAssetBundles ()
		{
			AssetBundleSettings settings = target as AssetBundleSettings;

			for (int i = 0; i < settings.exportUrls.Count; i++) {
				var url = settings.exportUrls [i];
				if (url.Enable) {

					BuildTarget buildTarget;
					string outputPath;

					string first_outputpath_standalone = string.Empty;
					string first_outputpath_ios = string.Empty;
					string first_outputpath_android = string.Empty;

					for (int j = 0; j < url.Urls.Count; j++) {

						string url_str = url.Urls [j];

						if (j == 0) {
							if (url.targetStandalone) {
								buildTarget = BuildTarget.StandaloneOSXUniversal;
								if (smartBuildAssetBundle (buildTarget, url_str, url.UrlId, out first_outputpath_standalone, i, url) == false) {
									break;
								}
							}
							if (url.targetIOS) {
								buildTarget = BuildTarget.iOS;
								if (smartBuildAssetBundle (buildTarget, url_str, url.UrlId, out first_outputpath_ios, i, url) == false) {
									break;
								}
							}
							if (url.targetAndroid) {
								buildTarget = BuildTarget.Android;
								if (smartBuildAssetBundle (buildTarget, url_str, url.UrlId, out first_outputpath_android, i, url) == false) {
									break;
								}
							}
						} else {
							// 进行拷贝
							if (url.targetStandalone) {
								buildTarget = BuildTarget.StandaloneOSXUniversal;
								smartCopy (buildTarget, url_str, url.UrlId, first_outputpath_standalone, i, url);
							}
							if (url.targetIOS) {
								buildTarget = BuildTarget.iOS;
								smartCopy (buildTarget, url_str, url.UrlId, first_outputpath_ios, i, url);
							}
							if (url.targetAndroid) {
								buildTarget = BuildTarget.Android;
								smartCopy (buildTarget, url_str, url.UrlId, first_outputpath_android, i, url);
							}
						}
					}
				}
			}
		}

		public bool smartBuildAssetBundle (BuildTarget buildTarget, string url_str, string url_id, out string first_outputpath, int idx, AssetBundleUrl_Export url)
		{
			first_outputpath = string.Empty;
			string outputPath = Path.Combine (url_str, Utility.GetPlatformForAssetBundles (buildTarget));

			Match m = Regex.Match (outputPath, ftp_url_pattern);
			if (m.Success) {
				Debug.Log (string.Format ("[Idx: {0}, Title: {1}]\n第一个目标 url 不能为FTP位置,请使用 AssetBundles 等本地位置!", idx, url.Title));
				return false;
			} else {

				string asset_id_root_folder = Path.Combine (outputPath, url_id);
				if (url.Clear) {
					if (Directory.Exists (asset_id_root_folder)) {
						Directory.Delete (asset_id_root_folder, true);
					}
				}

				BuildAssetBundle (url_id, outputPath, buildTarget);
				first_outputpath = outputPath;
			}
			return true;
		}

		public void smartCopy (BuildTarget buildTarget, string url_str, string url_id, string first_outputpath, int idx, AssetBundleUrl_Export url)
		{
			int u = url_str.IndexOf ("|");
			string[] upl = null;
			if (u != -1) {
				string u_p = url_str.Substring (u);
				upl = u_p.Split ('|');
				url_str = url_str.Substring (0, u);
			}

			string outputPath = Path.Combine (url_str, Utility.GetPlatformForAssetBundles (buildTarget));
			outputPath = Path.Combine (outputPath, url_id);
			string resourcePath = Path.Combine (first_outputpath, url_id);

			Match m = Regex.Match (outputPath, ftp_url_pattern);
			if (m.Success) {
				FilesCopyWithFTP.uploadDictionary (
					client: null,
					localDictionary: resourcePath,
					remotePath: m.Groups [4].Value,
					remoteHost: m.Groups [1].Value,
					remotePort: m.Groups [3].Value,
					userName: upl [1],
					password: upl [2]
				);
			} else {
				if (url.Clear) {
					Directory.Delete (outputPath, true);
				}
				FilesCopy.copyDirectory (resourcePath, outputPath);
			}
		}

		public void BuildAssetBundle (string url_id, string outputPath, BuildTarget buildTarget)
		{
			outputPath = Path.Combine (outputPath, url_id);

			if (!Directory.Exists (outputPath))
				Directory.CreateDirectory (outputPath);

			BuildPipeline.BuildAssetBundles (outputPath, BuildAssetBundleOptions.None, buildTarget);
		}

		public override void OnInspectorGUI ()
		{
			int new_toolbar_index = GUILayout.Toolbar (toolbar_index, ToolbarHeaders, new GUILayoutOption[]{ GUILayout.Height (25f) });
			if (new_toolbar_index != toolbar_index) {
				EditorPrefs.SetInt ("asset_bundle_settings_toolbar_index", new_toolbar_index);
				toolbar_index = new_toolbar_index;
			}

			switch (toolbar_index) {
			case 0:
				LoadingHandle ();
				break;
			case 1:
				ExportHandle ();
				break;
			case 2:
				SettingHandle ();
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
			WarningUniqueUrlId ();
		}

		public void ExportHandle ()
		{
			if (GUILayout.Button ("发布 AssetBundles !!")) {
				PublishAssetBundles ();
			}
			exportList.DoLayoutList ();
		}

		private StringBuilder sb;

		public void WarningUniqueUrlId ()
		{
			AssetBundleSettings settings = target as AssetBundleSettings;

			var res = settings.loadingUrls.Where (_ => _.Enable).GroupBy (_ => _.UrlId).Select (g => new {UrlId = g.Key, count = g.Count ()});
			if (sb == null) {
				sb = new StringBuilder ();
			} else {
				sb.Length = 0;
			}
			res.ToList ().ForEach (_ => {
				if (_.count > 1) {
					sb.AppendLine (string.Format ("Url Id:{0} 重复{1}次", _.UrlId, _.count));
				}
			});
			if (sb.Length > 0) {
				sb.AppendLine ("对于相同 Url Id 所标明的资源组,你只能选择1个!");
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox (sb.ToString (), MessageType.Warning);
			}
		}
	}

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
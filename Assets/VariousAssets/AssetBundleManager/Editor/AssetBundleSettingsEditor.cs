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
	using UniRx;
	using System.Net;
	using System.Net.Sockets;
	using System.Collections;
	using System.Collections.Generic;
	using Newtonsoft.Json;

	[CustomEditor (typeof(AssetBundleSettings))]
	public partial class AssetBundleSettingsEditor : Editor
	{

		public static Texture2D gizmo_enable;
		public static Texture2D gizmo_disable;
		public static Texture2D gizmo_yellow_dot;

		[InitializeOnLoad]
		public class Startup
		{
			static Startup ()
			{
				// 1.使用 EditorPrefs.GetXxx() 载入编辑器配置
				// 2.使用 AssetDatabase.LoadAssetAtPath() 加载一些 Texture2D
				// TODO:

				string root_path = GetSysRootPath ();
				httpServerUrl = GetHTTPServerUrl ();

				toolbar_index = EditorPrefs.GetInt ("asset_bundle_settings_toolbar_index", 0);

				gizmo_enable = AssetDatabase.LoadAssetAtPath<Texture2D> (root_path + "Gizmos/gizmo_enable.psd");
				gizmo_disable = AssetDatabase.LoadAssetAtPath<Texture2D> (root_path + "Gizmos/gizmo_disable.psd");
				gizmo_yellow_dot = AssetDatabase.LoadAssetAtPath<Texture2D> (root_path + "Gizmos/gizmo_local.psd");
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

		public static AssetBundleSettingsEditor Instance;

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
			exportList.onSelectCallback = OnSelectCallback_Export;
			Instance = this;
		}

		void OnSelectCallback_Export (ReorderableList list)
		{
			int[] selected_idxs = list.Selected;
			AssetBundleSettings settings = target as AssetBundleSettings;
			if (selected_idxs.Length == 1) {
				int selected_idx = selected_idxs [0];
				AssetBundleUrl_Export url = settings.exportUrls [selected_idx];

				AssetLabelManager.filterMode = url.FilterMode;
				AssetLabelManager.pathFilters = url.PathFilters;
				AssetLabelManager.urlId = url.UrlId;
			}
		}

		private string ftp_url_pattern = @"ftp://([\s\S]*?)(:(\d+))?/([^\|]*)(\|([^\|]+))?(\|([^\|/]+))?";

		public void PublishAssetBundles ()
		{
			AssetBundleSettings settings = target as AssetBundleSettings;

			for (int i = 0; i < settings.exportUrls.Count; i++) {
				var url = settings.exportUrls [i];
				if (url.Enable) {
					PublishAssetBundles (url, i);
				}
			}
		}

		public void PublishAssetBundles (AssetBundleUrl_Export url, int i)
		{
			BuildTarget buildTarget;
			string outputPath;

			string first_outputpath_standalone = string.Empty;
			string first_outputpath_ios = string.Empty;
			string first_outputpath_android = string.Empty;

			for (int j = 0; j < url.Urls.Count; j++) {

				string url_str = url.Urls [j];

				if (j == 0) {
					System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew ();
					Debug.LogFormat (AssetBundleSettings.logPrefix + "开始发布资源包向: {0}", url.UrlId);
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
					Debug.LogFormat (AssetBundleSettings.logPrefix + "发布资源包完成. 总共花费 {0}ms.", sw.Elapsed.TotalMilliseconds);
					sw.Stop ();


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

				if (url.FilterMode == ExportFilterMode.IGNORE || url.FilterMode == ExportFilterMode.NONE) {
					BuildAssetBundle (url_id, outputPath, buildTarget);
				} else {
					BuildAssetBundleWithFilter (url_id, outputPath, buildTarget, url.FilterMode, url.PathFilters);
				}
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
				System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew ();
				Observable.Start (() => {
					Debug.LogFormat (AssetBundleSettings.logPrefix + "开始上传资源包到FTP: {0}", outputPath);

					FilesCopyWithFTP.uploadDictionary (
						client: null,
						localDictionary: resourcePath,
						remotePath: m.Groups [4].Value,
						remoteHost: m.Groups [1].Value,
						remotePort: m.Groups [3].Value,
						userName: upl [1],
						password: upl [2]
					);

					Debug.LogFormat (AssetBundleSettings.logPrefix + "上传FTP完成. 总共花费 {0}ms.", sw.Elapsed.TotalMilliseconds);
					sw.Stop ();
				}).Subscribe ();

			} else {
				System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew ();

				Observable.Start (() => {
					Debug.LogFormat (AssetBundleSettings.logPrefix + "开始复制资源包到: {0}", outputPath);

					if (url.Clear) {
						Directory.Delete (outputPath, true);
					}
					FilesCopy.copyDirectory (resourcePath, outputPath);

					Debug.LogFormat (AssetBundleSettings.logPrefix + "复制资源包完成. 总共花费 {0}ms.", sw.Elapsed.TotalMilliseconds);
					sw.Stop ();
				}).Subscribe ();
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

		Vector2 scrollPosition;

		public void ExportHandle ()
		{
			if (GUILayout.Button ("发布 AssetBundles !!")) {

				Observable.NextFrame ().Subscribe (_ => {
					PublishAssetBundles ();
				});
			}
			exportList.DoLayoutList ();

			EditorGUILayout.Space ();

			if (GUILayout.Button ("打开 Asset Labels Manager")) {
				AssetLabelManager.AddWindow ();
			}

		}

		public string[] assetBundleNames;
		public Dictionary<string,string[]> assetsDic;

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

		public static string GetLocalIP ()
		{
			IPHostEntry host;
			string localIP = "";
			host = Dns.GetHostEntry (Dns.GetHostName ());
			foreach (IPAddress ip in host.AddressList) {
				if (ip.AddressFamily == AddressFamily.InterNetwork) {
					localIP = ip.ToString ();
					break;
				}
			}
			return localIP;
		}

		GUIStyle GetBlueTextStyle ()
		{
			GUIStyle s = new GUIStyle ();
			s.padding = new RectOffset (5, 5, 2, 2);
			s.alignment = TextAnchor.MiddleCenter;
			s.normal = new GUIStyleState () {
				textColor = Color.blue
			};
			return s;
		}

	}

}
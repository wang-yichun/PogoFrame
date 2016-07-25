using System.IO;


namespace pogorock
{
	using UnityEngine;
	using UnityEditor;
	using System.Collections;
	using System.Collections.Generic;
	using Newtonsoft.Json;
	using UniRx;
	using System;

	[ExecuteInEditMode]
	public class DecryptAssetBundle : EditorWindow
	{

		private static string assetURL = "";
		private static WWW www;
		//		public string[] assetBundleNames;
		public static Dictionary<string, string[]> abDic;
		public static AssetBundleUrl_Loading urlLoading;

		public static DecryptAssetBundle Instance {
			get {
				return EditorWindow.GetWindow<DecryptAssetBundle> ("资源包详细", true);
			}
		}

		public static void Init (AssetBundleUrl_Loading url, bool autoRun = false)
		{
			DecryptAssetBundle window = DecryptAssetBundle.Instance;
				
			urlLoading = url;
			abDic = new Dictionary<string, string[]> ();
			assetURL = Path.Combine (AssetBundleSettings.GetFullUrl (url), url.UrlId);
			window.loadManifest (assetURL);

			Debug.Log (AssetBundleSettings.logPrefix + "加载: " + JsonConvert.SerializeObject (
				new  {url = url, full = assetURL}, 
				Formatting.Indented
			));
		}

		Vector2 scp;

		void OnGUI ()
		{
			EditorGUILayout.SelectableLabel ("Asset bundle URL: " + assetURL);
			if (GUILayout.Button ("Refresh")) {
				DecryptAssetBundle.Instance.Close ();
				DecryptAssetBundle.Instance.Show (true);
			}

			if (abDic != null) {
				scp = EditorGUILayout.BeginScrollView (scp);
				GUILayout.BeginVertical ("box");

				foreach (var kvp in abDic) {
					
					string abn = kvp.Key;
					var s = GetAssetBundleNamesStyle ();
					// TODO:
					if (GUILayout.Button (abn, s)) {
						Debug.Log ("button clicked: " + abn);
						string asset_url = Path.Combine (AssetBundleSettings.GetFullUrl (urlLoading), abn);
						loadAssets (asset_url, abn);
						Repaint ();
					}

					if (abDic != null && abDic.ContainsKey (abn) && abDic [abn] != null) {
						string[] assetPaths = abDic [abn];
						for (int j = 0; j < assetPaths.Length; j++) {
							string assetPath = assetPaths [j];
							string preStr = "    ├  ";
							if (j == assetPaths.Length - 1) {
								preStr = "    └  ";
							}
							GUIStyle style = GetAssetBundleNamesStyle ();
							if (GUILayout.Button (preStr + assetPath, style)) {
								Debug.Log ("Button clicked: " + assetPath);
//								var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object> (assetPath);
//								Selection.activeObject = asset;
							}
						}
					}
				}

				GUILayout.EndVertical ();
				EditorGUILayout.EndScrollView ();
			}
		}

		private void loadManifest (string s)
		{
			
			ObservableWWW.GetWWW (s).CatchIgnore ((WWWErrorException ex) => {
				Debug.Log ("Exception: " + ex.RawErrorMessage);
			}).Subscribe (_ => {
				www = _;
				AssetBundle bundle = www.assetBundle;
				string[] d = bundle.GetAllAssetNames ();
//				Debug.Log (JsonConvert.SerializeObject (d, Formatting.Indented));
				AssetBundleManifest abm = bundle.LoadAsset (d [0]) as AssetBundleManifest;
				string[] assetBundleNames = abm.GetAllAssetBundles ();
				foreach (string abn in assetBundleNames) {
					abDic.Add (abn, null);

					string asset_url = Path.Combine (AssetBundleSettings.GetFullUrl (urlLoading), abn);
					loadAssets (asset_url, abn);
				}
//				Debug.Log (JsonConvert.SerializeObject (assetBundleNames, Formatting.Indented));
				bundle.Unload (true);
			});
		}

		private void loadAssets (string url, string assetBundleName)
		{
			ObservableWWW.GetWWW (url).CatchIgnore ((WWWErrorException ex) => {
				Debug.Log ("Exception: " + ex.RawErrorMessage);
			}).Subscribe (_ => {
				AssetBundle bundle = _.assetBundle;
				string[] d = bundle.GetAllAssetNames ();
//				Debug.Log (JsonConvert.SerializeObject (d, Formatting.Indented));
				abDic [assetBundleName] = d;
				bundle.Unload (true);
			});
		}

		GUIStyle GetAssetBundleNamesStyle ()
		{
			GUIStyle s = new GUIStyle ();
			s.padding = new RectOffset (5, 5, 2, 2);
			s.alignment = TextAnchor.MiddleLeft;
//			s.normal = new GUIStyleState () {
//				textColor = Color.blue
//			};
			return s;
		}

	}
}
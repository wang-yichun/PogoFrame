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
	using Newtonsoft.Json;
	using System.Collections;
	using System.Collections.Generic;

	public partial class AssetBundleSettingsEditor : Editor
	{
		public void CustomExport ()
		{
			string source_path = "Assets/OriginAssets";
			string target_path = "AssetBundles/OSX/asset1_exp";

			string[] objs = AssetDatabase.GetAssetPathsFromAssetBundle ("ingame");
			Debug.Log (JsonConvert.SerializeObject (objs, Formatting.Indented));

			AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

			AssetBundleBuild abb = new AssetBundleBuild ();
			abb.assetBundleName = "ingame";
			List<string> names = new List<string> ();
			for (int i = 0; i < objs.Length; i++) {
				string name = objs [i];
				names.Add (name);
			}
			abb.assetNames = names.ToArray ();
			buildMap [0] = abb;

			if (Directory.Exists (target_path) == false) {
				Directory.CreateDirectory (target_path);
			}

			EditorApplication.delayCall += () => {
				BuildPipeline.BuildAssetBundles (target_path, buildMap, BuildAssetBundleOptions.None, BuildTarget.StandaloneOSXUniversal);
			};
		}

		public void BuildAssetBundleWithFilter (string url_id, string outputPath, BuildTarget buildTarget, ExportFilterMode filterMode, List<string> filters)
		{
			string[] assetBundleNames = AssetDatabase.GetAllAssetBundleNames ();

			List<AssetBundleBuild> buildMap = new List<AssetBundleBuild> ();

			for (int i = 0; i < assetBundleNames.Length; i++) {
				string assetBundleName = assetBundleNames [i];
				AssetBundleBuild assetBundleBuild = new AssetBundleBuild ();
				assetBundleBuild.assetBundleName = assetBundleName;

				string[] objs = AssetDatabase.GetAssetPathsFromAssetBundle (assetBundleName);
				List<string> names = new List<string> ();
				for (int j = 0; j < objs.Length; j++) {
					string name = objs [j];

					if (filterMode == ExportFilterMode.OPT_IN) {
						foreach (var filter in filters) {
							if (name.StartsWith (filter)) {
								names.Add (name);
							}
						}
					} else if (filterMode == ExportFilterMode.OPT_OUT) {
						bool selected = true;
						foreach (var filter in filters) {
							if (name.StartsWith (filter)) {
								selected = false;
							}
						}
						if (selected) {
							names.Add (name);
						}
					}
						
				}
				if (names.Count > 0) {
					assetBundleBuild.assetNames = names.ToArray ();
					buildMap.Add (assetBundleBuild);
				}
			}
			string target_path = Path.Combine (outputPath, url_id);

			if (!Directory.Exists (target_path)) {
				Directory.CreateDirectory (target_path);
			}

			BuildPipeline.BuildAssetBundles (target_path, buildMap.ToArray (), BuildAssetBundleOptions.None, buildTarget);
		}
	}

}
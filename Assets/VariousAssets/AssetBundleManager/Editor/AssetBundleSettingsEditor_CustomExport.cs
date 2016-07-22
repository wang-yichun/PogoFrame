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
	}

}
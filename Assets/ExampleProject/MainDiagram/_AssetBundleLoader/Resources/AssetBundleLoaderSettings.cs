namespace pogorock
{

	using UnityEngine;
	using System;
	using System.Collections;
	using System.Collections.Generic;

	#if UNITY_EDITOR
	using UnityEditor;

	[InitializeOnLoad]
	#endif

	public class AssetBundleLoaderSettings : ScriptableObject
	{
		public static readonly string assetName = "AssetBundleLoaderSettings";
		public static readonly string fullPath = "Assets/ExampleProject/MainDiagram/_AssetbundleLoader/Resources/AssetBundleLoaderSettings.asset";

		private static AssetBundleLoaderSettings instance = null;

		public static AssetBundleLoaderSettings Instance {
			get {
				if (instance == null) {
					instance = Resources.Load<AssetBundleLoaderSettings> (assetName);
					if (instance == null) {
						instance = CreateInstance<AssetBundleLoaderSettings> ();

						#if UNITY_EDITOR
						AssetDatabase.CreateAsset (instance, fullPath);
						#endif
					}
				}
				return instance;
			}
		}
		#if UNITY_EDITOR
		[MenuItem ("PogoTools/AssetBundle Settings #&a")]
		public static void Execute ()
		{
			Selection.activeObject = AssetBundleLoaderSettings.Instance;
		}
		#endif

		// 定义数据载体
		public List<AssetBundleLoaderTargetPath> targetPaths;

		public int actionCount;
	}

	[System.Serializable]
	public class AssetBundleLoaderTargetPath
	{
		public string targetPath;
		public bool enable;
	}
}
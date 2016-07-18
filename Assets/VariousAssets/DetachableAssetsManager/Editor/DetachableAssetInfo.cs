namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using UnityEditor;
	using Newtonsoft.Json;

	public class DetachableAssetInfo
	{
		public string Name;
		public string Description;
		public string Url;
		public string Version;

		public string DevDataPathRoot;

		public bool isMultiPaths;
		public bool rootsFolded;
		public string AssetsPathRoot;

		public AssetsPathRootInfo[] AssetsPathRoots;

		public string Symbol;
	}

	public class AssetsPathRootInfo
	{
		public string path;
		public bool integrate;
		public bool backup;
	}
}
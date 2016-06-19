using System;
using System.IO;
using System.Text;
using PrefabEvolution.Migration;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace PrefabEvolution
{
	[InitializeOnLoad]
	class PECache : ScriptableObject
	{
		[SerializeField] private PEDictionaryStringList whereCache = new PEDictionaryStringList();
		[SerializeField] private PEDictionaryStringList whatCache = new PEDictionaryStringList();
		[SerializeField] private bool allAssetsLoaded;

		static private List<string> modelPathToApply = new List<string>();
		static private PECache instance;

		static public PECache Instance
		{
			get
			{
				if (instance == null)
				{
					instance = Resources.FindObjectsOfTypeAll<PECache>().FirstOrDefault();

					if (instance == null)
					{
						instance = CreateInstance<PECache>();
					}

					SetDontSave();
				}

				return instance;
			}
		}

		static void SetDontSave()
		{
			DontDestroyOnLoad(instance);
			instance.hideFlags = HideFlags.DontSave;
		}

		static private bool PathFilter(string path)
		{
			return path.EndsWith(".png") ||
			path.EndsWith(".psd") ||
			path.EndsWith(".jpg") ||
			path.EndsWith(".jpeg") ||
			path.EndsWith(".tif") ||
			path.EndsWith(".exr") ||
			path.EndsWith(".cs") ||
			path.EndsWith(".asset");
		}

		public void CheckPrefab(params string[] paths)
		{
			paths = paths.Where(p => !PathFilter(p)).ToArray();
			var i = 0;
			var prefabsPaths = paths.Where(p => p.EndsWith(".prefab")).ToArray();

			foreach (var path in prefabsPaths)
			{
				if (prefabsPaths.Length > 5 && i++ % 50 == 0)
					EditorUtility.DisplayProgressBar("Checking prefab dependencies", path, i / (float)prefabsPaths.Length);

				if (prefabsPaths.Length > 5 && !canPrefabContainsNestedInstances(path))
					continue;

				CheckPrefab(AssetDatabase.AssetPathToGUID(path), PEUtils.GetAssetByPath<GameObject>(path));

				if (i % 100 == 0)
					continue;

#if UNITY_5
				EditorUtility.UnloadUnusedAssetsImmediate();
#else
				EditorUtility.UnloadUnusedAssets();
#endif
			}

			foreach (var path in paths.Where(p => !p.EndsWith(".prefab")))
			{

				if (!modelPathToApply.Contains(path))
					continue;

				var importer = AssetImporter.GetAtPath(path);

				if (!(importer is ModelImporter))
					continue;

				var asset = PEUtils.GetAssetByPath<GameObject>(path);
				if (asset == null)
					continue;

				var prefabScript = asset.GetComponent<PEPrefabScript>();
				if (prefabScript == null)
					continue;

				modelPathToApply.Remove(path);

				if (PEPrefs.DebugLevel > 0)
					Debug.Log("Applying " + path);
				prefabScript.Prefab = asset;
				prefabScript.BuildLinks();
				EditorApplication.delayCall += () => PEUtils.DoApply(prefabScript);
			}

			EditorUtility.ClearProgressBar();
		}

		void CheckAllAssets()
		{
			if (allAssetsLoaded)
				return;

			allAssetsLoaded = true;
			this.whereCache.list.Clear();
			this.whatCache.list.Clear();
			var startTime = DateTime.Now;
			CheckPrefab(AssetDatabase.GetAllAssetPaths());
			Debug.Log("CheckTime: " + (DateTime.Now - startTime).TotalMilliseconds);
		}

		static public void ForceCheckAllAssets()
		{
			AssetDatabase.GetAllAssetPaths().Foreach(p =>
			{
				if (p.EndsWith(".prefab"))
					setContainsNestedInstances(AssetDatabase.AssetPathToGUID(p), true);
			});
			Instance.allAssetsLoaded = false;
			Instance.CheckAllAssets();
		}

		private List<string> this[string guid]
		{
			get
			{
				CheckAllAssets();
				return whereCache[guid];
			}
		}

		public IEnumerable<GameObject> GetPrefabsWithInstances(string guid)
		{
			var list = this[guid];
			if (list == null || list.Count == 0)
				yield break;

			foreach (var g in this[guid].ToArray())
			{
				var go = PEUtils.GetAssetByGUID<GameObject>(g);
				if (go)
					yield return go;
			}
		}

		private void CheckPrefab(string guid, GameObject prefab)
		{
			if (string.IsNullOrEmpty(guid) || prefab == null)
			{
				return;
			}

			var nestedInstances = PEUtils.GetNestedInstances(prefab).ToArray();

			foreach (var e in whatCache[guid])
			{
				whereCache[e].Remove(guid);
			}

			var rootInstance = prefab.GetComponent<PEPrefabScript>();
			if (rootInstance != null)
			{
				if (rootInstance.PrefabGUID == guid && !string.IsNullOrEmpty(rootInstance.ParentPrefabGUID))
					Add(rootInstance.ParentPrefabGUID, guid);
			}

			foreach (var instance in nestedInstances)
			{
				if (instance.PrefabGUID != guid)
					Add(instance.PrefabGUID, guid);
			}
			EditorApplication.delayCall += () => 
				setContainsNestedInstances(guid, nestedInstances.Length > 0 || (rootInstance && !string.IsNullOrEmpty(rootInstance.ParentPrefabGUID)));
		}

		static string getMetaInfo(string path)
		{
			path = path + ".meta";
			if (!File.Exists(path))
				return "";
			return FileUtility.ReadAllText(path, Encoding.UTF8);
		}

		static string getMetaUserData(string meta)
		{
			var prefix = "  userData:";
			var indexOfUserData = meta.LastIndexOf(prefix);
			var indexOfEOL = meta.IndexOf("\n", indexOfUserData);
			if (indexOfEOL == -1)
				indexOfEOL = meta.Length - 1;
			var start = indexOfUserData + prefix.Length;
			var length = indexOfEOL - 1 - start;
			return meta.Substring(start, length).Trim();
		}

		private const string HaveConst = "HaveNested_true";
		private const string DontHaveConst = "HaveNested_false";

		static void setContainsNestedInstances(string guid, bool state)
		{
			try
			{
				var path = AssetDatabase.GUIDToAssetPath(guid);
				var importer = AssetImporter.GetAtPath(path);
				if (importer == null)
					return;

				var origData = importer.userData;
				var data = origData.Replace(":", "_");
				if (data != string.Empty && data != HaveConst && data != DontHaveConst)
					return;
				var newData = state ? HaveConst : DontHaveConst;

				importer.userData = newData;
			}
			catch (Exception){}
		}

		static bool canPrefabContainsNestedInstances(string path)
		{
			try
			{
				var meta = getMetaInfo(path);
				if (meta == string.Empty)
					return true;
				var data = getMetaUserData(meta).Replace(":", "_");
				if (data != HaveConst && data != DontHaveConst)
					return true;
				return data == HaveConst;
			}
			catch (Exception)
			{
				return true;
			}
		}

		void Add(string prefabGuid, string guid)
		{
			var list = this[prefabGuid];

			if (!list.Contains(guid))
				list.Add(guid);

			list = whatCache[guid];
			if (!list.Contains(prefabGuid))
				list.Add(prefabGuid);
		}

		#region Processors

		public class PrefabAssetModificationProcessor : UnityEditor.AssetModificationProcessor
		{
			static void OnWillCreateAsset(string path)
			{
				Instance.CheckPrefab(path);
			}

			static string[] OnWillSaveAssets(string[] paths)
			{
				Instance.CheckPrefab(paths);
				return paths;
			}
		}

		public class PrefabPostProcessor : AssetPostprocessor
		{
			static public void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
			{
				Instance.CheckPrefab(importedAssets);
			}
		}

		public class PrefabEvolutionPostProcessor : AssetPostprocessor
		{
			void OnPostprocessModel(GameObject gameObject)
			{
				if (PEPrefs.AutoModels)
				{
					var guid = AssetDatabase.AssetPathToGUID(this.assetPath);
				
					if (string.IsNullOrEmpty(guid))
					{
						EditorApplication.delayCall += () => AssetDatabase.ImportAsset(this.assetPath);
						return;
					}

					var pi = gameObject.AddComponent<EvolvePrefab>() as PEPrefabScript;
					pi.PrefabGUID = guid;
					pi.BuildLinks(true);
					modelPathToApply.Add(this.assetPath);
				}
			}
		}

		#endregion

		#region Menu

		#if SHOW_DEBUG_MENU
		[MenuItem("Prefab Evolution/Show Prefab Cache")]
		static public void SelectPrefapCache()
		{
			Selection.activeObject = Instance;
		}

		[MenuItem("Prefab Evolution/Update Prefab List")]
		static public void UpdatePrefabList()
		{
			Instance.allAssetsLoaded = false;
			Instance.CheckAllAssets();
		}

		#endif

		#endregion
	}
}
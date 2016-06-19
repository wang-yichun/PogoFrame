using System.Linq;
using UnityEditor;
using System;
using UnityEngine;

namespace PrefabEvolution.Migration
{
	static public class MigrationUtility
	{
		//GUID for EvolePrefab imported from dll
		public const string DllMonoScriptString = "{fileID: 852368078, guid: 3654698d89f684c0d949d17375c1b9a3, type: 3}";

		static MonoScript FindEvolvePrefabScriptAsset(out string fileID, out string guid, out string path)
		{
			var paths = AssetDatabase.GetAllAssetPaths().Where(assetPath => assetPath.EndsWith(".dll") || assetPath.EndsWith(".cs")).ToArray();
			fileID = guid = path = "";

			foreach(var assetPath in paths)
			{
				var result = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<MonoScript>().FirstOrDefault(script => script.GetClass() == typeof(EvolvePrefab));

				if (result == null)
					continue;

				path = assetPath;
				guid = AssetDatabase.AssetPathToGUID(assetPath);
				fileID = Unsupported.GetLocalIdentifierInFile(result.GetInstanceID()).ToString();
				return result;
			}

			return null;
		}

		static string GetReplaceString()
		{
			string guid, fileID, path;
			var monoScript = FindEvolvePrefabScriptAsset(out fileID, out guid, out path);
			if (monoScript == null)
				return "";

			var result = string.Format("{{fileID: {0}, guid: {1}, type: 3}}", fileID, guid);
			Debug.Log("ReplaceString:" + result);
			return result;
		}

		static public void Migrate()
		{
			var replaceString = GetReplaceString();

			if (replaceString == DllMonoScriptString)
			{
				EditorUtility.DisplayDialog("Error", "Please remove PrefabEvolution.dll and PrefabEvolution_Editor.dll files from project", "OK");
				return;
			}

			if (replaceString == "")
			{
				EditorUtility.DisplayDialog("Error", "Can't find EvolvePrefab.cs script", "OK");
				return;
			}

			if (!EditorUtility.DisplayDialog("Migration", "This operation will replace all references to EvolvePrefab imported from PrefabEvolution.dll. Would you like to start migration? ", "Start", "Later"))
				return;

			var progressTime = Environment.TickCount;
			var replacesCount = 0;
			var paths = AssetDatabase.GetAllAssetPaths().Where(path => path.EndsWith(".prefab") || path.EndsWith(".unity")).ToArray();

			for( int i = 0; i < paths.Length; i++ )
			{
				if (Environment.TickCount - progressTime > 250)
				{
					progressTime = Environment.TickCount;
					EditorUtility.DisplayProgressBar(string.Format("Migrating. Found {0} files", replacesCount), paths[i], (float)i / (float)paths.Length);
				}

				replacesCount += ReplaceInFile(paths[i], DllMonoScriptString, replaceString) ? 1 : 0;
			}
			EditorUtility.ClearProgressBar();
			AssetDatabase.Refresh();

			if (EditorUtility.DisplayDialog("Migration", "Reimport models now?", "Reimport", "Later"))
			{
				paths = AssetDatabase.GetAllAssetPaths().Where(path => AssetImporter.GetAtPath(path) is ModelImporter).ToArray();
				try
				{
					AssetDatabase.StartAssetEditing();
					foreach(var path in paths)
						AssetDatabase.ImportAsset(path);
				}
				finally
				{
					AssetDatabase.StopAssetEditing();
				}
			}

			Debug.Log(string.Format("[PrefabEvolution][Migration]Migration completed. Replaced references in {0} files", replacesCount));
		}

		static public bool ReplaceInFile(string path, string fromString, string toString)
		{
			var content = FileUtility.ReadAllText(path);
			var newContent = content.Replace(fromString, toString);
			if (content == newContent)
				return false;
				
			Debug.Log("[PrefabEvolution][Migration] Replacing in file: " + path);

			FileUtility.WriteAllText(path, newContent);
			return true;
		}
	}
}


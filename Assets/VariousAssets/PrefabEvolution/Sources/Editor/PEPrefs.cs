using UnityEditor;
using UnityEngine;

namespace PrefabEvolution
{
	static class PEPrefs
	{
		static internal int DebugLevel = 0;
		private const string PrefsPrefix = "PrefabEvolution";

		static PEPrefs()
		{
			UpdateDebugLevel();
		}

		static void UpdateDebugLevel()
		{
			DebugLevel = DebugMode ? 1 : 0;
		}

		static string GetPrefName(string name)
		{
			return PrefsPrefix + name;
		}

		static internal bool AutoModels
		{
			get
			{
				return EditorPrefs.GetBool(GetPrefName("AutoModels"), true);
			}

			private set
			{
				if (AutoModels != value)
					EditorPrefs.SetBool(GetPrefName("AutoModels"), value);
			}
		}

		static internal bool AutoPrefabs
		{
			get
			{
				return EditorPrefs.GetBool(GetPrefName("AutoPrefabs"), false);
			}

			private set
			{
				if (AutoPrefabs != value)
					EditorPrefs.SetBool(GetPrefName("AutoPrefabs"), value);
			}
		}

		static internal bool DrawOnHeader
		{
			get
			{
				return EditorPrefs.GetBool(GetPrefName("DrawOnHeader"), true);
			}

			private set
			{
				if (DrawOnHeader != value)
					EditorPrefs.SetBool(GetPrefName("DrawOnHeader"), value);
			}
		}

		static internal bool AutoSaveAfterApply
		{
			get
			{
				return EditorPrefs.GetBool(GetPrefName("AutoSaveAfterApply"), false);
			}

			private set
			{
				if (AutoSaveAfterApply != value)
					EditorPrefs.SetBool(GetPrefName("AutoSaveAfterApply"), value);
			}
		}

		static internal bool DebugMode
		{
			get
			{
				return EditorPrefs.GetBool(GetPrefName("DebugMode"), false);
			}

			private set
			{
				if (DebugMode != value)
				{
					EditorPrefs.SetBool(GetPrefName("DebugMode"), value);
					UpdateDebugLevel();
				}
			}
		}

		static private Vector2 ScrollPos;

		#if TRIAL
		[PreferenceItem("Prefab Evolution Free")]
		#else
		[PreferenceItem("Prefab Evolution")]
		#endif
		static void OnPrefsGUI()
		{
			ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos);
			EditorGUILayout.HelpBox("Automaticly add \"EvolvePrefab\" script to models on import. ", MessageType.Info);
			AutoModels = EditorGUILayout.Toggle("Make imported models nested", AutoModels);

			EditorGUILayout.HelpBox("Automaticly add \"EvolvePrefab\" script to new prefab", MessageType.Info);
			AutoPrefabs = EditorGUILayout.Toggle("Make prefab nested on create", AutoPrefabs);

			EditorGUILayout.HelpBox("Show utility buttons on GameObject Inspector header", MessageType.Info);
			DrawOnHeader = EditorGUILayout.Toggle("Show in header", DrawOnHeader);

			EditorGUILayout.HelpBox("Force save all Assets after applying changes to prefab", MessageType.Info);
			AutoSaveAfterApply = EditorGUILayout.Toggle("Auto save", AutoSaveAfterApply);

			EditorGUILayout.HelpBox("Plugin will write detailed log", MessageType.Info);
			DebugMode = EditorGUILayout.Toggle("Debug mode", DebugMode);

			EditorGUILayout.HelpBox("Check all prefabs", MessageType.Info);
			if (GUILayout.Button("Check prefab dependencies"))
				PECache.ForceCheckAllAssets();

			EditorGUILayout.EndScrollView();
		}
	}
}
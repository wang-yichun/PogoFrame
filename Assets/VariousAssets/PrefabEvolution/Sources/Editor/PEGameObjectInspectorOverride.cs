using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace PrefabEvolution
{
	[CustomEditor(typeof(GameObject))]
	[CanEditMultipleObjects]
	[OverrideInternalEditorTypeMark("GameObjectInspector")]
	class PEGameObjectInspectorOverride : PEOverrideInternalEditor
	{
		private PEExposedPropertiesEditor exposedPropertiesEditor;

		protected override void OnEnable()
		{
			base.OnEnable();

			var prefabScripts = this.targets.OfType<GameObject>().Select(t => t.GetComponent<PEPrefabScript>()).Where(t => t != null).ToArray();
			if (prefabScripts.Any())
				exposedPropertiesEditor = new PEExposedPropertiesEditor(prefabScripts);
		}

		protected override void OnHeaderGUI()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				base.OnHeaderGUI();
				return;
			}
			var rect = GUILayoutUtility.GetRect(0, 0);
			float buttonWidth = (rect.width - 52f - 5f) / 3f;
			var applyButtonPosition = new Rect(52f + buttonWidth * 2f, 44f + rect.y, buttonWidth, 15f);

			var list = new List<GameObject>();
			
			foreach (var t in this.targets)
			{
				var go = t as GameObject;
				if (go == null)
					continue;
				
				var root = PrefabUtility.FindPrefabRoot(go);
				list.RemoveAll(r => r == root);
				list.Add(root);
			}

			if (targets.Length > 1)
			{
				base.OnHeaderGUI();
					
				if (list.Count > 0 && GUILayout.Button("Apply " + list.Count + (list.Count > 1 ? " Prefabs" : " Prefab"), EditorStyles.miniButton))
				{
					Apply();
				}
			}
			else
			{
				var gameObject = (target as GameObject);
				var prefabType = PrefabUtility.GetPrefabType(gameObject);

				var isPrefab = prefabType == PrefabType.Prefab;
				var isPrefabInstance = prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance;
				var isPrefabOrPrefabInstance = prefabType == PrefabType.Prefab || isPrefabInstance;

				gameObject = PrefabUtility.FindValidUploadPrefabInstanceRoot(gameObject);

				if (gameObject == null)
					gameObject = target as GameObject;

				var prefabInstance = gameObject != null ? gameObject.GetComponent<PEPrefabScript>() : null;
				var anyPrefabInstance = gameObject != null && gameObject.GetComponentsInChildren<PEPrefabScript>(true).Any();
				var isNestedPrefab = prefabInstance != null;

				if (isPrefabInstance && (isNestedPrefab || anyPrefabInstance))
				{
					var c = GUI.color;
					GUI.color = new Color(0.95f, 1, 0.95f, 1);

					if (Event.current.type == EventType.MouseUp && applyButtonPosition.Contains(Event.current.mousePosition))
					{
						Event.current.mousePosition = Vector2.one * 5000f;
						Apply();
					}

					base.OnHeaderGUI();
					GUI.color = c;
				}
				else
				{
					base.OnHeaderGUI();
					if (isPrefab && (isNestedPrefab || anyPrefabInstance))
					{
						GUILayout.Space(2);
						if (GUILayout.Button("Apply", EditorStyles.miniButton))
						{
							Apply();
						}
					}
				}
				var isRoot = PrefabUtility.FindValidUploadPrefabInstanceRoot(target as GameObject) == target;
				if (isPrefabOrPrefabInstance && !isNestedPrefab && isRoot)
				{
					GUILayout.Space(3);
					if (GUILayout.Button(new GUIContent("Allow this prefab to be nested"), EditorStyles.miniButton))
						PEUtils.MakeNested(gameObject);
				}
			}

			var prefabsWinthoutPrefabScript = list.Where(p => PrefabUtility.GetPrefabType(p) == PrefabType.Prefab && p.GetComponent<PEPrefabScript>() == null).ToArray();
			if (prefabsWinthoutPrefabScript.Length > 0)
			{
				GUILayout.Space(3);
				if (GUILayout.Button(new GUIContent("Allow this prefabs to be nested"), EditorStyles.miniButton))
					prefabsWinthoutPrefabScript.Foreach(PEUtils.MakeNested);
			}

			if (PEPrefs.DrawOnHeader && targets.Length == 1)
			{
				var go = (target as GameObject);
				if (go != null)
				{
					var prefabScript = go.GetComponent<PEPrefabScript>();

					if (prefabScript)
						PEPrefabScriptEditor.DrawView(prefabScript);
				}
			}
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (PEPrefs.DrawOnHeader)
			{
				if (exposedPropertiesEditor != null)
					exposedPropertiesEditor.Draw();
			}
		}

		void Apply()
		{
			var objs = targets.OfType<GameObject>();
			PEUtils.ApplyPrefab(objs.ToArray());
		}

		[MenuItem("GameObject/Apply Changes To Prefab")]
		static void MenuMock()
		{
			PEUtils.ApplyPrefab(Selection.gameObjects);
		}
	}
}

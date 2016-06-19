using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using PrefabEvolution;
using Object = UnityEngine.Object;

[CustomEditor(typeof(PEPrefabScript), true)]
class PEPrefabScriptEditor : Editor
{
	private PETreeNode rootNode;
	private PEAutoDict<Object, bool> expandedDict = new PEAutoDict<Object, bool>(false);

	PETreeNode Add(GameObject go, PETreeNode parent, bool includeChildren = true)
	{
		var isExpanded = expandedDict[go];

		var node = new PETreeNode(isExpanded) { content = new GUIContent(go.name, PEResources.icon), UserData = go };
		parent.children.Add(node);
		node.OnExpandChanged += expanded => expandedDict[go] = expanded;

		var pi = go.GetComponent<PEPrefabScript>();
		if (pi)
		{
			var isRoot = PrefabUtility.FindPrefabRoot(pi.gameObject) == pi.Prefab ||
			             PrefabUtility.GetPrefabParent(pi.gameObject) == pi.Prefab;
			if (isRoot)
			{
				node.color = Color.green;
				node.content.text += " (Root)";
			}
			else
			{
				node.color = Color.yellow;
				node.content.text += " (Nested)";
			}
		}
		if (prefabScript.Modifications.TransformParentChanges.Any(npo => npo.child == go.transform))
		{
			node.color = Color.cyan;
			node.content.text += " (Parent Changed)";
		}
		else
		if (prefabScript.Modifications.NonPrefabObjects.Any(c => c.child == go.transform))
		{
			node.color = Color.yellow;
			node.content.text += " (New)";
		}

		if (!includeChildren)
			return node;

		foreach (var property in GetProperties(go))
		{
			Add(property, node);
		}

		foreach (Component component in go.GetComponents<Component>())
		{
			Add(component, node);
		}

		foreach (Transform transform in go.transform)
		{
			var nd = Add(transform.gameObject, node);
			if (nd.children.Count == 0 && nd.color == Color.white)
				node.children.Remove(nd);
		}

		foreach (var obj in GetRemovedObjects(go, prefabScript))
		{
			PETreeNode nd = null;
			var component = obj as Component;
			if (component != null)
			{
				nd = Add(component, node, false);
			}
			else
			{
				var gameObject = obj as GameObject;
				if (gameObject != null)
				{
					nd = Add(gameObject, node, false);
				}
			}

			if (nd != null)
			{
				nd.color = Color.red;
				nd.content.text += " (Removed)";
			}
		}

		return node;
	}

	PETreeNode Add(SerializedProperty property, PETreeNode parent)
	{
		var node = new PETreeNode.PropertyNode { UserData = property };
		parent.children.Add(node);
		return node;
	}

	PETreeNode Add(Component component, PETreeNode parent, bool includeChildren = true)
	{
		var isNewComponent = prefabScript.Modifications.NonPrefabComponents.Any(c => c.child == component) &&
							 !(component is PEPrefabScript);

		if (!isNewComponent && !includeChildren)
			return null;
		var isExpanded = expandedDict[component];
		var node = new PETreeNode(isExpanded)
		{
			content = new GUIContent(component.GetType().Name, EditorGUIUtility.ObjectContent(component, component.GetType()).image),
			UserData = component
		};
		node.OnExpandChanged += expanded => expandedDict[component] = expanded;

		if (isNewComponent)
		{
			node.content.text += " (New)";
			node.color = Color.yellow;
		}
		else
		{
			var propertiesOverrides = GetProperties(component).ToArray();

			if (propertiesOverrides.Length == 0)
				return null;

			foreach (var property in propertiesOverrides)
			{
				Add(property, node);
			}
		}

		parent.children.Add(node);
		return node;
	}

	IEnumerable<SerializedProperty> GetProperties(Object obj)
	{
		var modifications = serializedObject.FindProperty("Modifications.Modificated");
		modifications.Next(true);
		for (var i = 0; i < modifications.arraySize; i++)
		{
			var property = modifications.GetArrayElementAtIndex(i);
			if (property.FindPropertyRelative("Object").objectReferenceValue == obj)
				yield return property;
		}
	}

	void OnEnable()
	{
		this.exposedPropertiesEditor = new PEExposedPropertiesEditor(this.targets.OfType<PEPrefabScript>().ToArray());
		rootNode = new PETreeNode(false) { content = new GUIContent("Modifications") };
		rootNode.children.Add(new PETreeNode());

		Action<bool> d = obj =>
		{
			if (obj)
			{
				prefabScript.BuildModifications();

				UpdateTree();
				prefabScript.OnBuildModifications += UpdateTree;
			}
			else
			{
				prefabScript.OnBuildModifications -= UpdateTree;
			}
		};
		rootNode.OnExpandChanged += d;
	}

	void OnDisable()
	{
		this.prefabScript.OnBuildModifications -= UpdateTree;
	}

	void UpdateTree()
	{
		serializedObject.Update();

		rootNode = new PETreeNode(rootNode.Expanded) { content = new GUIContent("Modifications") };
		Add(prefabScript.gameObject, rootNode);
	}

	static IEnumerable<Object> GetRemovedObjects(Object go, PEPrefabScript prefabInstance)
	{
		foreach (var liif in prefabInstance.Modifications.RemovedObjects)
		{
			var instanceObj = prefabInstance.GetDiffWith().Links[liif];
			if (instanceObj == null)
				continue;

			var removedGO = instanceObj.InstanceTarget as GameObject;
			var removedComponent = instanceObj.InstanceTarget as Component;

			if (removedComponent is PEPrefabScript)
				continue;

			var remoteParent = (removedGO != null) ? (removedGO.transform.parent == null ? removedGO.transform.gameObject : removedGO.transform.parent.gameObject) : (removedComponent != null ? removedComponent.gameObject : null);

			var localLink = prefabInstance.Links[prefabInstance.GetDiffWith().Links[remoteParent]];
			if (localLink == null)
				continue;

			var localParent = localLink.InstanceTarget;

			if (localParent == go)
				yield return instanceObj.InstanceTarget;
		}
	}

	internal PEPrefabScript prefabScript
	{
		get
		{
			return this.target as PEPrefabScript;
		}
	}

	static internal void DrawView(PEPrefabScript script)
	{
		GUILayout.Space(3);
		var icon = EditorGUIUtility.ObjectContent(null, typeof(GameObject));
		GUILayout.BeginHorizontal();

		if (!string.IsNullOrEmpty(script.ParentPrefabGUID))
		{
			var c = GUI.backgroundColor;

			if (!script.ParentPrefab)
				GUI.backgroundColor = Color.red;

			var content = new GUIContent(script.ParentPrefab ? script.ParentPrefab.name : "Missing:" + script.ParentPrefabGUID, icon.image);

			GUILayout.Label("Parent:", GUILayout.Width(50));

			if (GUILayout.Button(content, EditorStyles.miniButton, GUILayout.Height(16), GUILayout.MinWidth(0)))
				EditorGUIUtility.PingObject(script.ParentPrefab);

			GUI.backgroundColor = c;
		}
		if (!string.IsNullOrEmpty(script.PrefabGUID))
		{
			var c = GUI.backgroundColor;

			if (!script.Prefab)
				GUI.backgroundColor = Color.red;

			var content = new GUIContent(script.Prefab ? script.Prefab.name : "Missing:" + script.PrefabGUID, icon.image);

			GUILayout.Label("Prefab:", GUILayout.Width(50));

			if (GUILayout.Button(content, EditorStyles.miniButton, GUILayout.Height(16), GUILayout.MinWidth(0)))
				EditorGUIUtility.PingObject(script.Prefab);

			GUI.backgroundColor = c;
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(1);
		DrawCommands(script);

		EditorGUIUtility.labelWidth = 150;		
	}

	private PEExposedPropertiesEditor exposedPropertiesEditor;

	public override void OnInspectorGUI()
	{
		var script = prefabScript;
		if (!PEPrefs.DrawOnHeader)
		{
			DrawView(script);
			exposedPropertiesEditor.Draw();
		}

		if ((script.hideFlags & HideFlags.NotEditable) != HideFlags.NotEditable)
		{
			DrawDefault();
			if (rootNode != null)
				rootNode.Draw();
		}
	}

	static void DrawCommands(PEPrefabScript prefabScript)
	{
		var e = GUI.enabled;
		GUI.enabled = true;
		if (GUILayout.Button("Menu", EditorStyles.miniButton))
		{
			var menu = new GenericMenu();
			PEUtils.BuildMenu(menu, prefabScript, PrefabUtility.GetPrefabParent(prefabScript.gameObject) == prefabScript.Prefab);
			menu.ShowAsContext();
		}
		GUI.enabled = e;
	}

	void DrawDefault()
	{
		var obj = serializedObject;

		EditorGUI.BeginChangeCheck();
		obj.Update();
		SerializedProperty iterator = obj.GetIterator();
		bool enterChildren = true;
		while (iterator.NextVisible(enterChildren))
		{
			if (iterator.name != "m_Script" && iterator.name != "Modifications" && iterator.name != "PrefabGUID" && iterator.name != "ParentPrefabGUID")
				EditorGUILayout.PropertyField(iterator, true);
			enterChildren = false;
		}
		obj.ApplyModifiedProperties();
		EditorGUI.EndChangeCheck();
	}
}
//}


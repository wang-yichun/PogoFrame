using UnityEditor.VersionControl;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEditor;

namespace PrefabEvolution
{
	[InitializeOnLoad]
	static internal class PEUtils
	{
		static PEUtils()
		{
			EditorUtility.ClearProgressBar();//
			EditorApplication.hierarchyWindowItemOnGUI += OnItemGUI;
		}

		#region ReplaceReference

		static internal void ReplaceReference(Object root, Object from, Object to)
		{
			ReplaceReference(EditorUtility.CollectDeepHierarchy(new [] { root }), from, to);
		}

		static internal void ReplaceReference(Object[] roots, Object from, Object to)
		{
			var dict = new Dictionary<Object, Object>();
			dict.Add(from, to);
			ReplaceReference(roots, dict);
		}

		static internal void ReplaceReference(Object root, IDictionary<Object, Object> dict)
		{
			ReplaceReference(EditorUtility.CollectDeepHierarchy(new [] { root }), dict);
		}

		static internal void ReplaceReference(Object[] roots, IDictionary<Object, Object> dict)
		{
			foreach (var obj in roots)
			{
				if (obj == null)
					continue;

				var so = new SerializedObject(obj);
				var property = so.GetIterator();
				while (property.Next(true))
				{
					if (property.propertyType != SerializedPropertyType.ObjectReference)
						continue;

					if (PropertyFilter(property))
						continue;

					if (property.objectReferenceValue == null)
						continue;

					Object result;

					if (!dict.TryGetValue(property.objectReferenceValue, out result))
						continue;

					property.objectReferenceValue = result;
					property.serializedObject.ApplyModifiedProperties();
				}
			}
		}

		#endregion

		#region etc
		static public void Foreach<T>(this IEnumerable<T> targets, System.Action<T> action)
		{
			foreach (var t in targets)
			{
				action(t);
			}
		}

		static internal void SetParentAndSaveLocalTransform(Transform child, Transform parent)
		{
			var lp = child.localPosition;
			var lr = child.localRotation;
			var ls = child.localScale;

			#if UNITY_4_6 || UNITY_5_0 || UNITY_5
			Vector2 sizeDelta = Vector2.zero;
			var rectTransform = child as RectTransform;
			if (rectTransform)
				sizeDelta = rectTransform.sizeDelta;

			child.parent = parent;

			if (rectTransform)
				rectTransform.sizeDelta = sizeDelta;
			#else
			child.parent = parent;
			#endif

			child.localPosition = lp;
			child.localRotation = lr;
			child.localScale = ls;
		}

		static internal IEnumerable<PEPrefabScript> GetNestedInstances(GameObject gameObject)
		{
			var instances = gameObject.GetComponentsInChildren<PEPrefabScript>(true);
			var rootInstance = gameObject.GetComponent<PEPrefabScript>();

			foreach (var instance in instances)
			{
				if (instance == rootInstance)
					continue;

				var parentInstance = instance.gameObject.GetInParent<PEPrefabScript>();

				if (parentInstance == null)
				{
					yield return instance;
				}
				else
				if (parentInstance == rootInstance)
				{
					if (rootInstance.ParentPrefab == null)
						yield return instance;
					else
					if (rootInstance.ParentPrefab.GetComponent<PEPrefabScript>().Links[parentInstance.Links[instance]] == null)
						yield return instance;
				}
				else
				{
					if (parentInstance.IsNonPrefabObject(instance))
						yield return instance;
				}
			}
		}

		private static T GetInParent<T>(this GameObject obj) where T : MonoBehaviour
		{
			var item = obj.transform.parent;
			while (item != null)
			{
				var result = item.GetComponent<T>();

				if (result != null)
					return result;

				item = item.parent;
			}

			return null;
		}

		static internal void ExecuteOnEditorUpdate(this IEnumerator enumerator)
		{
			EditorApplication.CallbackFunction self = null;
			EditorApplication.CallbackFunction func = () =>
			{
				if (!enumerator.MoveNext())
					EditorApplication.update -= self;
			};
			self = func;

			EditorApplication.update += func;
		}

		#endregion

		#region GUI
		static internal GUIStyle emptyStyle = new GUIStyle();

		static void OnItemGUI(int instanceID, Rect rect)
		{
			var instance = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
			if (instance == null)
				return;

			var prefabInstance = instance.GetComponent<PEPrefabScript>();

			var isPrefab = PrefabUtility.GetPrefabParent(instance) && PrefabUtility.FindPrefabRoot(instance) == instance;
		
			if (prefabInstance)
			{

				bool rootPrefab = PrefabUtility.GetPrefabParent(prefabInstance.gameObject) == prefabInstance.Prefab;
				var color = GUI.color;
				GUI.color = rootPrefab ? Color.green : (Color.yellow);

				if (!prefabInstance.enabled)
					GUI.color = Color.white;

				if (prefabInstance.Prefab == null)
					GUI.color = Color.red;

				const int width = 15;
				var br = rect;
				br.height -= 2;
				br.y += 2 / 2;
				br.x += br.width - width;
				br.width = width;

				var content = new GUIContent(PEResources.icon, prefabInstance.Prefab ? prefabInstance.Prefab.name : "Missiog prefab with guid: " + prefabInstance.PrefabGUID);
				var click = GUI.Button(br, content, emptyStyle);

				GUI.color = color;

				var evt = Event.current;
				if (prefabInstance.Prefab && (evt.type == EventType.ContextClick || click || evt.type == EventType.MouseUp))
				{
					var mousePos = evt.mousePosition;
					if (br.Contains(mousePos))
					{
						var menu = new GenericMenu();
						BuildMenu(menu, prefabInstance, rootPrefab);

						menu.ShowAsContext();
						evt.Use();
					}
				}
			}
			else
			if (isPrefab)
			{
				var click = PEPrefs.AutoPrefabs;
				if (click)
					MakeNested(instance);
			}
		}
		static private LinkedList<GameObject> buildMenuRecursionList = new LinkedList<GameObject>();
		static internal void BuildMenu(GenericMenu menu, PEPrefabScript prefabInstance, bool rootPrefab, string path = "", bool showParent = true, bool showInstances = true)
		{
			if (prefabInstance == null)
				return;

			if (buildMenuRecursionList.Contains(prefabInstance.Prefab))
			{
				buildMenuRecursionList.AddLast(prefabInstance.Prefab);
				var prefabsArray = buildMenuRecursionList.Select(p => AssetDatabase.GetAssetPath(p)).ToArray();
				buildMenuRecursionList.Clear();
				throw new System.Exception("Prefab recursion detected:\n" + string.Join("\n", prefabsArray));
			}
			buildMenuRecursionList.AddLast(prefabInstance.Prefab);

			if (prefabInstance.ParentPrefab == null || !showParent)
				menu.AddItem(new GUIContent(path + prefabInstance.Prefab.name), false, () =>
				{
				});
			else
			{
				BuildMenu(menu, prefabInstance.ParentPrefab.GetComponent<PEPrefabScript>(), false, path + prefabInstance.Prefab.name + "/", true, false);
				menu.AddItem(new GUIContent(path + prefabInstance.Prefab.name), false, () =>
				{
				});
			}

			menu.AddSeparator(path + "");
			var isPrefab = prefabInstance.gameObject == prefabInstance.Prefab.gameObject;

			menu.AddItem(new GUIContent(path + "Select"), false, SelectPrefab, prefabInstance);

			var prefabType = PrefabUtility.GetPrefabType(prefabInstance.gameObject);
			var canApply = rootPrefab && prefabType != PrefabType.ModelPrefab && prefabType != PrefabType.ModelPrefabInstance && prefabType != PrefabType.DisconnectedModelPrefabInstance;

			if (canApply)
			{
				menu.AddItem(new GUIContent(path + "Apply"), false, Apply, prefabInstance);
			}
			if (!AssetDatabase.Contains(prefabInstance) || !isPrefab)
			{
				menu.AddItem(new GUIContent(path + "Revert"), false, Revert, prefabInstance);

				if (prefabInstance.ParentPrefab != null)
					menu.AddItem(new GUIContent(path + "Revert To Parent"), false, RevertToParent, prefabInstance);
			}
			menu.AddSeparator(path + "");
			menu.AddItem(new GUIContent(path + "Create Child"), false, CreateChild, prefabInstance);

			#if INJECTION
			if (prefabInstance.ParentPrefab != null)
				menu.AddItem(new GUIContent(path + "Insert Parent"), false, InjectParent, prefabInstance);
			#endif

			if (!rootPrefab && !AssetDatabase.Contains(prefabInstance))
			{
				menu.AddSeparator(path);
				if (prefabInstance.enabled)
					menu.AddItem(new GUIContent(path + "Disable"), false, obj => (obj as PEPrefabScript).enabled = false, prefabInstance);
				else
					menu.AddItem(new GUIContent(path + "Enable"), false, obj => (obj as PEPrefabScript).enabled = true, prefabInstance);
			}

			menu.AddSeparator(path);

			if (prefabInstance.GetPrefabsWithInstances().Any())
				menu.AddItem(new GUIContent(path + "Instances/Select All Instances"), false, SelectInstances, prefabInstance);
			if (showInstances)
			foreach (var prefab in prefabInstance.GetPrefabsWithInstances())
			{
				if (prefab == null)
					continue;
				var pi = prefab.GetComponent<PEPrefabScript>();

				var name = prefab.name;

				name = (pi != null && pi.ParentPrefab == prefabInstance.Prefab) ? "Child: " + name : "Contains in: " + name;


				if (pi != null)
					BuildMenu(menu, prefab.GetComponent<PEPrefabScript>(), false, path + "Instances/" + name + "/", false);

				var current = prefab;
				menu.AddItem(new GUIContent(path + "Instances/" + name), false, () =>
				{
					Selection.activeObject = current;
				});
			}

			menu.AddItem(new GUIContent(path + "Instantiate"), false, 
				pi => Selection.activeObject = PrefabUtility.InstantiatePrefab(((PEPrefabScript)pi).Prefab), prefabInstance);

			if (!AssetDatabase.Contains(prefabInstance))
				menu.AddItem(new GUIContent(path + "Replace"), false, Replace, prefabInstance);
			buildMenuRecursionList.Remove(prefabInstance.Prefab);
		}

		static internal void Replace(object prefabInstance)
		{
			SelectObjectRoutine(prefabInstance as PEPrefabScript).ExecuteOnEditorUpdate();
		}

		static IEnumerator SelectObjectRoutine(PEPrefabScript  prefabInstance)
		{
			EditorGUIUtility.ShowObjectPicker<GameObject>(null, false, "t:Prefab", 1);
			Object obj = null; 
			while (EditorGUIUtility.GetObjectPickerControlID() == 1)
			{
				obj = EditorGUIUtility.GetObjectPickerObject();

				yield return null;
			}
			if (obj != null && EditorUtility.DisplayDialog("Replace", string.Format("Do you want to replace {0} with {1}", prefabInstance.gameObject.name, obj.name), "Replace", "Cancel"))
			{
				prefabInstance.ReplaceInPlace(obj as GameObject, EditorUtility.DisplayDialog("Replace", "Apply modifications from current instance?", "Apply", "Don't apply"), false);
			}
		}

		static internal bool IsCousin(GameObject b, GameObject c)
		{
			if (b == null || c == null)
				return false;

			var bi = b.GetComponent<PEPrefabScript>();
			var ci = c.GetComponent<PEPrefabScript>();

			return bi != null && ci != null && (ci.ParentPrefab == bi.Prefab || IsCousin(b, ci.ParentPrefab));
		}

		static internal void MakeNested(GameObject instance)
		{
			var parent = PrefabUtility.GetPrefabParent(instance);
			var prefab = parent == null ? instance : parent as GameObject;
			if (prefab.GetComponent<EvolvePrefab>() != null)
				return;

			var pi = (PEPrefabScript)prefab.AddComponent<EvolvePrefab>();
			pi.Prefab = prefab;
			pi.BuildLinks();
		}

		static void SelectInstances(object obj)
		{
			Selection.objects = ((PEPrefabScript)obj).GetPrefabsWithInstances().ToArray();
		}

		static void Apply(object obj)
		{
			DoApply((PEPrefabScript)obj);
		}

		static internal void DoApply(PEPrefabScript script)
		{
			if (PEPrefs.DebugLevel > 0)
				Debug.Log("DoApply Start");

			script.ApplyChanges(true);

			if (PEPrefs.DebugLevel > 0)
				Debug.Log("DoApply Completed");

			DoAutoSave();
		}

		static internal void DoAutoSave()
		{
			EditorUtility.ClearProgressBar();
			if (PEPrefs.AutoSaveAfterApply)
				EditorApplication.delayCall += AssetDatabase.SaveAssets;
		}

		static internal void RevertToParent(object obj)
		{
			((PEPrefabScript)obj).RevertToParent();
		}

		static internal  void Revert(object obj)
		{
			((PEPrefabScript)obj).Revert();
		}

		static internal void CreateChild(object obj)
		{
			var pi = (PEPrefabScript)obj;
			
			var path = AssetDatabase.GenerateUniqueAssetPath(System.IO.Path.ChangeExtension(AssetDatabase.GUIDToAssetPath(pi.PrefabGUID), null) + "_Child.prefab");
			var go = PrefabUtility.CreatePrefab(path, pi.gameObject, ReplacePrefabOptions.Default);
			var prefabInstance = go.GetComponent<PEPrefabScript>();

			prefabInstance.Prefab = go;
			prefabInstance.ParentPrefab = pi.Prefab;
			prefabInstance.ParentPrefabGUID = pi.PrefabGUID;
			prefabInstance.BuildModifications();

			Selection.activeObject = PrefabUtility.InstantiatePrefab(go);
			AssetDatabase.ImportAsset(path);
			PECache.Instance.CheckPrefab(path);
		}

		static internal void InjectChild(PEPrefabScript obj, GameObject[] children)
		{	
			var path = AssetDatabase.GenerateUniqueAssetPath(System.IO.Path.ChangeExtension(AssetDatabase.GUIDToAssetPath(obj.PrefabGUID), null) + "_Child_Injected.prefab");

			var go = PrefabUtility.CreatePrefab(path, obj.gameObject, ReplacePrefabOptions.Default);

			var prefabInstance = go.GetComponent<PEPrefabScript>();
			prefabInstance.Prefab = go;
			prefabInstance.ParentPrefabGUID = obj.PrefabGUID;
			prefabInstance.BuildModifications();

			Selection.activeObject = PrefabUtility.InstantiatePrefab(go);
			AssetDatabase.ImportAsset(path);

			PECache.Instance.CheckPrefab(path);

			foreach (var child in children)
			{
				child.GetComponent<PEPrefabScript>().ParentPrefabGUID = AssetDatabase.AssetPathToGUID(path);
				PECache.Instance.CheckPrefab(AssetDatabase.GUIDToAssetPath(child.GetComponent<PEPrefabScript>().PrefabGUID));
			}
		}

		static internal void InjectParent(object obj)
		{
			var pi = (PEPrefabScript)obj;
			
			var path = AssetDatabase.GenerateUniqueAssetPath(System.IO.Path.ChangeExtension(AssetDatabase.GUIDToAssetPath(pi.PrefabGUID), null) + "_Parent_Inserted.prefab");

			var go = PrefabUtility.CreatePrefab(path, pi.gameObject, ReplacePrefabOptions.Default);

			var prefabInstance = go.GetComponent<PEPrefabScript>();
			prefabInstance.Prefab = go;
			prefabInstance.ParentPrefabGUID = pi.ParentPrefabGUID;
			prefabInstance.BuildModifications();

			Selection.activeObject = PrefabUtility.InstantiatePrefab(go);
			AssetDatabase.ImportAsset(path);

			PECache.Instance.CheckPrefab(path);
			pi.ParentPrefabGUID = prefabInstance.PrefabGUID;
			PECache.Instance.CheckPrefab(AssetDatabase.GUIDToAssetPath(pi.PrefabGUID));
		}

		static void SelectPrefab(object obj)
		{
			Selection.activeObject = ((PEPrefabScript)obj).Prefab;
		}

		static internal IEnumerable<GameObject> GetPrefabsWithInstance(string guid)
		{
			return PECache.Instance.GetPrefabsWithInstances(guid);
		}

		#endregion

		#region Assets

		static public void ApplyPrefab(GameObject gameObject)
		{
			gameObject = PrefabUtility.FindPrefabRoot(gameObject);

			var pi = gameObject.GetComponent<PEPrefabScript>();

			if (pi == null)
			{
				DefaultApply(gameObject);
				DoAutoSave();
			}
			else
				DoApply(pi);
		}

		static public void ApplyPrefab(GameObject[] targets)
		{
			var list = new List<GameObject>();

			foreach (var target in targets)
			{
				var root = PrefabUtility.FindPrefabRoot(target);
				list.RemoveAll(r => r == root);
				list.Add(root);
			}

			foreach (GameObject target in list)
				ApplyPrefab(target);
		}

		static void DefaultApply(GameObject obj)
		{
			foreach (var pi in obj.GetComponentsInChildren<PEPrefabScript>(true))
				pi.BuildModifications();

			var gameObject = obj;
			var prefabType = PrefabUtility.GetPrefabType(gameObject);

			if (prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance)
			{
				var gameObject2 = PrefabUtility.FindValidUploadPrefabInstanceRoot(gameObject);

				if (gameObject2 == null)
					return;

				var prefabParent = PrefabUtility.GetPrefabParent(gameObject2);
				var assetPath = AssetDatabase.GetAssetPath(prefabParent);

				var method = typeof(Provider).GetMethod("PromptAndCheckoutIfNeeded", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
				var canReplace = (bool)method.Invoke(null, new object[] {
					new [] {
						assetPath
					}, "The version control requires you to checkout the prefab before applying changes."
				});

				if (canReplace)
				{
					PrefabUtility.ReplacePrefab(gameObject2, prefabParent, ReplacePrefabOptions.ConnectToPrefab);
				}
			}
		}

		static internal T GetAssetByGUID<T>(string GUID) where T : Object
		{
			return GetAssetByPath<T>(AssetDatabase.GUIDToAssetPath(GUID));
		}

		static internal string GetAssetGUID(Object asset)
		{
			return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(asset));
		}

		static internal T GetAssetByPath<T>(string assetPath) where T : Object
		{
			return AssetDatabase.LoadAssetAtPath(assetPath, typeof(T)) as T;
		}

		#endregion

		static internal T GetInstance<T>(this SerializedProperty property)
		{
			return (T)GetInstance(property);
		}

		static internal object GetInstance(object obj, string path)
		{
			path = path.Replace(".Array.data", "");
			var split = path.Split('.');

			var stack = split;
			object v = obj;
			try
			{
				foreach (var name in stack)
				{
					if (name.Contains("["))
					{
						var n = name.Split('[', ']');
						v = GetField(v, n[0], int.Parse(n[1]));
					}
					else
						v = GetField(v, name);
				}
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
				return null;
			}

			return v;
		}

		static internal object GetInstance(this SerializedProperty property)
		{
			var obj = property.serializedObject.targetObject;
			var path = property.propertyPath;

			return GetInstance(obj, path);
		}

		private static object GetField(object obj, string field, int index = -1)
		{
			try
			{
				var obj2 = obj.GetType().GetField(field).GetValue(obj);
				return index == -1 ? obj2 : (obj2 as IList)[index];
			}
			catch (System.Exception)
			{
				return null;
			}
		}

		#region Serialization

		static internal bool PropertyFilter(SerializedProperty property)
		{
			return property.propertyPath.Contains("m_Prefab") ||
			property.propertyPath.Contains("m_FileID") ||
			property.propertyPath.Contains("m_PathID") ||
			property.propertyPath.Contains("m_ObjectHideFlags") ||
			property.propertyPath.Contains("m_Children") ||
			property.propertyPath.Contains("m_Father") ||
			property.propertyPath.Contains("m_GameObject") ||
			property.propertyPath.Contains("m_Component");
		}

		static internal object GetPropertyValue(this SerializedProperty prop)
		{
			switch (prop.propertyType)
			{
				case SerializedPropertyType.Integer:
					return prop.intValue;
				case SerializedPropertyType.Boolean:
					return prop.boolValue;
				case SerializedPropertyType.Float:
					return prop.floatValue;
				case SerializedPropertyType.String:
					return prop.stringValue;
				case SerializedPropertyType.Color:
					return prop.colorValue;
				case SerializedPropertyType.ObjectReference:
					return prop.objectReferenceValue;
				case SerializedPropertyType.LayerMask:
					return prop.intValue;
				case SerializedPropertyType.Enum:
					return prop.enumValueIndex;
				case SerializedPropertyType.Vector2:
					return prop.vector2Value;
				case SerializedPropertyType.Vector3:
					return prop.vector3Value;
				case SerializedPropertyType.Quaternion:
					return prop.quaternionValue;
				case SerializedPropertyType.Rect:
					return prop.rectValue;
				case SerializedPropertyType.ArraySize:
					return prop.intValue;
				case SerializedPropertyType.Character:
					return prop.intValue;
				case SerializedPropertyType.AnimationCurve:
					return prop.animationCurveValue;
				case SerializedPropertyType.Bounds:
					return prop.boundsValue;
				case SerializedPropertyType.Gradient:
					break;
			}
			return null;
		}

		static internal void SetPropertyValue(this SerializedProperty prop, object value)
		{
			if(SetInternalPropertyValue(prop, value))
				return;

			switch (prop.propertyType)
			{
				case SerializedPropertyType.Integer:
					prop.intValue = (int)value;
					break;
				case SerializedPropertyType.Boolean:
					prop.boolValue = (bool)value;
					break;
				case SerializedPropertyType.Float:
					prop.floatValue = (float)value;
					break;
				case SerializedPropertyType.String:
					prop.stringValue = (string)value;
					break;
				case SerializedPropertyType.Color:
					prop.colorValue = (Color)value;
					break;
				case SerializedPropertyType.ObjectReference:
					prop.objectReferenceValue = (Object)value;
					break;
				case SerializedPropertyType.LayerMask:
					prop.intValue = (int)value;
					break;
				case SerializedPropertyType.Enum:
					prop.enumValueIndex = (int)value;
					break;
				case SerializedPropertyType.Vector2:
					prop.vector2Value = (Vector2)value;
					break;
				case SerializedPropertyType.Vector3:
					prop.vector3Value = (Vector3)value;
					break;
				case SerializedPropertyType.Quaternion:
					prop.quaternionValue = (Quaternion)value;
					break;
				case SerializedPropertyType.Rect:
					prop.rectValue = (Rect)value;
					break;
				case SerializedPropertyType.ArraySize:
					prop.intValue = (int)value;
					break;
				case SerializedPropertyType.Character:
					prop.intValue = (int)value;
					break;
				case SerializedPropertyType.AnimationCurve:
					prop.animationCurveValue = (AnimationCurve)value;
					break;
				case SerializedPropertyType.Bounds:
					prop.boundsValue = (Bounds)value;
					break;
				case SerializedPropertyType.Gradient:
					break;
			}
		}

		static private bool SetInternalPropertyValue(this SerializedProperty prop, object value)
		{
			var targetTransform = prop.serializedObject.targetObject as Transform;
			if (targetTransform != null)
			{
				switch (prop.propertyPath)
				{
					case "m_RootOrder":
						targetTransform.SetSiblingIndex((int)value);
						return false;
					case "m_Father":
						SetParentAndSaveLocalTransform(targetTransform, (Transform)value);
						return true;
					default:
						return false;
				}
			}
			return false;
		}

		static internal void CopyPrefabInternalData(Object src, Object dest)
		{
			if (src == null || dest == null)
			{
				if (PEPrefs.DebugLevel > 0)
					Debug.LogError(string.Format("Failed to copy internal prefab data src:{0} dst:{1}", src, dest));
				return;
			}

			var destSO = new SerializedObject(dest);
			var srcSO = new SerializedObject(src);
			destSO.FindProperty("m_PrefabParentObject").SetPropertyValue(srcSO.FindProperty("m_PrefabParentObject").GetPropertyValue());
			destSO.FindProperty("m_PrefabInternal").SetPropertyValue(srcSO.FindProperty("m_PrefabInternal").GetPropertyValue());
			destSO.ApplyModifiedProperties();
		}

		static internal bool Compare(AnimationCurve c0, AnimationCurve c1)
		{
			if (c0 == null && c1 != null || c0 != null && c1 == null)
				return false;

			if (c0 == c1)
				return true;

			if (c0.postWrapMode != c1.postWrapMode)
				return false;

			if (c0.preWrapMode != c1.preWrapMode)
				return false;

			if (c0.keys == null && c1.keys != null)
				return false;

			if (c0.keys != null && c1.keys == null)
				return false;

			if (c0.keys != null && c1.keys != null)
			{
				if (c0.keys.Length != c1.keys.Length)
					return false;

				return !c0.keys.Where((t, i) => !Compare(t, c1.keys[i])).Any();
			}
			return true;
		}

		static internal bool Compare(Keyframe c0, Keyframe c1)
		{
			return c0.inTangent == c1.inTangent &&
			c0.outTangent == c1.outTangent &&
			c0.tangentMode == c1.tangentMode &&
			c0.time == c1.time &&
			c0.value == c1.value;
		}

		static internal Component CopyComponentTo(this Component component, GameObject gameObject)
		{
			if (component == null)
			{
				if (PEPrefs.DebugLevel > 0)
					Debug.Log("Trying to copy null component");

				return null;
			}

			if (PEPrefs.DebugLevel > 0)
				Debug.Log(string.Format("Add component {0} to {1}", component, gameObject));

			var newComponent = gameObject.AddComponent(component.GetType());

			if (!newComponent)
			{
				if (PEPrefs.DebugLevel > 0)
					Debug.LogWarning(string.Format("Failed to copy component of type {0}", component.GetType()));

				return null;
			}

			EditorUtility.CopySerialized(component, newComponent);
			return newComponent;
		}

		#endregion
	}
}

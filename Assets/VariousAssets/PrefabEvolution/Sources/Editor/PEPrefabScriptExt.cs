//#define PREFAB_DEBUG
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace PrefabEvolution
{
	[InitializeOnLoad]
	static class PEPrefabScriptExt
	{
		static PEPrefabScriptExt()
		{
			PEPrefabScript.EditorBridge.OnValidate = OnValidate;
			PEPrefabScript.EditorBridge.GetAssetGuid = (go) => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(go));
			PEPrefabScript.EditorBridge.GetAssetByGuid = (guid) => AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(GameObject)) as GameObject;
		}

		static void OnValidate(this PEPrefabScript _this)
		{
			if (PrefabUtility.GetPrefabType(_this.gameObject) == PrefabType.Prefab)
			{
				EditorApplication.delayCall += () =>
				{
					if (_this == null)
						return;
					if (_this.Prefab != _this.gameObject && _this.transform == _this.transform.root)
					{
						_this.Prefab = _this.gameObject;
						_this.BuildLinks();
					}
				};
			}
		}

		static internal bool IsNonPrefabObject(this PEPrefabScript _this, Object obj)
		{
			return _this.Links[obj] == null;
		}

		static internal IEnumerable<GameObject> GetPrefabsWithInstances(this PEPrefabScript _this)
		{
			return PECache.Instance.GetPrefabsWithInstances(_this.PrefabGUID);
		}

		static internal void RevertToParent(this PEPrefabScript _this)
		{
			_this.Modifications = new PEModifications();
			var thisPrefab = _this.Prefab;
			var thisParentPrefab = _this.ParentPrefab;

			var newInstance = _this.ReplaceInPlace(_this.ParentPrefab, false);
			newInstance.Prefab = thisPrefab;
			newInstance.ParentPrefab = thisParentPrefab;
		}

		static internal void Revert(this PEPrefabScript _this)
		{
			_this.Modifications = new PEModifications();
			_this.ReplaceInPlace(_this.Prefab, false);
		}

		static internal PEPrefabScript ReplaceInPlace(this PEPrefabScript _this, GameObject prefabObject, bool applyModifications = true, bool keepPrefabLink = true)
		{
			var newObject = PrefabUtility.InstantiatePrefab(prefabObject) as GameObject;

			PEUtils.SetParentAndSaveLocalTransform(newObject.transform, _this.transform.parent);
			newObject.transform.SetSiblingIndex(_this.transform.GetSiblingIndex());

			PrefabUtility.DisconnectPrefabInstance(newObject);
			PrefabUtility.DisconnectPrefabInstance(_this.gameObject);

			var newObjectPrefabInstance = newObject.GetComponent<PEPrefabScript>();

			if (newObjectPrefabInstance == null)
				Debug.LogWarning("EvolvePrefab not found on replacing object");

			newObject.transform.localPosition = _this.transform.localPosition;

			if (prefabObject == _this.ParentPrefab || !applyModifications)
			{
				foreach (var link in _this.Links.Links)
				{
					if (newObjectPrefabInstance.Links[link] == null)
						newObjectPrefabInstance.Links.Links.Add(link);
				}
			}

			if (applyModifications && newObjectPrefabInstance)
				_this.ApplyModifications(newObjectPrefabInstance);

			var replaceDict = new Dictionary<Object, Object>();
			replaceDict.Add(_this.gameObject, newObject);
			replaceDict.Add(_this.transform, newObject.transform);

			if (newObjectPrefabInstance)
			{
				foreach (var link in _this.Links.Links)
				{
					var from = link.InstanceTarget;
					var to = newObjectPrefabInstance.Links[link.LIIF];

					if (from == null || to == null)
						continue;

					if (from == _this.gameObject || from == _this.transform)
						continue;

					replaceDict.Add(from, to.InstanceTarget);
				}
			}

			var destroyList = new List<GameObject>();
			foreach (var link in _this.Links.Links)
			{
				if (link == null || link.InstanceTarget is Component)
					continue;

				var go = link.InstanceTarget as GameObject;

				if (!go)
					continue;

				if (_this.Modifications.NonPrefabObjects.Any(m => m.child == go.transform))
					continue;

				destroyList.Add(go);
			}

			PEUtils.ReplaceReference(_this.transform.root, replaceDict);

			var npo = _this.Modifications.NonPrefabObjects.Where(tm => tm != null && tm.child != null && tm.parent != null);

			{
				var indID = 0;
				var indexes = new List<int>(npo.Select(n => n.child.GetSiblingIndex()));
				npo.Foreach(transformModification => PEUtils.SetParentAndSaveLocalTransform(transformModification.child, transformModification.parent));
				npo.Foreach (transformModification => transformModification.child.SetSiblingIndex (indexes [indID++]));
			}
			var reversedComponents = _this.Modifications.NonPrefabComponents.Reverse<PEModifications.ComponentsData> ();
			var newComponents = new List<Component> ();
			foreach (var nonPrefabComponent in reversedComponents)
			{
				Component newComponent;
				newComponents.Add(newComponent = nonPrefabComponent.child.CopyComponentTo(nonPrefabComponent.parent));
				PEUtils.CopyPrefabInternalData(nonPrefabComponent.child, newComponent);
			}

			var i = 0;

			foreach (var nonPrefabComponent in reversedComponents)
			{
				var newComponent = newComponents[i++];
				if (newComponent)
				{
					PEUtils.ReplaceReference (nonPrefabComponent.parent.transform.root, nonPrefabComponent.child, newComponent);
					replaceDict.Add(nonPrefabComponent.child, newComponent);
				}
			}

			if (applyModifications)
			{
				foreach (var transformModification in _this.Modifications.TransformParentChanges)
				{
					var index = transformModification.child.GetSiblingIndex();
					PEUtils.SetParentAndSaveLocalTransform(transformModification.child, transformModification.parent);
					transformModification.child.SetSiblingIndex(index);
				}

				foreach (var id in _this.Modifications.RemovedObjects)
				{
					var link = newObjectPrefabInstance.Links[id];

					if (PEPrefs.DebugLevel > 0 && link != null)
						Debug.Log(string.Format("Object to remove: {0} {1}", id, link.InstanceTarget));

					if (link != null && link.InstanceTarget)
					{
						if (PEPrefs.DebugLevel > 0)
							Debug.Log(string.Format("Destroy Object: {0}", link.InstanceTarget));

						if (link.InstanceTarget == newObject || link.InstanceTarget == newObjectPrefabInstance)
						{
							Debug.LogError("Inconsistent Destroying while replacing");
							continue;
						}

						Object.DestroyImmediate(link.InstanceTarget);
					}
				}
			}

			if (keepPrefabLink)
			{
				newObjectPrefabInstance.ParentPrefab = _this.ParentPrefab;
				newObjectPrefabInstance.Prefab = _this.Prefab;
			}

			foreach (var kv in replaceDict)
			{
				PEUtils.CopyPrefabInternalData(kv.Key, kv.Value);
			}

			newObject.name = _this.gameObject.name;
			Object.DestroyImmediate(_this.gameObject);

			foreach (var go in destroyList)
			{
				if (go == null)
					continue;

				if (go == newObject)
				{
					Debug.LogError("Inconsistend Destroying while replacing");
					continue;
				}

				Object.DestroyImmediate(go);
			}

			if (newObjectPrefabInstance)
			{
				newObjectPrefabInstance.BuildModifications();
				EditorUtility.SetDirty(newObjectPrefabInstance);

				if (prefabObject == _this.ParentPrefab)
					newObjectPrefabInstance.Properties = _this.Properties;

				if (newObjectPrefabInstance.gameObject == null)
					Debug.LogError("New GameObject is destroyed while replacing... o_O");
			}

			try
			{
				newObjectPrefabInstance.gameObject.name = newObjectPrefabInstance.gameObject.name;
			}
			catch (MissingReferenceException)
			{
				Debug.LogError("New EvolvePrefabScript is destroyed while replacing");
			}
			catch (System.NullReferenceException)
			{
			}
	
			return newObjectPrefabInstance;
		}

		static internal void BuildLinks(this PEPrefabScript _this, bool force = false)
		{
			if (force || _this.Prefab == _this.gameObject)
			{
				_this.Links.BuildLinks(_this.gameObject);
				EditorUtility.SetDirty(_this);
			}
			else
			{
				_this.Prefab.GetComponent<PEPrefabScript>().BuildLinks();
				EditorUtility.SetDirty(_this.Prefab.GetComponent<PEPrefabScript>());
			}
		}

		static internal PEPrefabScript GetDiffWith(this PEPrefabScript _this)
		{
			var go = _this.ParentPrefab != null &&
			         (PrefabUtility.GetPrefabParent(_this.gameObject) == _this.Prefab ||
			         PrefabUtility.FindPrefabRoot(_this.gameObject) == _this.Prefab) 
				? _this.ParentPrefab : 
				_this.Prefab;

			return go != null ? go.GetComponent<PEPrefabScript>() : null;
		}

		static internal void BuildModifications(this PEPrefabScript _this)
		{
			var type = PrefabUtility.GetPrefabType(_this.gameObject);

			if (type != PrefabType.ModelPrefab)
			{
				var diff = _this.GetDiffWith();
				if (diff == null)
				{
					Debug.LogError(_this.name + " : " + "Diff Object is not exists");
					return;
				}

				_this.Modifications.CalculateModifications(_this.GetDiffWith(), _this);
			}

			_this.InvokeOnBuildModifications();
		}

		static void ApplyModifications(this PEPrefabScript _this, PEPrefabScript targetInstance)
		{
			targetInstance.Modifications = _this.Modifications;
			var modificatedList = new List<SerializedObject>();

			foreach (var modification in _this.Modifications.Modificated)
			{
				if (modification.Mode == PEModifications.PropertyData.PropertyMode.Ignore)
					continue;

				if (modification.Object == null)
					continue;

				var selfObject = new SerializedObject(modification.Object);

				var linkedObject = targetInstance.Links[modification.ObjeckLink];
				if (linkedObject == null || linkedObject.InstanceTarget == null)
					continue;

				var targetObject = new SerializedObject(linkedObject.InstanceTarget);

				var selfProperty = selfObject.FindProperty(modification.PropertyPath);
				var targetProperty = targetObject.FindProperty(modification.PropertyPath);

				if (targetProperty == null)
				{
					Debug.Log("Property not found " + modification.PropertyPath);
				}
				else
				{
					var value = selfProperty.GetPropertyValue();
					if (selfProperty.propertyType == SerializedPropertyType.ObjectReference)
					{
						var selfValue = selfProperty.GetPropertyValue();

						var selfLink = _this.Links[selfValue as Object];
						var targetLink = targetInstance.Links[selfLink];

						if (targetLink != null)
							value = targetLink.InstanceTarget;
					}
					targetProperty.SetPropertyValue(value);

					targetObject.ApplyModifiedProperties();
					//In some cases unity can destroy prev object, just fix runtime references...
					linkedObject.InstanceTarget = targetObject.targetObject; 
				}
			}

			foreach (var po in modificatedList)
				po.ApplyModifiedProperties();
		}

		static int recursionCounter;

		static internal void ApplyChanges(this PEPrefabScript _this, bool buildModifications = false)
		{
			if(!_this)
				return;

			if (PEPrefs.DebugLevel > 0)
				Debug.Log(string.Format("[Begin Apply] {0}", _this.name));
			
			EditorUtility.DisplayProgressBar("Apply changes", _this.name, 0f);

			if (buildModifications)
				foreach (var pi in _this.GetComponentsInChildren<PEPrefabScript>(true))
					pi.BuildModifications();

			if (recursionCounter++ > 100)
			{
				Debug.LogError("Recursion");
				recursionCounter = 0;
				EditorUtility.ClearProgressBar();
				return;
			}

			_this.BuildLinks(true);
			_this.BuildModifications();

			var newPrefab = !AssetDatabase.Contains(_this.gameObject) ? PrefabUtility.ReplacePrefab(_this.gameObject, _this.Prefab, ReplacePrefabOptions.ConnectToPrefab) : _this.gameObject;

			var prefabs = _this.GetPrefabsWithInstances();

			foreach (var prefab in prefabs)
			{
				if (PEPrefs.DebugLevel > 0)
					Debug.Log("[Apply] Found Nested instance:" + prefab.name);

				var instantiatedPrefabsList = new List<GameObject>();
				var instances = new List<PEPrefabScript>();
				if (prefab == null)
					continue;

				var pi = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
				PrefabUtility.DisconnectPrefabInstance(pi);
				var nestedInstances = PEUtils.GetNestedInstances(pi).Where(p => p.PrefabGUID == _this.PrefabGUID && p.enabled).ToArray();

				instances.AddRange(nestedInstances);

				var rootInstance = pi.GetComponent<PEPrefabScript>();
				if (rootInstance && rootInstance.ParentPrefabGUID == _this.PrefabGUID)
					instances.Insert(0, rootInstance);

				instantiatedPrefabsList.Add(pi);
				var counter = 0;

				foreach (var instance in instances)
				{
					if (instance == null || instance.gameObject == null)
					{
						Debug.LogWarning("[Apply] Huston we have a problem. Prefab is destroyed before replace");
						continue;
					}

					instantiatedPrefabsList.Remove(instance.gameObject);

					var newObject = instance.ReplaceInPlace(newPrefab);

					if (newObject == null || newObject.gameObject == null)
					{
						Debug.LogWarning("[Apply] Huston we have a problem. Prefab is destroyed after replace");
						continue;
					}

					if (newObject.ParentPrefab == newPrefab)
						instantiatedPrefabsList.Add(newObject.gameObject);

					PrefabUtility.RecordPrefabInstancePropertyModifications(newObject);
					EditorUtility.DisplayProgressBar("Apply changes", _this.name + " replaced in " + newObject.gameObject, ((float)counter++) / (float)instances.Count);
				}

				foreach (var instantiatedPrefab in instantiatedPrefabsList)
				{
					if (instantiatedPrefab == null)
						continue;

					var instance = instantiatedPrefab.GetComponent<PEPrefabScript>();

					if (instance)
						instance.ApplyChanges();
					else
						PrefabUtility.ReplacePrefab(instantiatedPrefab, PrefabUtility.GetPrefabParent(instantiatedPrefab), ReplacePrefabOptions.ConnectToPrefab);

					Object.DestroyImmediate(instantiatedPrefab);
				}
			}

			recursionCounter--;

			if (PEPrefs.DebugLevel > 0)
				Debug.Log(string.Format("[End Apply] {0}", _this.name));
		}
	}
}
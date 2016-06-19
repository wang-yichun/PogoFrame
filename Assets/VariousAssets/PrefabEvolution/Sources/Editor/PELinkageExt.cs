using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace PrefabEvolution
{
	static class PELinkageExt
	{
		static private void Add(this PELinkage _this, GameObject prefab, IEnumerable<Object> objs)
		{
			foreach (var obj in objs)
			{
				if (_this[obj] != null)
					continue;
				var pi = prefab.GetComponent<PEPrefabScript>();

				if (pi && AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(pi.PrefabGUID)) is ModelImporter)
				{
					var component = obj as Component;
					var go = obj as GameObject;

					var path = "";

					var targetGO = component ? component.gameObject : go;
					path = targetGO.name;

					if (targetGO.transform.root == targetGO.transform)
						path = "#ROOT#";

					if (component)
						path = string.Format("{0}({1}#{2})", path, component.GetType().Name, component.gameObject.GetComponents<Component>().Where(c => c.GetType() == component.GetType()).ToList().IndexOf(component));
					else
						path = string.Format("{0}({1})", path, go.GetType().Name);

					_this.Links.Add(new PELinkage.Link { InstanceTarget = obj, LIIF = path.GetHashCode() });
				}
				else
				{
					_this.Links.Add(new PELinkage.Link { InstanceTarget = obj, LIIF = System.Guid.NewGuid().GetHashCode() });
				}
			}
		}

		static internal void BuildLinks(this PELinkage _this, GameObject prefab)
		{
			var objects = EditorUtility.CollectDeepHierarchy(new Object[] { prefab });

			for (var i = 0; i < _this.Links.Count; i++)
			{
				var link = _this.Links[i];
				if (objects.Any(obj => obj == link.InstanceTarget))
					continue;

				_this.Links.RemoveAt(i);
				i--;
			}

			_this.Add(prefab, objects);
		}
	}
}
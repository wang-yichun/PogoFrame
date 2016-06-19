#if !UNITY_METRO && !UNITY_WP_8_1 && !UNITY_WINRT && !UNITY_WINRT_8_1
#define ENABLE_REFLECTION
using System.Reflection;
#endif

using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using Debug = UnityEngine.Debug;

namespace PrefabEvolution
{
	[System.Serializable]
	public class PEExposedProperties : ISerializationCallbackReceiver
	{
		[System.NonSerialized] internal List<BaseExposedData> InheritedProperties;
		[System.NonSerialized] public PEPrefabScript PrefabScript;

		public List<ExposedProperty> Properties = Utils.Create<List<ExposedProperty>>();
		public List<ExposedPropertyGroup> Groups = Utils.Create<List<ExposedPropertyGroup>>();

		[SerializeField] private List<int> Hidden = Utils.Create<List<int>>();

		#region ISerializationCallbackReceiver implementation

		public void OnBeforeSerialize()
		{

		}

		public void OnAfterDeserialize()
		{
#if UNITY_EDITOR
			InheritedProperties = null;
			foreach (var item in Properties.OfType<BaseExposedData>().Concat(Groups.OfType<BaseExposedData>()))
			{
				item.Container = this;
			}
#endif
		}

		#endregion

		public IEnumerable<BaseExposedData> GetInheritedProperties()
		{
			if (InheritedProperties == null)
			{
				InheritedProperties = new List<BaseExposedData>();
				if (this.PrefabScript == null)
					return InheritedProperties;
				if (this.PrefabScript.ParentPrefab != null)
				{
					var parentScript = PrefabScript.ParentPrefab.GetComponent<PEPrefabScript>();
					if (parentScript == null)
					{
						return InheritedProperties;
					}

					InheritedProperties.AddRange(parentScript.Properties.Items.Where(i => !i.Hidden).Select(p =>
					{
						var r = p.Clone();
						r.Container = this;
						return r;
					}));

					this.Properties.RemoveAll(p => p.Inherited);
					this.Groups.RemoveAll(p => p.Inherited);
					this.Hidden.RemoveAll(p => this.Items.All(item => item.Id != p));
					foreach (var property in InheritedProperties.OfType<ExposedProperty>())
					{
						var link = PrefabScript.Links[parentScript.Links[property.Target]];
						property.Target = link == null ? null : link.InstanceTarget;
						if (property.Target == null)
							Debug.Log("Inherited property Error: Local target is not found Path:" + property.PropertyPath, this.PrefabScript);
					}
				}
			}
			return InheritedProperties;
		}

		public void Add(BaseExposedData exposed)
		{
			exposed.Container = this;
			var exposedProperty = exposed as ExposedProperty;
			if (exposedProperty != null)
				Add(exposedProperty);
			else
				Add(exposed as ExposedPropertyGroup);
		}

		public void Add(ExposedProperty exposed)
		{
			exposed.Container = this;
			if (!Properties.Contains(exposed))
				Properties.Add(exposed);
		}

		public void Add(ExposedPropertyGroup exposed)
		{
			exposed.Container = this;
			if (!Groups.Contains(exposed))
				Groups.Add(exposed);
		}

		public void Remove(int id)
		{
			Properties.RemoveAll(p => p.Id == id);
			Groups.RemoveAll(p => p.Id == id);
		}

		public ExposedProperty FindProperty(string label)
		{
			return Items.OfType<ExposedProperty>().FirstOrDefault(p => p.Label == label);
		}

		public ExposedProperty FindProperty(int id)
		{
			var result = Items.OfType<ExposedProperty>().FirstOrDefault(p => p.Id == id);
			return result;
		}

		public ExposedProperty FindProperty(uint id)
		{
			return Items.OfType<ExposedProperty>().FirstOrDefault(p => p.Id == (int)id);
		}

		public BaseExposedData this[int id]
		{
			get
			{
				return Items.FirstOrDefault(p => p.Id == id);
			}
		}

		public BaseExposedData this[string label]
		{
			get
			{
				return OrderedItems.FirstOrDefault(p => p.Label == label);
			}
		}

		public IEnumerable<BaseExposedData> Items
		{
			get
			{
				return GetInheritedProperties().Concat(Properties.OfType<BaseExposedData>().Concat(Groups.OfType <BaseExposedData>()));
			}
		}

		public IEnumerable<BaseExposedData> OrderedItems
		{
			get
			{
				var comparer = new BaseExposedData.Comparer();
				var list = Items.ToList();
				list.Sort(comparer);
				return list;
			}
		}

		public bool GetInherited(int id)
		{
			return GetInheritedProperties().Any(i => i.Id == id);
		}

		public bool GetHidden(int id)
		{
			return Hidden.Any(i => i == id);
		}

		public void SetHide(BaseExposedData property, bool state)
		{
			if (state == Hidden.Contains(property.Id))
				return;

			if (state)
				Hidden.Add(property.Id);
			else
				Hidden.Remove(property.Id);

			Hidden.Sort();
		}
	}

	public class BaseExposedData : ISerializationCallbackReceiver
	{
		[System.NonSerialized]
		public PEExposedProperties Container;

		[SerializeField]
		private int guid = System.Guid.NewGuid().GetHashCode();
		public string Label;
		public int ParentId;
		public float Order;

		#region ISerializationCallbackReceiver implementation

		public virtual void OnBeforeSerialize()
		{

		}

		public virtual void OnAfterDeserialize()
		{

		}

		#endregion

		public int SiblingIndex
		{
			get
			{
				return this.Brothers.ToList().IndexOf(this);
			}
		}

		public float GetOrder(bool next)
		{
			var index = next ? SiblingIndex + 1 : SiblingIndex - 1;
			var nextBrother = this.Brothers.ElementAtOrDefault(index);
			return nextBrother == null ? this.Order + (next ? 1 : -1) : (this.Order + nextBrother.Order) * 0.5f;
		}

		public virtual BaseExposedData Clone()
		{
			var copy = System.Activator.CreateInstance(this.GetType()) as BaseExposedData;
			copy.ParentId = this.ParentId;
			copy.Label = this.Label;
			copy.guid = this.guid;
			copy.Order = this.Order;
			return copy;
		}

		public int Id
		{
			get
			{
				return guid;
			}
		}

		public BaseExposedData Parent
		{
			get
			{
				return this.Container[ParentId];
			}
			set
			{
				this.ParentId = value.Id;
			}
		}

		public IEnumerable<BaseExposedData> Children
		{
			get
			{
				return this.Container.OrderedItems.Where(item => item.ParentId == this.Id);
			}
		}

		public IEnumerable<BaseExposedData> Brothers
		{
			get
			{
				var parent = this.Parent;
				return Container.OrderedItems.Where(i => i.Parent == parent);
			}
		}

		public bool Inherited
		{
			get
			{
				return Container.GetInherited(this.Id);
			}
		}

		public bool Hidden
		{
			get
			{
				return Container.GetHidden(Id);
			}
			set
			{
				Container.SetHide(this, value);
			}
		}

		public struct Comparer : IComparer<BaseExposedData>
		{
			public int Compare(BaseExposedData x, BaseExposedData y)
			{
				return (int)Mathf.Sign(x.Order - y.Order);
			}
		}
	}

	[System.Serializable]
	public class ExposedPropertyGroup : BaseExposedData
	{
		static public Dictionary<int, bool> expandedDict = new Dictionary<int, bool>();
		private bool expandedLoaded;
		private bool expanded = true;

		public bool Expanded
		{
			get
			{
				if (!expandedLoaded)
				{
					expandedDict.TryGetValue(this.Id, out expanded);
					expandedLoaded = true;
				}
				return expanded;
			}

			set
			{
				if (value == expanded)
					return;

				if (!expandedDict.ContainsKey(this.Id))
					expandedDict.Add(this.Id, true);
				expandedDict[this.Id] = value;

				this.expanded = value;
			}
		}
	}

	[System.Serializable]
	public class ExposedProperty : BaseExposedData
	{
		public Object Target;
		public string PropertyPath;

		public override BaseExposedData Clone()
		{
			var copy = (ExposedProperty)(base.Clone());
			copy.Target = this.Target;
			copy.PropertyPath = this.PropertyPath;
			return copy;
		}
#if ENABLE_REFLECTION
		public override void OnAfterDeserialize()
		{
			base.OnAfterDeserialize();
			_invocationChain = null;
		}

		private PropertyInvocationChain _invocationChain;

		private PropertyInvocationChain invocationChain
		{
			get
			{
				if (_invocationChain == null)
				{
					_invocationChain = new PropertyInvocationChain(this.Target, this.PropertyPath);
					try
					{
						this.Value = this.Value;
					}
					catch (System.Exception e)
					{
						Debug.LogException(e);
						_invocationChain.members = null;
					}
				}

				return _invocationChain;
			}
		}

		public bool IsValid
		{
			get
			{
				return invocationChain.isValid;
			}
		}

		public object Value
		{
			get
			{
				if (!IsValid)
				{
					Debug.LogWarning("Trying to get value from invalid prefab property. Target:" + this.Target + " Property path:" + this.PropertyPath);
					return null;
				}
				return invocationChain.value;
			}
			set
			{
				if (!IsValid)
				{
					Debug.LogWarning("Trying to set value to invalid prefab property. [Target:" + this.Target + ", Property path:" + this.PropertyPath + "]");
					return;
				}

				invocationChain.value = value;
			}
#endif
	}
#if ENABLE_REFLECTION
		public class PropertyInvocationChain
		{
			public object root;
			public string path;
			public InvokeInfo[] members;

			public PropertyInvocationChain(object root, string path)
			{
				this.root = root;
				this.path = path;
				GetInstance(root, path, out members);
			}

			public object value
			{
				get
				{
					var v = root;
					GetValidFieldName(ref v, this.path);
					for (var i = 0; i < members.Length; i++)
					{
						v = members[i].GetValue(v);
					}
					return v;
				}
				set
				{
					var v = root;
					GetValidFieldName(ref v, this.path);
					for (var i = 0; i < members.Length - 1; i++)
					{
						v = members[i].GetValue(v);
					}
					members[members.Length - 1].SetValue(v, value);

					for (var i = members.Length - 2; i >= 0; i--)
					{
						var member = members[i];
						var nextMember = members[i + 1];
						if (member.member.MemberType == MemberTypes.Property || member.valueType.IsValueType || nextMember.valueType.IsValueType)
						{
							member.SetValue(nextMember.tempTarget);
						}
					}

				}
			}

			public bool isValid
			{
				get
				{
					return members != null;
				}
			}

			public class InvokeInfo
			{
				public MemberInfo member;
				public int index = -1;
				public object tempTarget;
				public System.Type valueType;

				public object GetValue(object target)
				{
					tempTarget = target;
					return GetMemberValue(target, member, index);
				}

				public void SetValue(object target, object value)
				{
					tempTarget = target;
					setValue(target, member, value, index);
				}

				public void SetValue(object value)
				{
					setValue(tempTarget, member, value, index);
				}
			}

			static internal object GetInstance(object obj, string path, out InvokeInfo[] members)
			{
				path = path.Replace(".Array.data", "");
				var split = path.Split('.');

				var stack = split;
				object v = obj;
				members = new InvokeInfo[stack.Length];
				try
				{
					var i = 0;
					foreach (var name in stack)
					{
						members[i] = new InvokeInfo();
						if (name.Contains("["))
						{
							var n = name.Split('[', ']');
							var index = int.Parse(n[1]);
							members[i].index = index;
							v = getField(v, n[0], out members[i].member, index);

						}
						else
							v = getField(v, name, out members[i].member);

						var propertyMember = members[i].member as PropertyInfo;
						var fieldMember = members[i].member as FieldInfo;

						if (fieldMember != null)
							members[i].valueType = fieldMember.FieldType;
						else
						if (propertyMember != null)
							members[i].valueType = propertyMember.PropertyType;
						i++;
					}
				}
				catch (System.Exception e)
				{
					members = null;
					Debug.LogException(e);
					return null;
				}

				return v;
			}

			private static object GetMemberValue(object target, MemberInfo member, int index = -1)
			{
				object result = null;

				var fieldInfo = member as FieldInfo;
				if (fieldInfo != null)
					result = fieldInfo.GetValue(target);

				var propertyInfo = member as PropertyInfo;
				if (propertyInfo != null)
					result = propertyInfo.GetValue(target, null);

				return result != null ? index == -1 ? result : (result as IList)[index] : null;
			}

			private static void setValue(object target, MemberInfo member, object value, int index = -1)
			{
				if (index != -1)
				{
					target = GetMemberValue(target, member);
					(target as IList)[index] = value;
					return;
				}

				var fieldInfo = member as FieldInfo;
				if (fieldInfo != null)
					fieldInfo.SetValue(target, value);

				var propertyInfo = member as PropertyInfo;
				if (propertyInfo != null)
					propertyInfo.SetValue(target, value, null);
			}

			static public string GetValidFieldName(ref object obj, string fieldName)
			{
				if (obj is Renderer && fieldName == "m_Materials")
					return "sharedMaterials";
				if (obj is MeshFilter && fieldName == "m_Mesh")
					return "sharedMesh";
				if (obj is GameObject && fieldName == "m_IsActive")
					obj = new GameObjectWrapper(obj as GameObject);

				return fieldName;
			}

			private static object getField(object obj, string field, out MemberInfo member, int index = -1)
			{
				member = null;
				try
				{
					var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;
					if (obj == null)
						return null;
					field = GetValidFieldName(ref obj, field);
					var type = obj.GetType();

					member = type.GetField(field, flags);
					if (member == null && field.StartsWith("m_"))
						member = type.GetField(field.Remove(0, 2), flags);

					member = type.GetProperty(field, flags);
					if (member == null && field.StartsWith("m_"))
						member = type.GetProperty(field.Remove(0, 2), flags);

					if (member == null)
						member = type.GetMembers(flags).First(m => m.Name.ToUpper() == field.ToUpper());

					if (member != null)
						return GetMemberValue(obj, member, index);
						
					member = null;
					return null;
				}
				catch (System.Exception)
				{
					member = null;
					return null;
				}
			}
		}
	}
#endif

	#region Wrappers
	static public class Wrappers
	{

	}

	public class GameObjectWrapper
	{
		public GameObject target;
		public bool m_IsActive
		{
			get
			{
				return target.activeSelf;
			}
			set
			{
				target.SetActive(value);
			}
		}

		public GameObjectWrapper(GameObject target)
		{
			this.target = target;
		}
	}
	#endregion
}


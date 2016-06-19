using UnityEditor;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace PrefabEvolution
{
	static public class PEPropertyHelper
	{
		public static void CopyPropertyValue(SerializedProperty source, SerializedProperty dest)
		{
			source = source.Copy();
			dest = dest.Copy();

			var destSO = dest.serializedObject;


			if (source.hasVisibleChildren)
			{
				var sourceRoot = source.propertyPath;
				var destRoot = dest.propertyPath;

				while (source.NextVisible(true) && source.propertyPath.StartsWith(sourceRoot))
				{
					if (source.propertyType == SerializedPropertyType.Generic)
						continue;

					var path = destRoot + source.propertyPath.Remove(0, sourceRoot.Length);
					var destProperty = destSO.FindProperty(path);
					SetPropertyValue(destProperty, GetPropertyValue(source));
				}
			}
			else
			{
				if (dest.propertyPath == "m_RootOrder")
				{
					(dest.serializedObject.targetObject as Transform).SetSiblingIndex((int)GetPropertyValue(source));
				}
				SetPropertyValue(dest, GetPropertyValue(source));
			}
		}

		public static FieldInfo GetFieldInfoFromProperty(SerializedProperty property)
		{
			System.Type type;
			return GetFieldInfoFromProperty(property, out type);
		}

		public static FieldInfo GetFieldInfoFromProperty(SerializedProperty property, out System.Type type)
		{
			var scriptTypeFromProperty = GetScriptTypeFromProperty(property);
			if (scriptTypeFromProperty == null)
			{
				type = null;
				return null;
			}
			return GetFieldInfoFromPropertyPath(scriptTypeFromProperty, property.propertyPath, out type);
		}

		private static System.Type GetScriptTypeFromProperty(SerializedProperty property)
		{
			SerializedProperty serializedProperty = property.serializedObject.FindProperty("m_Script");
			if (serializedProperty == null)
			{
				return null;
			}
			MonoScript monoScript = serializedProperty.objectReferenceValue as MonoScript;
			if (monoScript == null)
			{
				return null;
			}
			return monoScript.GetClass();
		}

		public static bool IsArrayOrList(System.Type listType)
		{
			return listType.IsArray || (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>));
		}

		private static System.Type GetArrayOrListElementType(System.Type listType)
		{
			if (listType.IsArray)
			{
				return listType.GetElementType();
			}
			if (listType.IsGenericType && listType.GetGenericTypeDefinition() == typeof(List<>))
			{
				return listType.GetGenericArguments()[0];
			}
			return null;
		}

		private static FieldInfo GetFieldInfoFromPropertyPath(System.Type host, string path, out System.Type type)
		{
			FieldInfo fieldInfo = null;
			type = host;
			string[] array = path.Split(new char[] {
				'.'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (i < array.Length - 1 && text == "Array" && array[i + 1].StartsWith("data["))
				{
					if (IsArrayOrList(type))
					{
						type = GetArrayOrListElementType(type);
					}
					i++;
				}
				else
				{
					FieldInfo fieldInfo2 = null;
					System.Type type2 = type;
					while (fieldInfo2 == null && type2 != null)
					{
						fieldInfo2 = type2.GetField(text, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
						type2 = type2.BaseType;
					}
					if (fieldInfo2 == null)
					{
						type = null;
						return null;
					}
					fieldInfo = fieldInfo2;
					type = fieldInfo.FieldType;
				}
			}
			return fieldInfo;
		}

		static public object GetPropertyValue(SerializedProperty prop)
		{
			if (prop == null)
				return null;
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

		static public void SetPropertyValue(SerializedProperty prop, object value)
		{
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

		static public GUIContent[] v2Labels = new []{new GUIContent("X"), new GUIContent("Y")};
		static public GUIContent[] v3Labels = new []{new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z")};

		static public bool PropertyFieldLayout(SerializedProperty property, GUIContent label, bool includeChildren, params GUILayoutOption[] options)
		{
			var position = EditorGUILayout.GetControlRect(LabelHasContent(label), EditorGUI.GetPropertyHeight(property, label, includeChildren), options);
			position.height -= 2;
			if (property.propertyType == SerializedPropertyType.Vector2 || property.propertyType == SerializedPropertyType.Vector3)
				position = EditorGUI.IndentedRect(position);
			return PropertyField(position, property, label, includeChildren);
		}

		static public bool PropertyField(Rect position, SerializedProperty property, GUIContent label, bool includeChildren)
		{
			//if (!includeChildren)
			{
				switch (property.propertyType)
				{
					case SerializedPropertyType.Vector2:
					case SerializedPropertyType.Vector3:
						property = property.Copy();
						var labels = property.propertyType == SerializedPropertyType.Vector2 ? v2Labels : v3Labels;
						if (label == null)
							label = new GUIContent(GetDisplayName(property));

						property.Next(true);

						MultiPropertyField(position, labels, property, label);
						return false;
				}
			}

			return EditorGUI.PropertyField(position, property, label, includeChildren);
		}

		internal static bool LabelHasContent(GUIContent label)
		{
			return label == null || label.text != string.Empty || label.image != null;
		}

		internal static float indent
		{
			get
			{
				return (float)EditorGUI.indentLevel * 15;
			}
		}

		internal static Rect MultiFieldPrefixLabel(Rect totalPosition, int id, GUIContent label, int columns)
		{
			if (!LabelHasContent(label))
			{
				return EditorGUI.IndentedRect(totalPosition);
			}

			if (EditorGUIUtility.wideMode)
			{
				Rect labelPosition = new Rect(totalPosition.x + indent, totalPosition.y, EditorGUIUtility.labelWidth - indent, 16);
				Rect result = totalPosition;
				result.xMin += EditorGUIUtility.labelWidth;
				if (columns > 1)
				{
					labelPosition.width -= 1;
					result.xMin -= 1;
				}
				if (columns == 2)
				{
					float num = (result.width - 4) / 3;
					result.xMax -= num + 2;
				}
				EditorGUI.HandlePrefixLabel(totalPosition, labelPosition, label, id);
				return result;
			}
			Rect labelPosition2 = new Rect(totalPosition.x + EditorGUI.indentLevel, totalPosition.y, totalPosition.width - indent, 16);
			Rect result2 = totalPosition;
			result2.xMin += EditorGUI.indentLevel + 15;
			result2.yMin += 16;
			EditorGUI.HandlePrefixLabel(totalPosition, labelPosition2, label, id);
			return result2;
		}

		private static int s_FoldoutHash = "Foldout".GetHashCode();
		public static void MultiPropertyField(Rect position, GUIContent[] subLabels, SerializedProperty valuesIterator, GUIContent label)
		{
			int controlID = GUIUtility.GetControlID(s_FoldoutHash, EditorGUIUtility.native, position);
			position = MultiFieldPrefixLabel(position, controlID, label, subLabels.Length);
			position.height = 16;
			MultiPropertyField(position, subLabels, valuesIterator);
		}

		internal static void MultiPropertyField(Rect position, GUIContent[] subLabels, SerializedProperty valuesIterator, float labelWidth = 13)
		{
			int num = subLabels.Length;
			float num2 = (position.width - (float)(num - 1) * 2) / (float)num;
			Rect position2 = new Rect(position);
			position2.width = num2;
			float labelWidth2 = EditorGUIUtility.labelWidth;
			int indentLevel = EditorGUI.indentLevel;
			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUI.indentLevel = 0;
			for (int i = 0; i < subLabels.Length; i++)
			{
				EditorGUI.PropertyField(position2, valuesIterator, subLabels[i]);
				position2.x += num2 + 2;
				valuesIterator.Next(false);
			}
			EditorGUIUtility.labelWidth = labelWidth2;
			EditorGUI.indentLevel = indentLevel;
		}

		static internal string GetDisplayName(this SerializedProperty prop)
		{
			const BindingFlags Flags = BindingFlags.Default | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance;
			return prop.GetType().GetProperty("displayName", Flags).GetValue(prop, new object[0]) as string;
		}

		static internal string GetNonPrettyName(this SerializedProperty prop)
		{
			return prop.propertyPath.Split('.').Last();
		}

		#region TransformUtils

		static public void Rebase(Transform transform, Transform to)
		{
			var transformSO = new SerializedObject(transform);
			var toSO = new SerializedObject(to);

			var transformPrefabLink = transformSO.FindProperty("m_PrefabInternal").objectReferenceValue;
			var toPrefabLink = toSO.FindProperty("m_PrefabInternal").objectReferenceValue;

			toSO.FindProperty("m_PrefabInternal").objectReferenceValue = null;
			transformSO.FindProperty("m_PrefabInternal").objectReferenceValue = null;

			transformSO.ApplyModifiedProperties();
			toSO.ApplyModifiedProperties();

			var index = transform.GetSiblingIndex();
			var lp = transform.localPosition;
			var lr = transform.localRotation;
			var ls = transform.localScale;
			transform.parent = to;
			transform.localPosition = lp;
			transform.localRotation = lr;
			transform.localScale = ls;
			transform.SetSiblingIndex(index);

			transformSO.Update();
			toSO.Update();

			transformSO.FindProperty("m_PrefabInternal").objectReferenceValue = transformPrefabLink;
			toSO.FindProperty("m_PrefabInternal").objectReferenceValue = toPrefabLink;

			transformSO.ApplyModifiedProperties();
			toSO.ApplyModifiedProperties();
		}

		#endregion
	}
}
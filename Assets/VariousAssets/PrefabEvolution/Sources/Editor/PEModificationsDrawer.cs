using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace PrefabEvolution
{
	[CustomPropertyDrawer(typeof(PEModifications.PropertyData))]
	class PropertyDataDrawer : PropertyDrawer
	{
		static KeyValuePair<GUIContent, System.Action>[] GetButtonsData(PEPrefabScript prefabInstance, SerializedProperty prefabProperty, SerializedProperty instanceProperty)
		{
			var buttons = new KeyValuePair<GUIContent, System.Action>[] {
				new KeyValuePair<GUIContent, System.Action>(new GUIContent("Revert", "Revert property to prefab value"), () => 
				{
					if (prefabProperty == null)
						return;
					if (instanceProperty.propertyType == SerializedPropertyType.ObjectReference)
					{
						var link = prefabInstance.GetDiffWith().Links[prefabProperty.objectReferenceValue];
						if (link == null)
							instanceProperty.SetPropertyValue(prefabProperty.GetPropertyValue());
						else
						{
							var instanceLink = prefabInstance.Links[link];
							if (instanceLink != null)
								instanceProperty.SetPropertyValue(prefabInstance.Links[link].InstanceTarget);
							else
							{
								if (PEPrefs.DebugLevel > 0)
									Debug.Log("Link null");
								instanceProperty.SetPropertyValue(prefabProperty.GetPropertyValue());
							}
						}
					}
					else
					{
						instanceProperty.SetPropertyValue(prefabProperty.GetPropertyValue());
					}
					instanceProperty.serializedObject.ApplyModifiedProperties();
				}),
				new KeyValuePair<GUIContent, System.Action>(new GUIContent("Update", "Update changes"), () => EditorApplication.delayCall += prefabInstance.BuildModifications),
			};
			return buttons;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var intent = EditorGUI.indentLevel;
			position = EditorGUI.IndentedRect(position);
			EditorGUI.indentLevel = 0;
			EditorGUI.BeginChangeCheck();

			var prefabInstance = property.serializedObject.targetObject as PEPrefabScript;
			var modificationInstance = property.GetInstance<PEModifications.PropertyData>();

			SerializedProperty prefabProperty;
			SerializedProperty instanceProperty;
			modificationInstance.GetProperties(out prefabProperty, out instanceProperty, prefabInstance);
			if (instanceProperty == null)
				return;
		
			var mode = property.FindPropertyRelative("Mode");

			var color = GUI.color;
			GUI.color = mode.enumValueIndex == 0 ? new Color(1, 1, 1, 1) : (mode.enumValueIndex == 1 ? new Color(0, 1, 0, 1) : new Color(1, 0, 0, 1));
			position.x -= 6;
			position.width += 6;
			GUI.Box(position, "", "Window");

			position.x += 12;
			position.width -= 18;

			GUI.color = color;

			var enabled = GUI.enabled;
			GUI.enabled = false;

			var height = EditorGUI.GetPropertyHeight(instanceProperty);
			position.y += EditorGUIUtility.singleLineHeight + 3;
			if (prefabProperty != null)
				EditorGUI.PropertyField(new Rect(position) { height = height }, prefabProperty, true);
			else
				EditorGUI.LabelField(new Rect(position) { height = height }, "Property not exist");

			position.y += height + 2;
			GUI.enabled = enabled;
		
			EditorGUI.PropertyField(new Rect(position) { height = height }, instanceProperty, true); 
			position.y += height;

			var buttons = GetButtonsData(prefabInstance, prefabProperty, instanceProperty);
			position.y += 3;
			var startRect = new Rect(position) {
				height = 15,
				width = position.width / (buttons.Length + 1),
			};

			var lw = EditorGUIUtility.labelWidth;
			EditorGUIUtility.labelWidth = 0;
			EditorGUIUtility.fieldWidth = 0;
			EditorGUI.PropertyField(new Rect(startRect) { width = startRect.width - 6 }, mode, new GUIContent("", "Select proprty mode"), true);
			EditorGUIUtility.labelWidth = lw;

			startRect.x += startRect.width;
			var i = 0;
			foreach (var button in buttons)
			{
				var style = i == 0 ? EditorStyles.miniButtonLeft : (i == buttons.Length - 1 ? EditorStyles.miniButtonRight : EditorStyles.miniButtonMid);
				if (GUI.Button(startRect, button.Key, style))
					button.Value();
				startRect.x += startRect.width;
				i++;
			}
			startRect.x = position.x;
			startRect.y += startRect.height;

			if (EditorGUI.EndChangeCheck())
			{
				instanceProperty.serializedObject.ApplyModifiedProperties();
				if (prefabProperty != null)
					prefabProperty.serializedObject.ApplyModifiedProperties();
			}
			EditorGUI.indentLevel = intent;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var prefabInstance = property.serializedObject.targetObject as PEPrefabScript;
			var propertyPath = property.FindPropertyRelative("PropertyPath").stringValue;
			var linkId = property.FindPropertyRelative("ObjeckLink").intValue;
			var instanceObject = prefabInstance.Links[linkId];
			var instanceProperty = new SerializedObject(instanceObject.InstanceTarget).FindProperty(propertyPath);
			if (instanceProperty == null)
				return 0;

			return (instanceProperty == null ? EditorGUIUtility.singleLineHeight : EditorGUI.GetPropertyHeight(instanceProperty)) * 2 + EditorGUIUtility.singleLineHeight * 2 + 11;
		}
	}
}


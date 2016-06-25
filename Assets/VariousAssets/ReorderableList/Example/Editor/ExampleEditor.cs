using UnityEngine;
using UnityEditor;
using System.Collections;
using Malee.Editor;

[CanEditMultipleObjects]
[CustomEditor(typeof(Example))]
public class ExampleEditor : Editor {
	
	private ReorderableList list1;
	private SerializedProperty list2;

	void OnEnable() {

		list1 = new ReorderableList(serializedObject.FindProperty("list1"));
		list2 = serializedObject.FindProperty("list2");
	}

	public override void OnInspectorGUI() {

		serializedObject.Update();

		//draw the list using GUILayout, you can of course specify your own position and label
		list1.DoLayoutList();

		//Caching the property is recommended
		EditorGUILayout.PropertyField(list2);

		//Still works, but there are some minor issues with selection state as properties can only be referenced by propertyPath
		//And if that propertyPath changes (by array modification) then the list will be either rebuilt or point to a different reference
		//EditorGUILayout.PropertyField(serializedObject.FindProperty("list2"));

		serializedObject.ApplyModifiedProperties();
	}
}

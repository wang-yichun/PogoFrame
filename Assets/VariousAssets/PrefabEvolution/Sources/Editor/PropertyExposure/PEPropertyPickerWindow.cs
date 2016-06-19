using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using PrefabEvolution;

namespace PrefabEvolution
{
	class PEPropertyPickerWindow : EditorWindow
	{
		[SerializeField]
		private List<int> expandList = new List<int>();

		private bool this[Object obj]
		{
			get
			{
				var key = obj.GetInstanceID();
				return expandList.Contains(key);
			}
			set
			{
				var key = obj.GetInstanceID();
				if (this[obj] == value)
					return;

				if (value)
					expandList.Add(key);
				else
					expandList.Remove(key);
			}
		}

		private GUIStyle plusButtonStyle = new GUIStyle("OL Plus");
		private GUIContent gameObjectContent = new GUIContent(EditorGUIUtility.ObjectContent(null, typeof(GameObject)));

		static public SerializedProperty labelProperty;
		static public SerializedProperty targetProperty;

		static public Object root;
		static public bool pickObject;

		static public bool showHiddenProperties;

		public delegate void OnPropertyPickerDelegate(SerializedProperty property) ;
		static public OnPropertyPickerDelegate OnPropertyPicked;

		public delegate void OnObjectPickerDelegate(Object obj) ;
		static public OnObjectPickerDelegate OnObjectPicked;

		public static void Show(Object root, OnPropertyPickerDelegate onPicked, Rect r, SerializedProperty labelProperty = null, SerializedProperty targetProperty = null)
		{
			PEPropertyPickerWindow.root = root;
			PEPropertyPickerWindow.OnPropertyPicked = onPicked;
			PEPropertyPickerWindow.labelProperty = labelProperty;
			PEPropertyPickerWindow.pickObject = false;
			PEPropertyPickerWindow.targetProperty = targetProperty;

			var picker = ScriptableObject.CreateInstance<PEPropertyPickerWindow>();

			picker.ShowAsDropDown(r, new Vector2(350, 500));
			if (PEPropertyPickerWindow.targetProperty != null)
			{
				picker.Expand(targetProperty.serializedObject.targetObject);
			}
			else
				picker.Expand(root);
		}

		public void Expand(Object obj)
		{
			if (obj == null)
				return;

			this[obj] = true;

			var gameObject = obj as GameObject;
			if (gameObject != null)
			{
				if (gameObject.transform.parent)
					Expand(gameObject.transform.parent.gameObject);
			}

			var component = obj as Component;

			if (component != null)
				Expand(component.gameObject);
		}

		public static void Show(Object root, OnObjectPickerDelegate onPicked, Rect r)
		{
			PEPropertyPickerWindow.root = root;
			PEPropertyPickerWindow.OnObjectPicked = onPicked;
			PEPropertyPickerWindow.labelProperty = null;
			PEPropertyPickerWindow.pickObject = true;

			var picker = ScriptableObject.CreateInstance<PEPropertyPickerWindow>();

			picker.ShowAsDropDown(r, new Vector2(350, 500));
		}

		private static bool CheckChild(SerializedProperty property)
		{
			return property.propertyType != SerializedPropertyType.String &&
			property.propertyType != SerializedPropertyType.ObjectReference &&
			property.propertyType != SerializedPropertyType.Vector2 &&
			property.propertyType != SerializedPropertyType.Vector3 &&
			property.propertyType != SerializedPropertyType.Vector4 &&
			property.propertyType != SerializedPropertyType.Quaternion &&
			property.propertyType != SerializedPropertyType.Rect &&
			property.propertyType != SerializedPropertyType.Color;
		}

		Vector2 scroll;
		void OnGUI()
		{
			if (labelProperty != null)
			{
				EditorGUI.BeginChangeCheck();

				EditorGUILayout.PropertyField(labelProperty);

				if (EditorGUI.EndChangeCheck())
					labelProperty.serializedObject.ApplyModifiedProperties();
			}

			scroll = GUILayout.BeginScrollView(scroll);

			DrawObject(root);

			GUILayout.EndScrollView();
			if (GUILayout.Button("Toggle Hidden Properties"))
				showHiddenProperties = !showHiddenProperties;
		}

		void OnPick(SerializedProperty property, bool close = true)
		{
			targetProperty = property;

			if (close)
				this.Close();

			if (OnPropertyPicked != null)
				OnPropertyPicked(property);
		}

		void OnPick(Object obj)
		{
			if (OnObjectPicked != null)
				OnObjectPicked(obj);

			this.Close();
		}

		void DrawObject(Object obj)
		{
			var content = EditorGUIUtility.ObjectContent(obj, obj.GetType());

			if (obj is GameObject)
				content.image = gameObjectContent.image;

			if (obj is Component)
			{
				content.text = obj.GetType().Name;
			}

			if (!pickObject)
			{
				this[obj] = EditorGUILayout.Foldout(this[obj], content);
			}
			else
			{
				EditorGUILayout.BeginHorizontal();
				this[obj] = EditorGUILayout.Foldout(this[obj], content);

				if (GUILayout.Button(GUIContent.none, this.plusButtonStyle, GUILayout.Width(30), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
					OnPick(obj);

				EditorGUILayout.EndHorizontal();
			}

			if (!this[obj])
				return;

			EditorGUI.indentLevel++;

			var gameObject = obj as GameObject;
			DrawProperties(obj);
			if (gameObject != null)
			{
				foreach (var child in gameObject.GetComponents<Component>())
				{
					if (child is PEPrefabScript)
						continue;
					DrawObject(child);
				}

				foreach (Transform child in gameObject.transform)
				{
					DrawObject(child.gameObject);
				}
			}

			EditorGUI.indentLevel--;
		}

		void DrawProperties(Object obj)
		{
			var so = new SerializedObject(obj);
			if (obj is GameObject)
			{
				DrawProperty(so.FindProperty("m_IsActive"), EditorGUI.indentLevel, false);
			}
			else if (!pickObject)
			{
				var ident = EditorGUI.indentLevel;
				var p = so.GetIterator();
				bool withChildren;
				System.Func<bool, bool> nextFunction = (enterChildren) => (showHiddenProperties ? p.Next(enterChildren) : p.NextVisible(enterChildren));
				while (nextFunction(withChildren = CheckChild(p)))
				{
					DrawProperty(p, ident, !withChildren);
				}
			}
		}

		void DrawProperty(SerializedProperty property, int rootIdention, bool showChildren)
		{
			if (property.propertyType == SerializedPropertyType.ArraySize)
				return;
			property = property.Copy();
			EditorGUI.indentLevel = rootIdention + property.depth + 1;

			GUILayout.BeginHorizontal();

			PEPropertyHelper.PropertyFieldLayout(property, null, showChildren);

			if (targetProperty != null && targetProperty.serializedObject.targetObject == property.serializedObject.targetObject && property.propertyPath == targetProperty.propertyPath)
			{
				var rect = GUILayoutUtility.GetLastRect();
				EditorGUI.DrawRect(rect, new Color(0, 1, 0, 0.1f));
			}

			if (GUILayout.Button(GUIContent.none, this.plusButtonStyle, GUILayout.Width(30), GUILayout.Height(EditorGUIUtility.singleLineHeight)))
				OnPick(property, !Event.current.control && !Event.current.command);

			GUILayout.EndHorizontal();

			EditorGUI.indentLevel = rootIdention;
		}
	}
}
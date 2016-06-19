using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System;

class PETreeNode
{
	public List<PETreeNode> children = new List<PETreeNode>();

	[SerializeField]
	private bool expanded;

	virtual public bool Expanded
	{ 
		get { return expanded; }
		set
		{
			if (Expanded == value)
				return;

			expanded = value; 
			if (OnExpandChanged != null)
				OnExpandChanged(value);

			if (OnExpandChangedOnce != null)
			{
				OnExpandChangedOnce(value);
				OnExpandChangedOnce = null;
			}
		}
	}

	public event Action<bool> OnExpandChanged;
	public event Action<bool> OnExpandChangedOnce;

	public virtual GUIContent content{ get; set; }

	public Color color = Color.white;

	public object UserData;

	public PETreeNode(bool expanded = true)
	{
		this.expanded = expanded;
	}

	public Rect foldRect;
	public Rect childrenRect;

	public virtual void Draw()
	{
		DrawRoot();

		foldRect = GUILayoutUtility.GetLastRect();

		if (!Expanded)
			return;

		DrawChildrens();

		var lastChildRect = GUILayoutUtility.GetLastRect();

		childrenRect = new Rect(foldRect.x + 0, foldRect.y + foldRect.height, foldRect.width, 0);
		childrenRect.height = (lastChildRect.y + lastChildRect.height) - childrenRect.y;
	}

	public virtual void DrawRoot()
	{
		var c = GUI.color;
		GUI.color = color;
		if (children.Count > 0 || OnExpandChangedOnce != null)
			this.Expanded = EditorGUILayout.Foldout(this.Expanded, this.content);
		else
			EditorGUILayout.LabelField(content);
		GUI.color = c;
	}

	public virtual void DrawChildrens()
	{
		EditorGUI.indentLevel++;
		foreach (var child in children)
		{
			child.Draw();
		}
		EditorGUI.indentLevel--;
	}

	public class PropertyNode : PETreeNode
	{
		public bool includeChildren = false;

		public SerializedProperty property
		{
			get
			{
				return this.UserData as SerializedProperty;
			}
		}

		public override GUIContent content
		{
			get
			{
				if (base.content == null)
					base.content = new GUIContent(((SerializedProperty)UserData).name);
				return base.content;
			}
			set
			{
				base.content = value;
			}
		}

		public override void DrawRoot()
		{
			if (property == null)
				return;

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(property, includeChildren);

			if (EditorGUI.EndChangeCheck())
			{
				property.serializedObject.ApplyModifiedProperties();
			}
		}
	}
}

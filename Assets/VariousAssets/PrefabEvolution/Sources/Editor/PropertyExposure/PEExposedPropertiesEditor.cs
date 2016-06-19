using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PrefabEvolution
{
	class PEExposedPropertiesEditor
	{
		static public PEExposedPropertiesEditor current;

		public PEPrefabScript[] targets;
		public List<ItemDrawer> drawers = new List<ItemDrawer>();
		public List<ItemDrawer> rootDrawers = new List<ItemDrawer>();
		public bool needRepaint;
		public bool needRebuild;
		private bool Expanded = true;

		static private GUIStyle _lockButton;

		static private GUIStyle lockButton
		{
			get
			{
				if (_lockButton == null)
					_lockButton = "IN LockButton";
				return _lockButton;
			}
		}

		private bool isEditMode;

		static public bool editMode
		{
			get
			{
				return current.isEditMode;
			}
			set
			{
				current.isEditMode = value;
			}
		}

		public PEExposedPropertiesEditor(params PEPrefabScript[] targets)
		{
			this.targets = targets;

			Build();
		}

		void DrawLock(Rect rect)
		{
			if (this.targets.Length > 1 || PrefabUtility.GetPrefabParent(this.targets[0].gameObject) != this.targets[0].Prefab)
				return;

			editMode = !GUI.Toggle(new Rect(rect){ x = rect.x + rect.width - 13, width = 13 }, !editMode, GUIContent.none, lockButton);
		}

		public void Build()
		{
				Build (false);
		}

		public void Build(bool setDirty)
		{
			drawers = new List<ItemDrawer>();
			rootDrawers = new List<ItemDrawer>();

			var idDict = new Dictionary<int, List<BaseExposedData>>();

			foreach (var target in this.targets)
			{
				foreach (var property in target.Properties.OrderedItems)
				{
					List<BaseExposedData> list;
					if (!idDict.TryGetValue(property.Id, out list))
						idDict.Add(property.Id, list = new List<BaseExposedData>());
					list.Add(property);
				}
			}

			var comparer = new BaseExposedData.Comparer();
			var orderedList = idDict.Values.ToList();
			orderedList.Sort((x, y) => (comparer.Compare(x[0], y[0])));

			foreach (var data in orderedList)
			{
				drawers.Add(new ItemDrawer(this.targets, data));
			}

			foreach (var drawer in drawers)
			{
				var parentId = drawer.ParentId;
				drawer.Parent = drawers.FirstOrDefault(d => d.Id == parentId);

				if (drawer.Parent != null)
					drawer.Parent.Children.Add(drawer);
			}

			rootDrawers = drawers.Where(d => d.Parent == null).ToList();

			if (setDirty)
				EditorApplication.delayCall += () => targets.Foreach(EditorUtility.SetDirty);
		}

		public void Draw()
		{
			current = this;
			var rect = EditorGUILayout.GetControlRect();

			if (editMode)
			{
				var r = new Rect(rect);
				r.x += r.width - 33;
				r.width = r.height = 16;
				if (GUI.Button(r, PEResources.addIcon, PEUtils.emptyStyle))
					OnAdd(this);
			}

			DrawLock(rect);
			Expanded = EditorGUI.Foldout(rect, Expanded, "Prefab Properties", false);

			if (Expanded)
			{
				EditorGUI.indentLevel++;
				rootDrawers.ForEach(d => d.Draw());

				EditorGUI.indentLevel--;
			}
			if (needRepaint)
			{
				needRepaint = false;
				Event.current.Use();
			}
			if (needRebuild)
			{
				needRebuild = false;
				this.Build(true);
			}
		}

		static public void OnRemove(PEExposedPropertiesEditor editor, int propertyId)
		{
			editor.targets.Foreach(t => t.Properties.Remove(propertyId));
			editor.Build(true);
			Event.current.Use();
			Resources.FindObjectsOfTypeAll<Editor>()[0].Repaint();
			GUIUtility.ExitGUI();
		}

		static public void OnAdd(PEExposedPropertiesEditor editor, int parentId = 0)
		{
			var lr = GUILayoutUtility.GetLastRect();
			if (lr.min == Vector2.zero)
				lr.center = Event.current.mousePosition;
			lr.center = GUIUtility.GUIToScreenPoint(lr.center);
			var menu = new GenericMenu();
			menu.AddItem(new GUIContent("Property"), false, () =>
			{
				PEPropertyPickerWindow.OnPropertyPickerDelegate onChanged = property =>
				{
					var newProperty = new ExposedProperty();
					newProperty.Label = property.GetDisplayName();
					newProperty.PropertyPath = property.propertyPath;
					newProperty.Target = property.serializedObject.targetObject;
					newProperty.ParentId = parentId;
					editor.targets[0].Properties.Add(newProperty);
					newProperty.Order = editor.targets[0].Properties.OrderedItems.Any() ? editor.targets[0].Properties.OrderedItems.Max(i => i.Order + 10) : 0;
					editor.Build(true);

					Event.current.Use();

					Resources.FindObjectsOfTypeAll<Editor>()[0].Repaint();
					GUIUtility.ExitGUI();
				};
				EditorApplication.delayCall += () =>
					PEPropertyPickerWindow.Show(editor.targets[0].gameObject, onChanged, lr);
			});
			menu.AddItem(new GUIContent("Group"), false, () =>
					EditorApplication.delayCall += () =>
			{
				editor.targets[0].Properties.Add(new ExposedPropertyGroup {
					Label = "Property Group", ParentId = parentId,
					Order = editor.targets[0].Properties.OrderedItems.Any() ? editor.targets[0].Properties.OrderedItems.Max(i => i.Order + 10) : 0
				});
				editor.Build(true);
			});
			menu.ShowAsContext();
		}

		static public void OnEdit(PEExposedPropertiesEditor editor, ItemDrawer propertyToEdit)
		{
			var lr = GUILayoutUtility.GetLastRect();

			if (lr.min == Vector2.zero)
				lr.center = Event.current.mousePosition;

			var p = propertyToEdit.ExposedData.First() as ExposedProperty;
			lr.center = GUIUtility.GUIToScreenPoint(lr.center);

			PEPropertyPickerWindow.OnPropertyPickerDelegate onChanged = property =>
			{
				var newProperty = p;
				newProperty.Label = property.GetDisplayName();
				newProperty.PropertyPath = property.propertyPath;
				newProperty.Target = property.serializedObject.targetObject;
				editor.Build(true);
			};
			EditorApplication.delayCall += () =>
				PEPropertyPickerWindow.Show(editor.targets[0].gameObject, onChanged, lr, null, propertyToEdit.Property);
		}
	}

	class ItemDrawer
	{
		public IEnumerable<PEPrefabScript> PrefabScripts;
		public IEnumerable<BaseExposedData> ExposedData;
		public List<ItemDrawer> Children = new List<ItemDrawer>();
		public ItemDrawer Parent;

		public bool Inherited;
		public bool Hidden;

		public GUIContent Label;
		public SerializedProperty Property;
		public bool IsGroup;

		public enum HighLightMode
		{
			None,
			Current,
			Before,
			After,
		}

		private HighLightMode Higlight;

		private bool Expanded
		{
			get
			{
				var exposedPropertyGroup = ExposedData.First() as ExposedPropertyGroup;
				return exposedPropertyGroup != null && exposedPropertyGroup.Expanded;
			}
			set
			{
				var exposedPropertyGroup = ExposedData.First() as ExposedPropertyGroup;
				if (exposedPropertyGroup != null)
					exposedPropertyGroup.Expanded = value;
			}
		}

		public int Id
		{
			get
			{
				return ExposedData.First().Id;
			}
		}

		public int ParentId
		{
			get
			{
				return ExposedData.First().ParentId;
			}
		}

		IEnumerable<ItemDrawer> Parents
		{
			get
			{
				for (var p = this.Parent; p != null; p = p.Parent)
					yield return p;
			}
		}

		public ItemDrawer(IEnumerable<PEPrefabScript> PrefabScripts, IEnumerable<BaseExposedData> ExposedData)
		{
			this.PrefabScripts = PrefabScripts;
			this.ExposedData = ExposedData;

			this.Inherited = ExposedData.First().Inherited;
			this.Hidden = ExposedData.All(p => PrefabScripts.First().Properties.GetHidden(p.Id));

			Label = new GUIContent(ExposedData.First().Label, string.Format("Property id: (uint){0} (int){1}", (uint)ExposedData.First().Id, ExposedData.First().Id));
			IsGroup = !(ExposedData.First() is ExposedProperty);
			if (!IsGroup)
			{
				var exposedProperties = ExposedData.OfType<ExposedProperty>().ToArray();
				var path = exposedProperties[0].PropertyPath;

				var targets = ((PrefabScripts.Count() > 1) ? exposedProperties.Where(p => !p.Hidden) : exposedProperties).Select(data => data.Target).Where(t => t != null).ToArray();

				if (targets.Length > 0)
				{
					var so = new SerializedObject(targets);
					Property = so.FindProperty(path);
				}
			}
		}

		public virtual void Draw()
		{
			var color = GUI.color;

			var rect = EditorGUILayout.GetControlRect(true, this.GetHeight());
			Draw(rect);
			GUI.color = color;
		}

		public virtual void Draw(Rect rect)
		{
			if (PEExposedPropertiesEditor.editMode)
				DrawEdit(rect);
			else
			if (!Hidden)
				DrawNormal(rect);

			if (PEExposedPropertiesEditor.editMode || this.Expanded && !Hidden)
				DrawChildren();
		}

		void DrawHighlight(Rect rect)
		{
			var color = new Color(0, 1, 0, 0.1f);
			switch (Higlight)
			{
				case HighLightMode.None:
					break;
				case HighLightMode.Current:
					EditorGUI.DrawRect(EditorGUI.IndentedRect(rect), color);
					break;
				case HighLightMode.Before:
					{
						var r = rect;
						r.y -= 1;
						r.height = 1;
						EditorGUI.DrawRect(EditorGUI.IndentedRect(r), color);
						break;
					}
				case HighLightMode.After:
					{
						var r = rect;
						r.y += r.height + 1;
						r.height = 1;
						EditorGUI.DrawRect(EditorGUI.IndentedRect(r), color);
						break;
					}
			}
		}

		bool IsParentOf(ItemDrawer drawer)
		{
			return drawer.Parents.Contains(this);
		}

		void UpdateDrag(Rect rect)
		{
			if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragExited)
				Higlight = HighLightMode.None;

			if (Event.current.type == EventType.layout)
				return;

			DrawHighlight(rect);
			var testRect = rect;

			testRect.height += 2;

			if (!testRect.Contains(Event.current.mousePosition))
				return;

			var dragData = DragAndDrop.GetGenericData("ExposedProperty") as ItemDrawer;

			if (Event.current.type != EventType.MouseDrag && (dragData == this || dragData != null && dragData.IsParentOf(this)))
				return;
				
			switch (Event.current.type)
			{
				case EventType.MouseDrag:
					if (!this.Inherited)
					{
						GUI.FocusControl("");
						DragAndDrop.PrepareStartDrag();
						DragAndDrop.SetGenericData("ExposedProperty", this);
						DragAndDrop.paths = new[] {
							" "
						};
						DragAndDrop.StartDrag(this.Label.text);
					}
					break;
				case EventType.DragUpdated:
					DragAndDrop.visualMode = dragData == this ? DragAndDropVisualMode.None : DragAndDropVisualMode.Copy;

					Higlight = IsGroup && Mathf.Abs(Event.current.mousePosition.y - rect.center.y) < 6 ? HighLightMode.Current : Event.current.mousePosition.y > rect.center.y ? HighLightMode.After : HighLightMode.Before;

					if (this.Parent != null && this.Higlight != HighLightMode.Current)
						this.Parent.Higlight = HighLightMode.Current;

					PEExposedPropertiesEditor.current.needRepaint = true;
					break;
				case EventType.DragPerform:
					var dragExposedData = dragData.ExposedData.First();

					var addToCurrent = IsGroup && Higlight == HighLightMode.Current;

					dragExposedData.ParentId = addToCurrent ? this.ExposedData.First().Id : this.ExposedData.First().ParentId;

					if (!addToCurrent)
						dragExposedData.Order = this.ExposedData.First().GetOrder(Higlight == HighLightMode.After);

					CompleteDrag();
					break;
			}
		}

		void CompleteDrag()
		{
			Higlight = HighLightMode.None;
			var editor = PEExposedPropertiesEditor.current;
			DragAndDrop.AcceptDrag();

			EditorApplication.delayCall += () =>
			{
				editor.Build(true);
				EditorApplication.RepaintHierarchyWindow();
			};
			editor.needRebuild = editor.needRepaint = true;
		}

		void DrawEdit(Rect rect)
		{
			UpdateDrag(rect);
			var guiEnabled = GUI.enabled;
			if (this.Inherited)
				GUI.enabled = false;

			var label = ExposedData.First().Label;
			var labelRect = new Rect(rect){ x = rect.x - 1, width = rect.width - 48 };

			EditorGUI.BeginChangeCheck();
			label = EditorGUI.TextField(labelRect, label);
			if (EditorGUI.EndChangeCheck())
			{
				this.Label.text = label;
				this.ExposedData.Foreach(d => d.Label = label);
			}

			labelRect.x += labelRect.width;
			labelRect.width = 16;
			GUI.tooltip = "Remove";

			if (GUI.Button(labelRect, PEResources.removeIcon, PEUtils.emptyStyle))
				PEExposedPropertiesEditor.OnRemove(PEExposedPropertiesEditor.current, this.Id);

			labelRect.x += labelRect.width;

			if (!IsGroup)
			{
				GUI.tooltip = "Edit";
				if (GUI.Button(labelRect, PEResources.editIcon, PEUtils.emptyStyle))
					PEExposedPropertiesEditor.OnEdit(PEExposedPropertiesEditor.current, this);
			}

			GUI.enabled = guiEnabled;

			if (IsGroup)
			{
				GUI.tooltip = "Add";
				if (GUI.Button(labelRect, PEResources.addIcon, PEUtils.emptyStyle))
					PEExposedPropertiesEditor.OnAdd(PEExposedPropertiesEditor.current, this.Id);
			}

			labelRect.x += labelRect.width + 4;

			if (this.Inherited)
			{
				var lw = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 0;

				labelRect.y -= 1;
				GUI.tooltip = "Visible";

				var result = !GUI.Toggle(labelRect, !this.ExposedData.First().Hidden, "Show");

				if (result != this.ExposedData.First().Hidden)
				{
					this.ExposedData.First().Hidden = result;
					EditorApplication.delayCall += PEExposedPropertiesEditor.current.Build;
				}

				EditorGUIUtility.labelWidth = lw;
			}

			DrawHighlight(rect);
		}

		void DrawNormal(Rect rect)
		{
			if (!IsGroup)
			{
				if (Property == null)
				{
					var exposedProperty = (ExposedProperty)this.ExposedData.First();

					if (exposedProperty.Target == null)
						EditorGUI.LabelField(rect, Label.text, string.Format("Target missing! Path:{0}", exposedProperty.PropertyPath));
					else
						EditorGUI.LabelField(rect, Label.text, string.Format("Property missing! Path:{0}", exposedProperty.PropertyPath));
					return;
				}

				Property.serializedObject.UpdateIfDirtyOrScript();

				EditorGUI.BeginChangeCheck();
				PEPropertyHelper.PropertyField(rect, Property, Label, true);

				if (EditorGUI.EndChangeCheck())
					Property.serializedObject.ApplyModifiedProperties();

				Expanded = Property.isExpanded;
			}
			else
			{
				this.Expanded = EditorGUI.Foldout(rect, Expanded, Label);
			}
		}

		void DrawChildren()
		{
			EditorGUI.indentLevel++;
			if (this.Children != null)
			{
				foreach (var drawer in Children)
				{
					drawer.Draw();
				}
			}
			EditorGUI.indentLevel--;
		}

		public virtual float GetHeight()
		{
			float result;
			result = Property == null && !Hidden || PEExposedPropertiesEditor.editMode ? EditorGUIUtility.singleLineHeight : (this.Hidden ? 0 : EditorGUI.GetPropertyHeight(this.Property, Label, true));
			return result;
		}
	}
}

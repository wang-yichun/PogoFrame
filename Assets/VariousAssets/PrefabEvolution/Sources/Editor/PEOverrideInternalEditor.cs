using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace PrefabEvolution
{
	[System.AttributeUsage(System.AttributeTargets.Class)]
	class OverrideInternalEditorTypeMarkAttribute:System.Attribute
	{
		public System.Type type;

		public OverrideInternalEditorTypeMarkAttribute(string typeName)
		{
			var types = Assembly.GetAssembly(typeof(Editor)).GetTypes();
			foreach (var t in types)
			{
				if (t.Name != typeName)
					continue;
			
				this.type = t;
				break;
			}
			if (this.type == null)
				Debug.LogError("Type not found:" + typeName);
		}
	}

	public class PEOverrideInternalEditor : Editor
	{
		public Editor _baseEditor;

		public Editor baseEditor
		{
			get
			{
				if (_baseEditor == null)
				{
					var atr = System.Attribute.GetCustomAttribute(this.GetType(), typeof(OverrideInternalEditorTypeMarkAttribute)) as OverrideInternalEditorTypeMarkAttribute;
					if (atr != null)
					{
						if (this.targets != null && this.targets.Length > 0)
							_baseEditor = CreateEditor(this.targets, atr.type);
					}
					else
					{
						Debug.Log ("Attribute is not found");
					}
				}
				return _baseEditor;
			}
		}

		virtual protected void OnEnable()
		{
			var editor = baseEditor;
			if (editor == null)
				Debug.LogError("Failed to initialize:" + this.GetType());
		}
			
		virtual protected void OnDisable()
		{
			DestroyImmediate(_baseEditor);
		}

		private MethodInfo OnSceneGUIMethod;
		virtual protected void OnSceneGUI()
		{
			if (baseEditor == null)
				return;

			if (OnSceneGUIMethod == null)
				OnSceneGUIMethod = baseEditor.GetType().GetMethod("OnSceneGUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
			if (OnSceneGUIMethod != null)
				OnSceneGUIMethod.Invoke(baseEditor, new object[0]);
		}

		private MethodInfo OnSceneDragMethod;
		public void OnSceneDrag(SceneView sceneView)
		{
			if (baseEditor == null)
				return;

			if (OnSceneDragMethod == null)
				OnSceneDragMethod = baseEditor.GetType().GetMethod("OnSceneDrag", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
			if (OnSceneDragMethod != null)
				OnSceneDragMethod.Invoke(baseEditor, new object[]{sceneView});
		}

		private MethodInfo OnHeaderGUIMethod;
		protected override void OnHeaderGUI()
		{
			if (OnHeaderGUIMethod == null)
				OnHeaderGUIMethod = baseEditor.GetType().GetMethod("OnHeaderGUI", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

			OnHeaderGUIMethod.Invoke(baseEditor, new object[0]);
		}

		public override GUIContent GetPreviewTitle()
		{
			return target == null ? null : baseEditor.GetPreviewTitle();
		}

		public override string GetInfoString()
		{
			return target == null ? "" : baseEditor.GetInfoString();
		}

		public override bool HasPreviewGUI()
		{
			return target != null && baseEditor.HasPreviewGUI();
		}

		public override void OnInspectorGUI()
		{
			if (target != null)
				baseEditor.OnInspectorGUI();
		}

		public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
		{
			if (target != null)
				baseEditor.OnInteractivePreviewGUI(r, background);
		}

		public override void OnPreviewGUI(Rect r, GUIStyle background)
		{
			if (target != null)
				baseEditor.OnPreviewGUI(r, background);
		}

		public override void OnPreviewSettings()
		{
			if (target != null)
				baseEditor.OnPreviewSettings();
		}

		public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
		{
			return target == null ? null : baseEditor.RenderStaticPreview(assetPath, subAssets, width, height);
		}

		public override void DrawPreview(Rect previewArea)
		{
			baseEditor.DrawPreview(previewArea);
		}

		public override void ReloadPreviewInstances()
		{
			baseEditor.ReloadPreviewInstances();
		}

		public override bool RequiresConstantRepaint()
		{
			return baseEditor.RequiresConstantRepaint();
		}

		public override bool UseDefaultMargins()
		{
			return baseEditor.UseDefaultMargins();
		}
	}
}


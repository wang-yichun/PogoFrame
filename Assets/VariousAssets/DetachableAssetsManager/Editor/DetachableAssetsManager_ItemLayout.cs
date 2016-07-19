namespace pogorock
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEditor;
	using Newtonsoft.Json;
	using System.IO;
	using System.Linq;

	public partial class DetachableAssetsManagerWindow : EditorWindow
	{

		void InfoItemLayout (DetachableAssetInfo info)
		{
			EditorGUILayout.BeginVertical ("box");
			EditorGUILayout.BeginHorizontal ();


			bool enable = false;
			if (SymbolHelper.ExistSymbol (info.Symbol)) {
				enable = true;
			}
			EditorGUILayout.Space ();
			EditorGUILayout.BeginVertical (GUILayout.Width (100f));
			EditorGUILayout.Space ();
			if (enable) {
				if (Directory.Exists (info.DevDataPathRoot)) {
					GUILayout.Label (gizmo_integrated);
				} else {
					GUILayout.Label (gizmo_ok_nobackup);
				}
			} else {
				if (Directory.Exists (info.DevDataPathRoot)) {
					GUILayout.Label (gizmo_detached);
				} else {
					GUILayout.Label (gizmo_unready);
				}
			}
			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("集成")) {
				DoIntegrate (info);
			}
			if (GUILayout.Button ("拆卸")) {
				DoDetach (info);
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("备份")) {
				DoCopy (info);
			}
			if (GUILayout.Button ("删除")) {
				DoDelete (info);
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("清理")) {
				DoClean (info);
			}
			if (GUILayout.Button ("打包")) {
				DoExportPackage (info);
			}
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();

			EditorGUILayout.Space ();

			fixedInfoContent (info, enable);

			EditorGUILayout.EndHorizontal ();

			multiPaths (info);

			EditorGUILayout.EndVertical ();
		}

		void fixedInfoContent (DetachableAssetInfo info, bool enable)
		{
			EditorGUILayout.BeginVertical (GUILayout.Width (350f));

			string tip_after_title = string.Empty;
			if (enable == false) {
				if (Directory.Exists (info.DevDataPathRoot) == false) {
					tip_after_title = "(未准备)";
				} else {
					tip_after_title = "(已拆卸)";
				}
			}

			string title = string.Format ("{0} ({1}) {2}", info.Name, info.Description, tip_after_title);
			GUILayout.Label (title, EditorStyles.largeLabel);
			GUILayout.Label ("    版本: " + info.Version ?? "(with no version)");

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("    官网: " + info.Url);
			EditorGUILayout.BeginVertical (GUILayout.Width (50f));
			if (GUILayout.Button (gizmo_enter)) {
				Application.OpenURL (string.Format (info.Url));
			}
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();

			GUILayout.Label ("    备份: " + info.DevDataPathRoot);

			if (info.isMultiPaths) {
//				if (info.rootsFolded = EditorGUILayout.ToggleLeft ("项目中位置: ", info.rootsFolded)) {
//					EditorGUI.indentLevel++;
//					EditorGUILayout.BeginVertical ("box");
//					for (int i = 0; i < info.AssetsPathRoots.Length; i++) {
//						EditorGUILayout.BeginHorizontal ();
//						EditorGUILayout.BeginVertical (GUILayout.Width (20f));
//						GUILayout.Toggle (info.AssetsPathRoots [i].integrate, "集成");
//						EditorGUILayout.EndVertical ();
//						EditorGUILayout.BeginVertical (GUILayout.Width (20f));
//						GUILayout.Toggle (info.AssetsPathRoots [i].integrate, "备份");
//						EditorGUILayout.EndVertical ();
//						GUILayout.Label ("      " + info.AssetsPathRoots [i].path);
//						EditorGUILayout.EndHorizontal ();
//					}
//					EditorGUILayout.EndVertical ();
//					EditorGUI.indentLevel--;
//				}
				if (info.AssetsPathRoots == null || info.AssetsPathRoots.Length == 0) {

					GUILayout.Label (string.Format ("    项目: (未定义)", info.AssetsPathRoots [0], info.AssetsPathRoots.Length));
				} else {

					EditorGUILayout.BeginHorizontal ();
					GUILayout.Label (string.Format ("    项目: {0}... (等{1}个位置)", info.AssetsPathRoots [0].path, info.AssetsPathRoots.Length));

					EditorGUILayout.BeginVertical (GUILayout.Width (50f));
					if (GUILayout.Button (gizmo_detail)) {
						info.rootsFolded = !info.rootsFolded;
					}
					EditorGUILayout.EndVertical ();
					EditorGUILayout.EndHorizontal ();
				}
			} else {
				GUILayout.Label ("    项目: " + info.AssetsPathRoot);
			}

			GUILayout.Label ("    定义: " + info.Symbol);
			EditorGUILayout.EndVertical ();
		}

		string inputPath;

		void multiPaths (DetachableAssetInfo info)
		{
			if (info.isMultiPaths && info.rootsFolded) {
//				if (info.rootsFolded = EditorGUILayout.ToggleLeft ("项目中位置: (详细)", info.rootsFolded)) {
				EditorGUILayout.BeginVertical ("box");
				for (int i = 0; i < info.AssetsPathRoots.Length; i++) {
					EditorGUILayout.BeginHorizontal ();
					EditorGUILayout.BeginVertical (GUILayout.Width (20f));
					info.AssetsPathRoots [i].integrate = GUILayout.Toggle (info.AssetsPathRoots [i].integrate, "集成");
					EditorGUILayout.EndVertical ();
					EditorGUILayout.BeginVertical (GUILayout.Width (20f));
					info.AssetsPathRoots [i].backup = GUILayout.Toggle (info.AssetsPathRoots [i].backup, "备份");
					EditorGUILayout.EndVertical ();
					GUILayout.Label ("      " + info.AssetsPathRoots [i].path);
					EditorGUILayout.BeginVertical (GUILayout.Width (20f));
					if (GUILayout.Button (gizmo_del)) {
						List<AssetsPathRootInfo> list = info.AssetsPathRoots.ToList ();
						list.RemoveAt (i);
						info.AssetsPathRoots = list.ToArray ();
					}
					EditorGUILayout.EndVertical ();
					EditorGUILayout.EndHorizontal ();
				}

				EditorGUILayout.BeginHorizontal ();

				canDragPathsLabel (info);
				EditorGUILayout.BeginVertical (GUILayout.Width (20f));
				if (GUILayout.Button (gizmo_add)) {
					List<AssetsPathRootInfo> list = info.AssetsPathRoots.ToList ();
					if (list.Exists (_ => _.path == inputPath) == false) {
						list.Add (new AssetsPathRootInfo () {
							path = inputPath,
							integrate = true,
							backup = true
						});
					}
					info.AssetsPathRoots = list.ToArray ();
				}
				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndHorizontal ();




				EditorGUILayout.EndVertical ();
			} 
		}

		void canDragPathsLabel (DetachableAssetInfo info)
		{
			Rect sfxPathRect = EditorGUILayout.GetControlRect ();
			// 用刚刚获取的文本输入框的位置和大小参数，创建一个文本输入框，用于输入特效路径

			inputPath = EditorGUI.TextField (sfxPathRect, inputPath);

			// 判断当前鼠标正拖拽某对象或者在拖拽的过程中松开了鼠标按键
			// 同时还需要判断拖拽时鼠标所在位置处于文本输入框内 
			if (sfxPathRect.Contains (Event.current.mousePosition)) {
				// 判断是否拖拽了文件 
				if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0) {
					string[] sfxPath = DragAndDrop.paths;
					// 拖拽的过程中，松开鼠标之后，拖拽操作结束，此时就可以使用获得的 sfxPath 变量了 
					if (sfxPath != null && sfxPath.Length > 0 && Event.current.type == EventType.DragExited) {
						DragAndDrop.AcceptDrag ();
						// 好了，这下想用这个 sfxPath 变量干嘛就干嘛吧 
						Debug.Log ("sfxPaths:" + JsonConvert.SerializeObject (sfxPath));

						List<AssetsPathRootInfo> list = info.AssetsPathRoots.ToList ();

						foreach (string path in sfxPath) {

							if (list.Exists (_ => _.path == path) == false) {
								list.Add (new AssetsPathRootInfo () {
									path = path,
									integrate = true,
									backup = true
								});
							}
						}

						info.AssetsPathRoots = list.ToArray ();

						DragAndDrop.paths = null;
					}
				}
			}
		}
	}
}
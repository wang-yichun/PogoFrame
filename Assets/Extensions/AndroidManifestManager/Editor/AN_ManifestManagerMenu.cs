using UnityEngine;
using UnityEditor;
using System.Collections;

static public class AN_ManifestManagerMenu {

	[MenuItem("Window/Stan's Assets/Manifest Manager/Edit")]
	public static void LoadManagerWindow() {
		EditorWindow.GetWindow<AN_ManifestManagerWindow>();
	}

	//--------------------------------------
	//  GENERAL
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Manifest Manager/Documentation/Getting Started/Overview")]
	public static void AMMOverview() {
		Application.OpenURL("https://unionassets.com/android-manifest-manager/overview-222");
	}

	[MenuItem("Window/Stan's Assets/Manifest Manager/Documentation/Getting Started/Using with Unity Editor")]
	public static void AMMUsingWithUnityEditor() {
		Application.OpenURL("https://unionassets.com/android-manifest-manager/using-with-unity-editor-223");
	}

	[MenuItem("Window/Stan's Assets/Manifest Manager/Documentation/Getting Started/Scripting API")]
	public static void AMMScriptingAPI() {
		Application.OpenURL("https://unionassets.com/android-manifest-manager/scripting-api-244");
	}

}

////////////////////////////////////////////////////////////////////////////////
//  
// @module V2D
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
public class SocialPlatfromInit  {

	static SocialPlatfromInit () {

		if(SA_FileStaticAPI.IsFolderExists("Extensions/UltimateMobile/")) {
			return;
		}

		if(SA_FileStaticAPI.IsFolderExists("Extensions/AndroidNative/")) {
			return;
		}


		if(!SocialPlatfromSettingsEditor.IsInstalled) {
			EditorApplication.update += OnEditorLoaded;
		} else {
			if(!SocialPlatfromSettingsEditor.IsUpToDate) {
				EditorApplication.update += OnEditorLoaded;
			}
		}
		
	}
	
	private static void OnEditorLoaded() {
		
		EditorApplication.update -= OnEditorLoaded;
		Debug.LogWarning("Mobile Social Plugin Install Required. Opening Plugin settings...");
		Selection.activeObject = SocialPlatfromSettings.Instance;
	}
}

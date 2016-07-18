using UnityEngine;
using UnityEditor;
using System.Collections;

public class SA_EditorTool {


	public static void ContactSupportWithSubject(string subject) {
		string url = "mailto:support@stansassets.com?subject=" + EscapeURL(subject);
		Application.OpenURL(url);
	}

	static	string  EscapeURL (string url){
		return WWW.EscapeURL(url).Replace("+","%20");
	}

	private static Texture2D _SALogo = null;
	
	public static Texture2D SALogo {
		get {
			if(_SALogo == null) {
				if(EditorGUIUtility.isProSkin) {
					_SALogo =  Resources.Load("sa_logo_small") as Texture2D;
				} else {
					_SALogo =  Resources.Load("sa_logo_small_light") as Texture2D;
				}
			} 
			
			return _SALogo;
		}
	}
	
	
	public static void DrawSALogo() {
		
		GUIStyle s =  new GUIStyle();
		GUIContent content =  new GUIContent(SALogo, "Visit site");
		
		bool click = GUILayout.Button(content, s);
		if(click) {
			Application.OpenURL("https://stansassets.com/");
		}
	}

	public static bool ToggleFiled(string title, bool value) {
		
		SA_Bool initialValue = SA_Bool.Enabled;
		if(!value) {
			initialValue = SA_Bool.Disabled;
		}
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(title);
		
		initialValue = (SA_Bool) EditorGUILayout.EnumPopup(initialValue);
		if(initialValue == SA_Bool.Enabled) {
			value = true;
		} else {
			value = false;
		}
		EditorGUILayout.EndHorizontal();
		
		return value;
	}



	public static bool SrotingButtons(object currentObject, IList ObjectsList) {
		
		int ObjectIndex = ObjectsList.IndexOf(currentObject);
		if(ObjectIndex == 0) {
			GUI.enabled = false;
		} 
		
		bool up 		= GUILayout.Button("↑", EditorStyles.miniButtonLeft, GUILayout.Width(20));
		if(up) {
			object c = currentObject;
			ObjectsList[ObjectIndex]  		= ObjectsList[ObjectIndex - 1];
			ObjectsList[ObjectIndex - 1] 	=  c;
		}
		
		
		if(ObjectIndex >= ObjectsList.Count -1) {
			GUI.enabled = false;
		} else {
			GUI.enabled = true;
		}
		
		bool down 		= GUILayout.Button("↓", EditorStyles.miniButtonMid, GUILayout.Width(20));
		if(down) {
			object c = currentObject;
			ObjectsList[ObjectIndex] =  ObjectsList[ObjectIndex + 1];
			ObjectsList[ObjectIndex + 1] = c;
		}
		
		
		GUI.enabled = true;
		bool r 			= GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20));
		if(r) {
			ObjectsList.Remove(currentObject);
		}
		
		return r;
	}



	public static void ChnageDefineState(string file, string tag, bool IsEnabled) {
		if(SA_FileStaticAPI.IsFileExists(file)) {
			string content = SA_FileStaticAPI.Read(file);
			//	Debug.Log(file);
			//Debug.Log(content);
			
			int endlineIndex;
			endlineIndex = content.IndexOf(System.Environment.NewLine);
			if(endlineIndex == -1) {
				endlineIndex = content.IndexOf("\n");
			}
			string TagLine = content.Substring(0, endlineIndex);
			
			if(IsEnabled) {
				content 	= content.Replace(TagLine, "#define " + tag);
			} else {
				content 	= content.Replace(TagLine, "//#define " + tag);
			}
			//		Debug.Log(content);
			
			SA_FileStaticAPI.Write(file, content);
		}		
	}


	public static void BlockHeader(string header) {
		EditorGUILayout.Space();
		EditorGUILayout.HelpBox(header, MessageType.None);
		EditorGUILayout.Space();
	}


	private static GUIContent SupportEmail = new GUIContent("Support [?]", "If you have any technical questions, feel free to drop us an e-mail");
	public static void SupportMail() {
		SelectableLabelField(SupportEmail, "support@stansassets.com");
	}

	private static GUIContent FBdkVersion   	= new GUIContent("Facebook SDK Version [?]", "Version of Unity Facebook SDK Plugin");
	public static void FBSdkVersionLabel () {

		string SdkVersionCode = SA_ModulesInfo.FB_SDK_VersionCode;




		SelectableLabelField(FBdkVersion,  SdkVersionCode);
	}



	
	public static void SelectableLabelField(GUIContent label, string value) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(label, GUILayout.Width(180), GUILayout.Height(16));
		EditorGUILayout.SelectableLabel(value, GUILayout.Height(16));
		EditorGUILayout.EndHorizontal();
	}

}

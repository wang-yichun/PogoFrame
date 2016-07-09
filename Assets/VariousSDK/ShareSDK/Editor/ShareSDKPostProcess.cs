using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using Debug = UnityEngine.Debug;

public class ShareSDKPostProcess
{

	[PostProcessBuild (100)]
	public static void OnPostprocessBuild (BuildTarget target, string pathToBuiltProject)
	{
		Debug.Log ("ShareSDKPostProcess began to run!!");
	}

	[MenuItem ("PogoTools/Run ShareSDK PostProcess #%c")]
	public static void Execute ()
	{
		string pathToBuiltProject = @"/Users/EthanW/Documents/UnityProjects/PogoFrame/Builds/IOSProj/IOSProj";
		OnPostprocessBuild (BuildTarget.iOS, pathToBuiltProject);
	}
}

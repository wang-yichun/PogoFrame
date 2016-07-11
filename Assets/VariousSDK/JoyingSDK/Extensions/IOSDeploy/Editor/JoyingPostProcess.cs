namespace pogorock.Joying
{
	using UnityEngine;
	using UnityEditor;
	using UnityEditor.Callbacks;
	using System.Collections;
	using System.Diagnostics;
	using System.Xml;
	using System.Collections.Generic;
	using System.IO;
	using System.Text.RegularExpressions;
	using UnityEditor.iOS.Xcode;
	using System;

	using Debug = UnityEngine.Debug;

//	SystemConfiguration.framework
//	CFNetwork.framework
//	MediaPlayer.framework
//	libz.dylib
//	StoreKit.framework
//	CoreMotion.framework
//	AudioToolbox.framework
//	libicucore.tbd
//	AdSupport.framework
//	CoreTelephony.framework
//	Security.framework

	public class JoyingPostProcess
	{
		[PostProcessBuild (100)]
		public static void OnPostprocessBuild (BuildTarget buildTarget, string path)
		{
			Debug.Log ("ShareSDKPostProcess began to run!!");

			if (buildTarget == BuildTarget.iOS) {
				string projPath = PBXProject.GetPBXProjectPath (path);

				PBXProject proj = new PBXProject ();
				proj.ReadFromString (File.ReadAllText (projPath));

				string targetName = PBXProject.GetUnityTargetName ();
				string target = proj.TargetGuidByName (targetName);

				proj.AddFrameworkToProject (target, "SystemConfiguration.framework", false);
				proj.AddFrameworkToProject (target, "CFNetwork.framework", false);
				proj.AddFrameworkToProject (target, "MediaPlayer.framework", false);

				proj.AddFileToBuild(target, proj.AddFile("usr/lib/libz.dylib", "Frameworks/libz.dylib", PBXSourceTree.Sdk));

				proj.AddFrameworkToProject (target, "StoreKit.framework", false);
				proj.AddFrameworkToProject (target, "CoreMotion.framework", false);
				proj.AddFrameworkToProject (target, "AudioToolbox.framework", false);

				AddTBDFile (projPath, proj, target, "libicucore.tbd");

				proj.AddFrameworkToProject (target, "AdSupport.framework", false);
				proj.AddFrameworkToProject (target, "CoreTelephony.framework", false);
				proj.AddFrameworkToProject (target, "Security.framework", false);

				proj.AddBuildProperty (target, "OTHER_LDFLAGS", "-ObjC");

				File.WriteAllText (projPath, proj.WriteToString ());
			}
		}


		public static void AddTBDFile (string projPath, PBXProject proj, string target, string tbdName)
		{
			proj.AddFileToBuild (target, proj.AddFile ("usr/lib/" + tbdName, "Frameworks/" + tbdName, PBXSourceTree.Sdk));
			string projText = addTbdLibrary (target, proj.WriteToString (), tbdName);

			File.WriteAllText (projPath, projText);
		}

		private static string addTbdLibrary (string target, string projText, string tbdName)
		{
			string[] lines = projText.Split ('\n');
			List<string> newLines = new List<string> ();

			string refId = null;
			bool editFinish = false;

			for (int i = 0; i < lines.Length; i++) {

				string line = lines [i];

				if (editFinish) {
					newLines.Add (line);

				} else if (line.IndexOf (tbdName) > -1) {
					if (refId == null && line.IndexOf ("PBXBuildFile") > -1) {
						refId = line.Substring (0, line.IndexOf ("/*")).Trim ();
					} else if (line.IndexOf ("lastKnownFileType") > -1) {
						line = line.Replace ("lastKnownFileType = file;", "lastKnownFileType = \"sourcecode.text-based-dylib-definition\";");
					}
					newLines.Add (line);

				} else if (line.IndexOf ("isa = PBXFrameworksBuildPhase;") > -1) {
					do {
						newLines.Add (line);
						line = lines [++i];
					} while (line.IndexOf ("files = (") == -1);

					while (true) {
						if (line.IndexOf (")") > -1) {
							newLines.Add (refId + ",");
							newLines.Add (line);
							break;
						} else if (line.IndexOf (refId) > -1) {
							newLines.Add (line);
							break;
						} else {
							newLines.Add (line);
							line = lines [++i];
						}
					}
					editFinish = true;

				} else {
					newLines.Add (line);
				}
			}

			return string.Join ("\n", newLines.ToArray ());
		}

		[MenuItem ("PogoTools/Run Joying PostProcess")]
		public static void Execute ()
		{
			string pathToBuiltProject = @"/Users/EthanW/Documents/UnityProjects/PogoFrame/Builds/IOSProj";
			OnPostprocessBuild (BuildTarget.iOS, pathToBuiltProject);
		}
	}
}
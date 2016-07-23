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

/// <summary>
/// ethan tip: 为了将ShareSDK 整个文件夹导入iOS 项目, 需要先将 ShareSDK 拷入Unity 项目根目录的 /DevData 下准备好.
/// Build之前,把准备ShareSDK 的位置应该在 [Unity项目根目录]/DevData/ShareSDK
/// </summary>

public class ShareSDKPostProcess
{

	[PostProcessBuild (110)]
	public static void OnPostprocessBuild (BuildTarget buildTarget, string path)
	{
		if (buildTarget == BuildTarget.iOS) {
			
			Debug.Log ("ShareSDKPostProcess began to run!!");

			string projPath = PBXProject.GetPBXProjectPath (path);

			PBXProject proj = new PBXProject ();
			proj.ReadFromString (File.ReadAllText (projPath));

			string targetName = PBXProject.GetUnityTargetName ();
			string target = proj.TargetGuidByName (targetName);

			AddTBDFile (projPath, proj, target, "libicucore.tbd");
			AddTBDFile (projPath, proj, target, "libz.tbd");
			AddTBDFile (projPath, proj, target, "libstdc++.tbd");
			AddTBDFile (projPath, proj, target, "libsqlite3.tbd");

			proj.AddFrameworkToProject (target, "JavaScriptCore.framework", false);
			proj.AddFrameworkToProject (target, "ImageIO.framework", false);

			CopyShareSDKDirtionary (path, proj, target);

			proj.AddBuildProperty (target, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/ShareSDK");
			proj.AddBuildProperty (target, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/ShareSDK/Support/Optional");
			proj.AddBuildProperty (target, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/ShareSDK/Support/PlatformSDK/QQSDK");
			proj.AddBuildProperty (target, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/ShareSDK/Support/Required");

			proj.AddBuildProperty (target, "LIBRARY_SEARCH_PATHS", "$(PROJECT_DIR)/ShareSDK/Support/PlatformSDK/SinaWeiboSDK");
			proj.AddBuildProperty (target, "LIBRARY_SEARCH_PATHS", "$(PROJECT_DIR)/ShareSDK/Support/PlatformSDK/WeChatSDK");
			proj.AddBuildProperty (target, "LIBRARY_SEARCH_PATHS", "$(SRCROOT)/Libraries");

			File.WriteAllText (projPath, proj.WriteToString ());
		}
	}

	public static void CopyShareSDKDirtionary (string projRootPath, PBXProject proj, string target)
	{
		string sourceDirectory = Application.dataPath + "/../DevData/ShareSDK";
		string destDirectory = Path.Combine (projRootPath, "ShareSDK");

		Debug.Log (string.Format ("source: {0}\ndest: {1}", sourceDirectory, destDirectory));

		copyDirectory (
			sourceDirectory, 
			destDirectory,
			(sourceFile, destFile) => {
				destFile = destFile.Replace (projRootPath + "/", string.Empty);
				Debug.Log (string.Format ("1: {0}\n2: {1}", sourceFile, destFile));
				proj.AddFileToBuild (target, proj.AddFile (destFile, destFile, PBXSourceTree.Source));
			},
			projRootPath
		);
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

	public static void copyDirectory (string sourceDirectory, string destDirectory, Action<string, string> afterCopyCallback = null, string projRootPath = "")
	{
		//判断源目录和目标目录是否存在，如果不存在，则创建一个目录
		if (!Directory.Exists (sourceDirectory)) {
			Directory.CreateDirectory (sourceDirectory);
		}
		if (!Directory.Exists (destDirectory)) {
			Directory.CreateDirectory (destDirectory);
		}

		//拷贝子目录       
		//获取所有子目录名称

		Action<string,string> sub_callback = afterCopyCallback;

		if (sourceDirectory.EndsWith (".framework") || sourceDirectory.EndsWith (".bundle")) {
			sub_callback = null;
		}

		string[] directionName = Directory.GetDirectories (sourceDirectory);

		foreach (string directionPath in directionName) {
			//根据每个子目录名称生成对应的目标子目录名称
			string directionPathTemp = destDirectory + "/" + directionPath.Substring (sourceDirectory.Length + 1);

			//递归下去
			copyDirectory (directionPath, directionPathTemp, sub_callback, projRootPath);
		}

		//拷贝文件
		if (sourceDirectory.EndsWith (".framework") || sourceDirectory.EndsWith (".bundle")) {
			
			copyFile (sourceDirectory, destDirectory, sub_callback);

			string destFile = destDirectory.Replace (projRootPath + "/", string.Empty);
			Debug.Log ("*.framework destFile: " + destFile);

			if (afterCopyCallback != null) {
				afterCopyCallback.Invoke (sourceDirectory, destDirectory);
			}
		} else {
			copyFile (sourceDirectory, destDirectory, afterCopyCallback);
		}
	}

	public static void copyFile (string sourceDirectory, string destDirectory, Action<string, string> afterCopyCallback = null)
	{
		//获取所有文件名称
		string[] fileName = Directory.GetFiles (sourceDirectory);

		foreach (string filePath in fileName) {
			//根据每个文件名称生成对应的目标文件名称
			string filePathTemp = destDirectory + "/" + filePath.Substring (sourceDirectory.Length + 1);

			if (filePathTemp.ToLower ().Contains (".ds_store"))
				continue;

			//若不存在，直接复制文件；若存在，覆盖复制
			if (File.Exists (filePathTemp)) {
				File.Copy (filePath, filePathTemp, true);
			} else {
				File.Copy (filePath, filePathTemp);
			}

			Debug.Log (string.Format ("file copy: {0}", filePathTemp));
			if (afterCopyCallback != null) {
				afterCopyCallback.Invoke (filePath, filePathTemp);
			}
		}
	}

//	[MenuItem ("PogoTools/Run ShareSDK PostProcess")]
//	public static void Execute ()
//	{
//		string pathToBuiltProject = @"/Users/EthanW/Documents/UnityProjects/PogoFrame/Builds/IOSProj";
//		OnPostprocessBuild (BuildTarget.iOS, pathToBuiltProject);
//	}

	public static  void SetURLTypesAndWhiteName (string path)
	{
		string infoPlistPath = Path.Combine (path, "./Info.plist");
		PlistDocument plist = new PlistDocument ();
		plist.ReadFromString (File.ReadAllText (infoPlistPath));

		PlistElementDict rootDict = plist.root;
		SetURLTypes (rootDict);
		SetWhiteNameArray (rootDict);

		File.WriteAllText (infoPlistPath, plist.WriteToString ());
	}

	public static void SetURLTypes (PlistElementDict rootDict)
	{
		PlistElementArray URLTypesArray;
		if (rootDict.values.ContainsKey ("CFBundleURLTypes")) {
			URLTypesArray = rootDict ["CFBundleURLTypes"].AsArray ();
		} else {
			URLTypesArray = rootDict.CreateArray ("CFBundleURLTypes");
		}
		var URLTypesDicItem = URLTypesArray.AddDict ();
		var schemesArray = URLTypesDicItem.CreateArray ("CFBundleURLSchemes");
		schemesArray.AddString ("wx4868b35061f87885");
	}

	public static void SetWhiteNameArray (PlistElementDict rootDict)
	{
		PlistElementArray WhiteNameArray;
		if (rootDict.values.ContainsKey ("LSApplicationQueriesSchemes")) {
			WhiteNameArray = rootDict ["LSApplicationQueriesSchemes"].AsArray ();
		} else {
			WhiteNameArray = rootDict.CreateArray ("LSApplicationQueriesSchemes");
		}
		WhiteNameArray.AddString ("weixin");
		WhiteNameArray.AddString ("wechat");
	}
}

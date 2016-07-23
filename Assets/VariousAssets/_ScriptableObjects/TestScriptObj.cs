using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
#endif

public class TestScriptObj : ScriptableObject
{
	public static readonly string assetName = "TestScriptObj";
	public static readonly string fullPath = "Assets/OriginAssets/_ScriptableObjectsAssets/TestScriptObj.asset";

	private static TestScriptObj instance = null;

	public static TestScriptObj Instance {
		get {
			if (instance == null) {
				instance = Resources.Load<TestScriptObj> (assetName);
				if (instance == null) {
					instance = CreateInstance<TestScriptObj> ();

					#if UNITY_EDITOR
					AssetDatabase.CreateAsset (instance, fullPath);
					#endif
				}
			}
			return instance;
		}
	}
	//	#if UNITY_EDITOR
	//	[MenuItem ("PogoTools/TestScriptObj")]
	//	public static void Execute ()
	//	{
	//		Selection.activeObject = TestScriptObj.Instance;
	//	}
	//	#endif

	// 定义数据载体

	public string varString = "Hello World!";
	public int varInt = 123;
	public bool varBool = false;
	public List<InnerInfo> varList;
}

[Serializable]
public class InnerInfo
{
	public string a;
	public int b;
}
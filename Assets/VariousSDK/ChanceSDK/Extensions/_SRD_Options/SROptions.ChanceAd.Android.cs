#if SDK_CHANCEAD

using UnityEngine;
using System.ComponentModel;
using UnityEngine.Advertisements;
using pogorock.ChanceAd;

public partial class SROptions
{
	#if UNITY_ANDROID

	//	[Category ("Android Plugin Test")]
	//	public void AndroidPlugin_Test_001 ()
	//	{
	//		Debug.Log ("AndroidPlugin_Test_001() Begin. ReturnSum");
	//		AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
	//		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
	//		int result = jo.Call<int> ("ReturnSum", 1, 2);
	//		Debug.Log ("AndroidPlugin_Test_001() End: " + result);
	//	}
	//
	//	[Category ("Android Plugin Test")]
	//	public void AndroidPlugin_Test_002 ()
	//	{
	//		Debug.Log ("AndroidPlugin_Test_002() Begin. ReturnHelloWorld");
	//		AndroidJavaClass jc = new AndroidJavaClass ("com.pogorockgames.pogoframe.ExampleOverrideActivity");
	//		string result = jc.CallStatic<string> ("ReturnHelloWorld");
	//		Debug.Log ("AndroidPlugin_Test_002() End: " + result);
	//	}

	//	[Category ("Android Plugin Test"),DisplayName ("[NativeRecommend]")]
	//	public void AndroidPlugin_Test_003 ()
	//	{
	//		Debug.Log ("AndroidPlugin_Test_003() Begin. ReturnHelloWorld");
	//		AndroidJavaClass jc = new AndroidJavaClass ("com.pogorockgames.chancead.ChanceAndroidBridge");
	//		jc.CallStatic ("NativeRecommend");
	//		Debug.Log ("AndroidPlugin_Test_003() End: ");
	//	}


	//	[Category ("Android Plugin Test")]
	//	public void AndroidPlugin_Test_001 ()
	//	{
	//		Debug.Log ("AndroidPlugin_Test_001() Begin. Example 1");
	//		AndroidJavaObject jo = new AndroidJavaObject ("java.lang.String", "some_string");
	//		int hash = jo.Call<int> ("hashCode");
	//		Debug.Log ("AndroidPlugin_Test_001() Result: " + hash);
	//	}
	//
	//	[Category ("Android Plugin Test")]
	//	public void AndroidPlugin_Test_002 ()
	//	{
	//		Debug.Log ("AndroidPlugin_Test_002() Begin. Example 2");
	//		AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
	//		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity");
	//
	//		string canonicalPath = jo.Call<AndroidJavaObject> ("getCacheDir").Call<string> ("getCanonicalPath");
	//		Debug.Log ("AndroidPlugin_Test_002() Result: " + canonicalPath);
	//	}
	//
	//	[Category ("Android Plugin Test")]
	//	public void AndroidPlugin_Test_003 ()
	//	{
	//		Debug.Log ("AndroidPlugin_Test_003() Begin. Example 3");
	//		AndroidJNIHelper.debug = true;
	//		using (AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
	//			jc.CallStatic ("UnitySendMessage", "UnitySendMessageReceiver", "JavaMessage", "Hello World!");
	//		}
	//		Debug.Log ("AndroidPlugin_Test_003() End.");
	//	}
	//
	//	[Category ("Android Plugin Test")]
	//	public void AndroidPlugin_Test_004 ()
	//	{
	//		Debug.Log ("AndroidPlugin_Test_004() Begin. - onBackPressed");
	//		AndroidJNIHelper.debug = true;
	//		using (AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
	//			using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity")) {
	//				jo.Call ("onBackPressed");
	//			}
	//		}
	//		Debug.Log ("AndroidPlugin_Test_004() End.");
	//	}
	//
	//	[Category ("Android Plugin Test")]
	//	public void AndroidPlugin_Test_005 ()
	//	{
	//		Debug.Log ("AndroidPlugin_Test_005() Begin. - ReturnHelloWorld");
	//		AndroidJNIHelper.debug = true;
	//		using (AndroidJavaClass jc = new AndroidJavaClass ("com.unity3d.player.UnityPlayer")) {
	//			using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject> ("currentActivity")) {
	//				string result = jo.Call<string> ("ReturnHelloWorld");
	//				Debug.Log ("AndroidPlugin_Test_005() Result: " + result);
	//			}
	//		}
	//		Debug.Log ("AndroidPlugin_Test_005() End.");
	//	}
	#endif
}
#endif
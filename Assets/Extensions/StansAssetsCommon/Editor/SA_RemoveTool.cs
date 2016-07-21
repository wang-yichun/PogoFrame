using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using System.Collections.Generic;


public class SA_RemoveTool  {
	
	
	public static void RemoveOneSignal() {
		RemoveNativeFileIOS("libOneSignal");
		RemoveNativeFileIOS("OneSignal");
		RemoveNativeFileIOS("OneSignalUnityRuntime");
		SA_FileStaticAPI.DeleteFolder("StansAssetsCommon/OneSignal");
	}
	
	
	
	public static void RemovePlugins() {
		
		int option = EditorUtility.DisplayDialogComplex(
			"Remove Stans Asets Plugins",
			"Following plugins wiil be removed:\n" + SA_VersionsManager.InstalledPluginsList,
			"Remove",
			"Cancel",
			"Documentation");
		
		
		switch(option) {
		case 0:
			ProcessRemove();
			break;
			
		case 2:
			string url = "https://goo.gl/CCBFIZ";
			Application.OpenURL(url);
			break;
		}
		
	}
	
	
	
	private static void ProcessRemove() {
		SA_FileStaticAPI.DeleteFolder ("Extensions/AllDocumentation");
		SA_FileStaticAPI.DeleteFolder ("Extensions/FlashLikeEvents");
		SA_FileStaticAPI.DeleteFolder ("Extensions/AndroidManifestManager");
		SA_FileStaticAPI.DeleteFolder ("Extensions/GooglePlayCommon");
		SA_FileStaticAPI.DeleteFolder ("Extensions/StansAssetsCommon");
		SA_FileStaticAPI.DeleteFolder ("Extensions/StansAssetsPreviewUI");
		SA_FileStaticAPI.DeleteFolder ("Extensions/IOSDeploy");

		
		if (SA_VersionsManager.Is_AN_Installed) {
			SA_FileStaticAPI.DeleteFolder ("Extensions/AndroidNative");
			RemoveAndroidPart();	
		}
		
		
		if (SA_VersionsManager.Is_MSP_Installed){
			SA_FileStaticAPI.DeleteFolder ("Extensions/MobileSocialPlugin");
			RemoveIOSPart();
			RemoveAndroidPart();
		}
		
		
		if (SA_VersionsManager.Is_GMA_Installed){
			SA_FileStaticAPI.DeleteFolder ("Extensions/GoogleMobileAd");
			RemoveIOSPart();
			RemoveAndroidPart();
			RemoveWP8Part();
		}
		
		
		
		if (SA_VersionsManager.Is_ISN_Installed){
			SA_FileStaticAPI.DeleteFolder("Extensions/IOSNative");
			RemoveIOSPart();
		}
		
		
		if (SA_VersionsManager.Is_UM_Installed){
			SA_FileStaticAPI.DeleteFolder("Extensions/UltimateMobile");
			SA_FileStaticAPI.DeleteFolder("Extensions/WP8Native");
			SA_FileStaticAPI.DeleteFolder("WebPlayerTemplates");
			SA_FileStaticAPI.DeleteFolder("Extensions/GoogleAnalytics");
			SA_FileStaticAPI.DeleteFolder("Extensions/MobileNativePopUps");
			
			RemoveWP8Part();
			RemoveIOSPart();
			RemoveAndroidPart();
		}
		
		
		SA_FileStaticAPI.DeleteFolder ("Plugins/StansAssets");
		AssetDatabase.Refresh();
		
		
		EditorUtility.DisplayDialog("Plugins Removed", "Unity Editor relaunch required.", "Okay");
	}
	
	
	
	
	
	private static void RemoveAndroidPart() {
		SA_FileStaticAPI.DeleteFile(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "androidnative.jar");
		SA_FileStaticAPI.DeleteFile(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "mobilenativepopups.jar");

		SA_FileStaticAPI.DeleteFolder (PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "libs");
	}
	
	
	private static void RemoveWP8Part() {
		SA_FileStaticAPI.DeleteFile ("Plugins/WP8/GoogleAds.dll");
		SA_FileStaticAPI.DeleteFile ("Plugins/WP8/GoogleAds.xml");
		SA_FileStaticAPI.DeleteFile ("Plugins/WP8/MockIAPLib.dll");
		SA_FileStaticAPI.DeleteFile ("Plugins/WP8/WP8Native.dll");
		SA_FileStaticAPI.DeleteFile ("Plugins/WP8/WP8PopUps.dll");
		SA_FileStaticAPI.DeleteFile ("Plugins/WP8/GoogleAdsWP8.dll");
		SA_FileStaticAPI.DeleteFile ("Plugins/GoogleAdsWP8.dll");
		SA_FileStaticAPI.DeleteFile ("Plugins/Metro/WP8Native.dll");
		SA_FileStaticAPI.DeleteFile ("Plugins/Metro/WP8PopUps.dll");
	}
	
	
	private static void RemoveIOSPart() {
		//TODO просмотреть не забыли ли чего лучге смотреть в УМ
		
		//ISN
		RemoveNativeFileIOS("AppEventListener");
		RemoveNativeFileIOS("CloudManager");
		RemoveNativeFileIOS("CustomBannerView");
		RemoveNativeFileIOS("iAdBannerController");
		RemoveNativeFileIOS("iAdBannerObject");
		RemoveNativeFileIOS("InAppPurchaseManager");
		RemoveNativeFileIOS("IOSNativeNotificationCenter");
		RemoveNativeFileIOS("ISN_GameCenterListner");
		RemoveNativeFileIOS("ISN_GameCenterManager");
		RemoveNativeFileIOS("ISN_GameCenter");
		RemoveNativeFileIOS("ISN_Media");
		RemoveNativeFileIOS("ISN_iAd");
		RemoveNativeFileIOS("ISN_InApp");
		RemoveNativeFileIOS("ISN_NativePopUpsManager");
		RemoveNativeFileIOS("ISN_NativeUtility");
		RemoveNativeFileIOS("ISN_NSData+Base64");
		RemoveNativeFileIOS("ISN_Reachability");
		RemoveNativeFileIOS("ISN_Security");
		RemoveNativeFileIOS("ISN_Camera");
		RemoveNativeFileIOS("ISN_ReplayKit");
		RemoveNativeFileIOS("ISN_SocialGate");
		RemoveNativeFileIOS("ISN_NativeCore");
		RemoveNativeFileIOS("ISNDataConvertor");
		RemoveNativeFileIOS("ISNSharedApplication");
		RemoveNativeFileIOS("ISNVideo");
		RemoveNativeFileIOS("SKProduct+LocalizedPrice");
		RemoveNativeFileIOS("SocialGate");
		RemoveNativeFileIOS("StoreProductView");
		RemoveNativeFileIOS("TransactionServer");
		
		
		//UM
		RemoveNativeFileIOS("UM_IOS_INSTALATION_MARK");
		
		//GMA
		RemoveNativeFileIOS("GoogleMobileAdBanner");
		RemoveNativeFileIOS("GoogleMobileAdController");
		
		//MPS
		RemoveNativeFileIOS("IOSInstaPlugin");
		RemoveNativeFileIOS("IOSTwitterPlugin");
		RemoveNativeFileIOS("MGInstagram");
		
		
		RemoveOneSignal();
	}
	
	
	
	
	
	
	private static void RemoveNativeFileIOS(string filename) {
		string filePath = PluginsInstalationUtil.IOS_DESTANATION_PATH  + filename;
		
		SA_FileStaticAPI.DeleteFile (filePath + ".h");
		SA_FileStaticAPI.DeleteFile (filePath + ".m");
		SA_FileStaticAPI.DeleteFile (filePath + ".mm");
		SA_FileStaticAPI.DeleteFile (filePath + ".a");
		SA_FileStaticAPI.DeleteFile (filePath + ".txt");
		
	}
	
}

using UnityEngine;
using UnityEditor;
using System.Collections;

public class PluginsInstalationUtil : MonoBehaviour {
	
	
	public const string ANDROID_SOURCE_PATH       = "Plugins/StansAssets/Android/";
	public const string ANDROID_DESTANATION_PATH  = "Plugins/Android/";
	
	
	public const string IOS_SOURCE_PATH       = "Plugins/StansAssets/IOS/";
	public const string IOS_DESTANATION_PATH  = "Plugins/IOS/";
	
	
	
	
	
	public static void IOS_UpdatePlugin() {
		IOS_InstallPlugin(false);
	}
	
	public static void IOS_InstallPlugin(bool IsFirstInstall = true) {
		
		IOS_CleanUp();
		
		
		
		
		
		//IOS Native
		SA_FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_Camera.mm.txt", 				PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_Camera.mm");
		SA_FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_GameCenter.mm.txt", 			PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_GameCenter.mm");
		SA_FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_iAd.mm.txt", 					PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_iAd.mm");
		SA_FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_InApp.mm.txt", 					PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_InApp.mm");
		SA_FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_Media.mm.txt", 					PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_Media.mm");
		SA_FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_ReplayKit.mm.txt", 				PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_ReplayKit.mm");
		SA_FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_GestureRecognizer.mm.txt", 		PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_GestureRecognizer.mm");
		SA_FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_CloudKit.mm.txt", 				PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_CloudKit.mm");
		SA_FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_NSData+Base64.h.txt", 			PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_NSData+Base64.h");
		SA_FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_NSData+Base64.m.txt", 			PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_NSData+Base64.m");
		
		
		IOS_Install_SocialPart();
		InstallGMAPart();
		
		
		
	}
	
	public static void InstallGMAPart() {
		//GMA
		SA_FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "GMA_SA_Lib_Proxy.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "GMA_SA_Lib_Proxy.mm");
		SA_FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "GMA_SA_Lib.h.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "GMA_SA_Lib.h");
		SA_FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "GMA_SA_Lib.m.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "GMA_SA_Lib.m");
		
	}
	
	
	public static void IOS_Install_SocialPart() {
		//IOS Native +  MSP
		SA_FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_SocialGate.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_SocialGate.mm");
		SA_FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_NativeCore.h.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_NativeCore.h");
		SA_FileStaticAPI.CopyFile(PluginsInstalationUtil.IOS_SOURCE_PATH + "ISN_NativeCore.mm.txt", 	PluginsInstalationUtil.IOS_DESTANATION_PATH + "ISN_NativeCore.mm");
	}
	
	
	
	
	public static void Remove_FB_SDK_WithDialog() {
		bool result = EditorUtility.DisplayDialog(
			"Removing Facebook SDK",
			"Are you sure you want to remove Facebook OAuth API?",
			"Remove",
			"Cansel");
		
		if(result) {
			Remove_FB_SDK();
		}
	}
	public static void Remove_FB_SDK() {
		
		SA_FileStaticAPI.DeleteFolder(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "facebook");
		SA_FileStaticAPI.DeleteFolder("Plugins/facebook", false);
		SA_FileStaticAPI.DeleteFolder("Facebook", false);
		SA_FileStaticAPI.DeleteFolder("FacebookSDK", false);
		
		//MSP
		SA_FileStaticAPI.DeleteFile("Extensions/MobileSocialPlugin/Example/Scripts/MSPFacebookUseExample.cs", false);
		SA_FileStaticAPI.DeleteFile("Extensions/MobileSocialPlugin/Example/Scripts/MSP_FacebookAnalyticsExample.cs", false);
		SA_FileStaticAPI.DeleteFile("Extensions/MobileSocialPlugin/Example/Scripts/MSP_FacebookAndroidTurnBasedAndGiftsExample.cs", false);
		
		//FB v7
		SA_FileStaticAPI.DeleteFolder("Examples", false);
		SA_FileStaticAPI.DeleteFolder(PluginsInstalationUtil.IOS_DESTANATION_PATH + "Facebook", false);
		
		
		SA_FileStaticAPI.DeleteFolder(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "libs/bolts-android-1.2.0.jar");
		SA_FileStaticAPI.DeleteFolder(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "libs/facebook-android-sdk-4.7.0.jar");
		SA_FileStaticAPI.DeleteFolder(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "libs/facebook-android-wrapper-release.jar");
		
		AssetDatabase.Refresh();
	}
	
	
	private static string AN_SoomlaGrowContent = "Extensions/AndroidNative/Other/Soomla/AN_SoomlaGrow.cs";
	public static void DisableSoomlaFB() {
		ChnageDefineState(AN_SoomlaGrowContent, "FACEBOOK_ENABLED", false);
	}
	
	
	
	
	
	private static void ChnageDefineState(string file, string tag, bool IsEnabled) {
		
		if(!SA_FileStaticAPI.IsFileExists(file)) {
			Debug.Log("ChnageDefineState for tag: " + tag + " File not found at path: " + file);
			return;
		}
		
		string content = SA_FileStaticAPI.Read(file);
		
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
		
		SA_FileStaticAPI.Write(file, content);
		
	}
	
	
	public static void IOS_CleanUp() {
		
		
		//Old APi
		RemoveIOSFile("AppEventListener");
		RemoveIOSFile("CloudManager");
		RemoveIOSFile("CustomBannerView");
		RemoveIOSFile("GameCenterManager");
		RemoveIOSFile("GCHelper");
		RemoveIOSFile("iAdBannerController");
		RemoveIOSFile("iAdBannerObject");
		RemoveIOSFile("InAppPurchaseManager");
		RemoveIOSFile("IOSGameCenterManager");
		RemoveIOSFile("IOSNativeNotificationCenter");
		RemoveIOSFile("IOSNativePopUpsManager");
		RemoveIOSFile("IOSNativeUtility");
		RemoveIOSFile("ISN_NSData+Base64");
		RemoveIOSFile("ISN_Reachability");
		RemoveIOSFile("ISNCamera");
		RemoveIOSFile("ISNDataConvertor");
		RemoveIOSFile("ISNSharedApplication");
		RemoveIOSFile("ISNVideo");
		RemoveIOSFile("PopUPDelegate");
		RemoveIOSFile("RatePopUPDelegate");
		RemoveIOSFile("SKProduct+LocalizedPrice");
		RemoveIOSFile("SocialGate");
		RemoveIOSFile("StoreProductView");
		RemoveIOSFile("TransactionServer");
		
		RemoveIOSFile("OneSignalUnityRuntime");
		RemoveIOSFile("OneSignal");
		RemoveIOSFile("libOneSignal");
		RemoveIOSFile("ISN_Security");
		RemoveIOSFile("ISN_NativeUtility");
		RemoveIOSFile("ISN_NativePopUpsManager");
		RemoveIOSFile("ISN_Media");
		RemoveIOSFile("ISN_GameCenterTBM");
		RemoveIOSFile("ISN_GameCenterRTM");
		RemoveIOSFile("ISN_GameCenterManager");
		RemoveIOSFile("ISN_GameCenterListner");
		RemoveIOSFile("IOSNativeNotificationCenter");
		
		
		
		//New API
		RemoveIOSFile("ISN_Camera");
		RemoveIOSFile("ISN_GameCenter");
		RemoveIOSFile("ISN_InApp");
		RemoveIOSFile("ISN_iAd");
		RemoveIOSFile("ISN_NativeCore");
		RemoveIOSFile("ISN_SocialGate");
		RemoveIOSFile("ISN_ReplayKit");
		RemoveIOSFile("ISN_CloudKit");
		RemoveIOSFile("ISN_Soomla");
		RemoveIOSFile("ISN_GestureRecognizer");
		
		
		
		//Google Ad old v1
		RemoveIOSFile("GADAdMobExtras");
		RemoveIOSFile("GADAdNetworkExtras");
		RemoveIOSFile("GADAdSize");
		RemoveIOSFile("GADBannerViewDelegate");
		RemoveIOSFile("GADInAppPurchase");
		RemoveIOSFile("GADInAppPurchaseDelegate");
		RemoveIOSFile("GADInterstitialDelegate");
		RemoveIOSFile("GADModules");
		RemoveIOSFile("GADRequest");
		RemoveIOSFile("GADRequestError");
		RemoveIOSFile("libGoogleAdMobAds");
		
		//Google Ad old v2
		RemoveIOSFile("GoogleMobileAdBanner");
		RemoveIOSFile("GoogleMobileAdController");
		
		
		//Google Ad new
		RemoveIOSFile("GMA_SA_Lib");
		
		
		//MSP old
		RemoveIOSFile("IOSInstaPlugin");
		RemoveIOSFile("IOSTwitterPlugin");
		RemoveIOSFile("MGInstagram");
		
		
		
		
		
	}
	
	
	public static void RemoveIOSFile(string filename) {
		SA_FileStaticAPI.DeleteFile(IOS_DESTANATION_PATH + filename + ".h");
		SA_FileStaticAPI.DeleteFile(IOS_DESTANATION_PATH + filename + ".m");
		SA_FileStaticAPI.DeleteFile(IOS_DESTANATION_PATH + filename + ".mm");
		SA_FileStaticAPI.DeleteFile(IOS_DESTANATION_PATH + filename + ".a");
	}
	
	
	public static void Android_UpdatePlugin() {
		Android_InstallPlugin(false);
	}
	
	
	
	public static void EnableGooglePlayAPI() {
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/an_googleplay.txt", 			ANDROID_DESTANATION_PATH + "libs/an_googleplay.aar");
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-base.txt", 		ANDROID_DESTANATION_PATH + "libs/play-services-base.aar");
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-basement.txt", 	ANDROID_DESTANATION_PATH + "libs/play-services-basement.aar");
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-ads-lite.txt", 	ANDROID_DESTANATION_PATH + "libs/play-services-ads-lite.aar");
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-games.txt", 		ANDROID_DESTANATION_PATH + "libs/play-services-games.aar");
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-gcm.txt", 		ANDROID_DESTANATION_PATH + "libs/play-services-gcm.aar");
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-plus.txt", 		ANDROID_DESTANATION_PATH + "libs/play-services-plus.aar");
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-appinvite.txt", 	ANDROID_DESTANATION_PATH + "libs/play-services-appinvite.aar");
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-analytics.txt", 	ANDROID_DESTANATION_PATH + "libs/play-services-analytics.aar");
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-auth.txt", 		ANDROID_DESTANATION_PATH + "libs/play-services-auth.aar");
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-auth-base.txt", 	ANDROID_DESTANATION_PATH + "libs/play-services-auth-base.aar");
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-drive.txt", 		ANDROID_DESTANATION_PATH + "libs/play-services-drive.aar");
	}
	
	public static void DisableGooglePlayAPI() {
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/an_googleplay.aar");
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-base.aar");
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-basement.aar");
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-ads-lite.aar");
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-games.aar");
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-gcm.aar");
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-plus.aar");
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-appinvite.aar");
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-analytics.aar");
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-auth.aar");
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-auth-base.aar");
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-drive.aar");
	}

	public static void EnableDriveAPI() {
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-drive.txt", 		ANDROID_DESTANATION_PATH + "libs/play-services-drive.aar");
	}

	public static void DisableDriveAPI() {
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-drive.aar");
	}

	public static void EnableOAuthAPI() {
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-auth.txt", 		ANDROID_DESTANATION_PATH + "libs/play-services-auth.aar");
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-auth-base.txt", 	ANDROID_DESTANATION_PATH + "libs/play-services-auth-base.aar");
	}

	public static void DisableOAuthAPI(){
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-auth.aar");
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-auth-base.aar");
	}

	public static void EnableAnalyticsAPI() {
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-analytics.txt", 	ANDROID_DESTANATION_PATH + "libs/play-services-analytics.aar");
	}

	public static void DisableAnalyticsAPI() {
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-analytics.aar");
	}

	public static void EnableAppInvitesAPI() {
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-appinvite.txt", 	ANDROID_DESTANATION_PATH + "libs/play-services-appinvite.aar");
	}

	public static void DisableAppInvitesAPI() {
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-appinvite.aar");
	}

	public static void EnableGooglePlusAPI() {
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-plus.txt", 		ANDROID_DESTANATION_PATH + "libs/play-services-plus.aar");
	}

	public static void DisableGooglePlusAPI() {
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-plus.aar");
	}

	public static void EnablePushNotificationsAPI() {
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-gcm.txt", 		ANDROID_DESTANATION_PATH + "libs/play-services-gcm.aar");
	}

	public static void DisablePushNotificationsAPI() {
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-gcm.aar");
	}

	public static void EnableGoogleAdMobAPI() {
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-ads-lite.txt", 	ANDROID_DESTANATION_PATH + "libs/play-services-ads-lite.aar");
	}

	public static void DisableGoogleAdMobAPI() {
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-ads-lite.aar");
	}

	public static void EnableGooglePlayServicesAPI () {
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "google_play/play-services-games.txt", 		ANDROID_DESTANATION_PATH + "libs/play-services-games.aar");
	}

	public static void DisableGooglePlayServicesAPI() {
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/play-services-games.aar");
	}
	
	public static void EnableAndroidCampainAPI() {
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "libs/sa_analytics.txt", 	ANDROID_DESTANATION_PATH + "libs/sa_analytics.jar");
	}
	
	
	public static void DisableAndroidCampainAPI() {
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/sa_analytics.jar");
	}
	
	
	public static void EnableAppLicensingAPI() {
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "app_licensing/an_licensing_library.txt", 	ANDROID_DESTANATION_PATH + "libs/an_licensing_library.jar");
	}
	
	
	public static void DisableAppLicensingAPI() {
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/an_licensing_library.jar");
	}
	
	
	public static void EnableSoomlaAPI() {
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "libs/an_sa_soomla.txt", 	ANDROID_DESTANATION_PATH + "libs/an_sa_soomla.jar");
	}
	
	
	public static void DisableSoomlaAPI() {
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/an_sa_soomla.jar");
	}
	
	
	
	public static void EnableBillingAPI() {
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "billing/an_billing.txt", 	ANDROID_DESTANATION_PATH + "libs/an_billing.aar");
	}
	
	public static void DisableBillingAPI() {
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/an_billing.aar");
	}
	
	
	
	
	public static void EnableSocialAPI() {
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "social/an_social.txt", 	ANDROID_DESTANATION_PATH + "libs/an_social.aar");
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "social/twitter4j-core-4.0.4.txt", 	ANDROID_DESTANATION_PATH + "libs/twitter4j-core-4.0.4.jar");
	}
	
	public static void DisableSocialAPI() {
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/an_social.aar");
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/twitter4j-core-4.0.4.jar");
	}
	
	
	
	
	
	
	public static void EnableCameraAPI() {
		//Unity 5 upgdare:
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "libs/image-chooser-library-1.6.0.txt", 	ANDROID_DESTANATION_PATH + "libs/image-chooser-library-1.6.0.jar");
	}
	
	public static void DisableCameraAPI() {
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/image-chooser-library-1.6.0.jar");
	}
	
	
	
	
	
	public static void Android_InstallPlugin(bool IsFirstInstall = true) {
		
		
		//Unity 5 upgdare:
		SA_FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/httpclient-4.3.1.jar");
		SA_FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/signpost-commonshttp4-1.2.1.2.jar");
		SA_FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/signpost-core-1.2.1.2.jar");
		SA_FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/libGoogleAnalyticsServices.jar");
		
		SA_FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/android-support-v4.jar");

		//Remove previous Image Chooser Library version
		SA_FileStaticAPI.DeleteFile(ANDROID_DESTANATION_PATH + "libs/image-chooser-library-1.3.0.jar");
		SA_FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/image-chooser-library-1.6.0.jar");

		SA_FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/twitter4j-core-3.0.5.jar");
		SA_FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "libs/google-play-services.jar");
		
		
		SA_FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "social/an_social.jar");
		SA_FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "social/twitter4j-core-3.0.5.jar");
		
		
		SA_FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "google_play/an_googleplay.jar");
		SA_FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "google_play/google-play-services.jar");
		
		SA_FileStaticAPI.DeleteFile(ANDROID_SOURCE_PATH + "billing/an_billing.jar");
		
		
		
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "libs/support-v4-23.4.0.txt", 	ANDROID_DESTANATION_PATH + "libs/support-v4-23.4.0.aar");
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "androidnative.txt", 	        	ANDROID_DESTANATION_PATH + "androidnative.aar");
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + "sa_analytics.txt", 	        	ANDROID_DESTANATION_PATH + "sa_analytics.jar");
		
		SA_FileStaticAPI.CopyFile (ANDROID_SOURCE_PATH + "mobile-native-popups.txt",             ANDROID_DESTANATION_PATH + "mobile-native-popups.aar");
		SA_FileStaticAPI.DeleteFile (ANDROID_SOURCE_PATH + "mobilenativepopups.txt");
		SA_FileStaticAPI.DeleteFile (ANDROID_DESTANATION_PATH + "mobilenativepopups.jar");
		
		SA_FileStaticAPI.CopyFolder(ANDROID_SOURCE_PATH + "facebook", 			ANDROID_DESTANATION_PATH + "facebook");
		
		#if UNITY_3_5 || UNITY_4_0 || UNITY_4_1	|| UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6
		
		#else
		SA_FileStaticAPI.DeleteFolder(ANDROID_SOURCE_PATH + "facebook");
		#endif
		
		if(IsFirstInstall) {
			EnableBillingAPI();
			EnableGooglePlayAPI();
			EnableSocialAPI();
			EnableCameraAPI();
			EnableAppLicensingAPI();
		}
		
		
		
		
		string file;
		file = "AN_Res/res/values/analytics.xml";
		if(!SA_FileStaticAPI.IsFileExists(ANDROID_DESTANATION_PATH + file)) {
			SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file, 	ANDROID_DESTANATION_PATH + file);
		}
		
		
		file = "AN_Res/res/values/ids.xml";
		if(!SA_FileStaticAPI.IsFileExists(ANDROID_DESTANATION_PATH + file)) {
			SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file, 	ANDROID_DESTANATION_PATH + file);
		}
		
		file = "AN_Res/res/xml/file_paths.xml";
		if(!SA_FileStaticAPI.IsFileExists(ANDROID_DESTANATION_PATH + file)) {
			SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file, 	ANDROID_DESTANATION_PATH + file);
		}
		
		
		file = "AN_Res/res/values/version.xml";
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file, 	ANDROID_DESTANATION_PATH + file);
		
		file = "AN_Res/project.properties";
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file, 	ANDROID_DESTANATION_PATH + file);
		
		file = "AN_Res/AndroidManifest";
		SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file + ".txt", 	ANDROID_DESTANATION_PATH + file + ".xml");
		
		//First install dependense		
		
		file = "AndroidManifest";
		if(!SA_FileStaticAPI.IsFileExists(ANDROID_DESTANATION_PATH + file)) {
			SA_FileStaticAPI.CopyFile(ANDROID_SOURCE_PATH + file + ".txt", 	ANDROID_DESTANATION_PATH + file + ".xml");
		} 
		
		AssetDatabase.Refresh();
		
	}
	
	
	
	public static bool IsFacebookInstalled {
		get {
			return SA_FileStaticAPI.IsFileExists("Facebook/Scripts/FB.cs") || SA_FileStaticAPI.IsFileExists("FacebookSDK/SDK/Scripts/FB.cs");
		}
	}
	
	
}

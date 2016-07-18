#define INAPP_API_ENABLED

////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////
 


using UnityEngine;
using System.Collections;
#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class IOSNativeMarketBridge  {

	#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _loadStore(string ids);
	
	[DllImport ("__Internal")]
	private static extern void _restorePurchases();
	
	
	[DllImport ("__Internal")]
	private static extern void _buyProduct(string id);


	[DllImport ("__Internal")]
	private static extern bool _ISN_InAppSettingState();
	
	[DllImport ("__Internal")]
	private static extern void _verifyLastPurchase(string url);



	//SKCloudServiceController


	[DllImport ("__Internal")]
	private static extern int ISN_SKCloudService_AuthorizationStatus();

	[DllImport ("__Internal")]
	private static extern void ISN_SKCloudService_RequestAuthorization();

	[DllImport ("__Internal")]
	private static extern void ISN_SKCloudService_RequestCapabilities();


	[DllImport ("__Internal")]
	private static extern void ISN_SKCloudService_RequestStorefrontIdentifier();



	#endif


	public static void loadStore(string ids) {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
			_loadStore(ids);
		#endif
	}
	
	public static void buyProduct(string id) {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
			_buyProduct(id);
		#endif
	}
	
	public static void restorePurchases() {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
			_restorePurchases();
		#endif
	}
	
	public static void verifyLastPurchase(string url) {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
			_verifyLastPurchase(url);
		#endif
	}


	public static bool ISN_InAppSettingState() {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
		return _ISN_InAppSettingState();
		#else
		return false;
		#endif
	}


	//--------------------------------------
	//  SKCloudServiceController
	//--------------------------------------





	public static int CloudService_AuthorizationStatus()  {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
		return ISN_SKCloudService_AuthorizationStatus();
		#else
		return 0;
		#endif
	}


	public static void CloudService_RequestAuthorization() {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
		ISN_SKCloudService_RequestAuthorization();
		#endif
	}


	public static void CloudService_RequestCapabilities() {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
		ISN_SKCloudService_RequestCapabilities();
		#endif
	}



	public static void CloudService_RequestStorefrontIdentifier() {
		#if ( (UNITY_IPHONE || UNITY_TVOS)  && !UNITY_EDITOR && INAPP_API_ENABLED) || SA_DEBUG_MODE
		ISN_SKCloudService_RequestStorefrontIdentifier();
		#endif
	}

}


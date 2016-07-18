//#define GESTURE_API
////////////////////////////////////////////////////////////////////////////////
//  
// @module IOS Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;


#if ( (UNITY_TVOS || UNITY_IPHONE)   && !UNITY_EDITOR  && GESTURE_API) || SA_DEBUG_MODE
using System.Runtime.InteropServices;
#endif

public class ISN_GestureRecognizer : ISN_Singleton<ISN_GestureRecognizer> {


	#if ( (UNITY_TVOS || UNITY_IPHONE)  && !UNITY_EDITOR && GESTURE_API) || SA_DEBUG_MODE
	[DllImport ("__Internal")]
	private static extern void _ISN_InitTvOsGestureRecognizer();
	#endif

	public event Action<ISN_SwipeDirection> OnSwipe = delegate {};

	void Awake() {
		#if ( (UNITY_TVOS || UNITY_IPHONE)  && !UNITY_EDITOR && GESTURE_API) || SA_DEBUG_MODE
		 _ISN_InitTvOsGestureRecognizer();
		#endif
	}

	private void OnSwipeAction(string data) {
		int val = System.Convert.ToInt32(data);
		OnSwipe((ISN_SwipeDirection) val);
	} 

}

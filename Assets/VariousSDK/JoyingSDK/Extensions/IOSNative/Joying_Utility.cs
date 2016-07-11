//#define SA_DEBUG_MODE

namespace pogorock.Joying
{

	using UnityEngine;
	using System;
	using System.Collections;

	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	using System.Runtime.InteropServices;
	#endif



	public class Joying_Utility : ISN_Singleton<IOSNativeUtility>
	{
		
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	
		[DllImport ("__Internal")]
		private static extern void init ();

		[DllImport ("__Internal")]
		private static extern void initAppID (string appId, string appKey);


		void Awake ()
		{
			DontDestroyOnLoad (gameObject);
			init ();
		}

		void Init ()
		{
			init ();
		}

		void InitAppID (string appId, string appKey)
		{
			initAppID (appId, appKey);
		}

		#endif
	}

}
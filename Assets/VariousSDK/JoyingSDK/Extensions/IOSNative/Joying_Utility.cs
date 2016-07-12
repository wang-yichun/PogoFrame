﻿#define SA_DEBUG_MODE

namespace pogorock.Joying
{

	using UnityEngine;
	using System;
	using System.Collections;

	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	using System.Runtime.InteropServices;
	#endif



	public class Joying_Utility : Joying_Singleton<Joying_Utility>
	{
		
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	
		[DllImport ("__Internal")]
		private static extern void init ();

		[DllImport ("__Internal")]
		private static extern void initAppID (string appId, string appKey);

		[DllImport ("__Internal")]
		private static extern void videoHasCanPlayVideo ();

		[DllImport ("__Internal")]
		private static extern void videoPlay_FullScreen ();

		#endif

		void Awake ()
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			DontDestroyOnLoad (gameObject);
			init ();
			#endif
		}

		void Init ()
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			init ();
			#endif
		}

		public void InitAppID (string appId, string appKey)
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			Debug.Log ("Joying_Utility InitAppID.");
			initAppID (appId, appKey);
			#endif
		}

		public void VideoHasCanPlayVideo ()
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			videoHasCanPlayVideo ();
			#endif
		}

		public void VideoHasCanPlayVideo_Callback (string idStr)
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			Debug.Log ("videoHasCanPlayVideo_Callback: " + idStr);
			#endif
		}

		public void VideoPlay_FullScreen ()
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			videoPlay_FullScreen ();
			#endif
		}

		public void VideoPlay_Callback_isFinishPlay (string idStr)
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			Debug.Log ("videoPlay_Callback_isFinishPlay: " + idStr);
			#endif
		}

		public void VideoPlay_Callback_isLegal (string idStr)
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			Debug.Log ("videoPlay_Callback_isLegal: " + idStr);
			#endif
		}
	}
}
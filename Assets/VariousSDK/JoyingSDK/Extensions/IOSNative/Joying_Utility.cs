#define SA_DEBUG_MODE

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
		private static extern void joying_init ();

		[DllImport ("__Internal")]
		private static extern void initAppID (string appId, string appKey);

		[DllImport ("__Internal")]
		private static extern void videoHasCanPlayVideo ();

		[DllImport ("__Internal")]
		private static extern void videoPlay_FullScreen ();

		[DllImport ("__Internal")]
		private static extern void videoPlay_CustomRect ();

		#region delegate

		public delegate void VideoPlayCallback_IsFinishPlay (bool isFinishPlay);

		public delegate void VideoPlayCallback_IsLegal (bool isLegal);

		public static event VideoPlayCallback_IsFinishPlay OnVideoPlayCallback_IsFinishPlay;
		public static event VideoPlayCallback_IsLegal OnVideoPlayCallback_IsLegal;

		#endregion

		#endif

		void Awake ()
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			DontDestroyOnLoad (gameObject);
			joying_init ();
			#endif
		}

		void Init ()
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			joying_init ();
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

		public void VideoPlay_CustomRect ()
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			videoPlay_CustomRect ();
			#endif
		}

		public void VideoPlay_Callback_isFinishPlay (string value)
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			Debug.Log ("videoPlay_Callback_isFinishPlay: " + value);
			if (value.ToLower () == "yes") {
				OnVideoPlayCallback_IsFinishPlay.Invoke (true);
			} else if (value.ToLower () == "no") {
				OnVideoPlayCallback_IsFinishPlay.Invoke (false);
			}
			#endif
		}

		public void VideoPlay_Callback_isLegal (string value)
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			if (value.ToLower () == "yes") {
				OnVideoPlayCallback_IsLegal.Invoke (true);
			} else if (value.ToLower () == "no") {
				OnVideoPlayCallback_IsLegal.Invoke (false);
			}
			#endif
		}
	}
}
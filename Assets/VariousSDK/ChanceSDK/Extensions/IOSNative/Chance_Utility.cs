//#define SA_DEBUG_MODE

namespace pogorock.ChanceAd
{

	using UnityEngine;
	using System;
	using System.Collections;

	#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	using System.Runtime.InteropServices;
	#endif



	public class Chance_Utility : Chance_Singleton<Chance_Utility>
	{
		public string chance_publisherID = "879722473-2D6F82-FB79-BB4C-E89B3FC08";
		#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
	
		[DllImport ("__Internal")]
		private static extern void init ();

		[DllImport ("__Internal")]
		private static extern void publisherID (string publisherID);

		//[DllImport ("__Internal")]
		//private static extern void videoHasCanPlayVideo ();

		[DllImport ("__Internal")]
		private static extern void loadCSVideoAD ();

		[DllImport ("__Internal")]
		private static extern void playVideoAD ();
		[DllImport ("__Internal")]
		private static extern void queryVideoAD ();
		

#region delegate

		
		public delegate void VideoPlayCallback_IsFinishPlay (bool isFinishPlay);
		public static event VideoPlayCallback_IsFinishPlay OnVideoPlayCallback_IsFinishPlay;
		public delegate void VideoLoadCallback_IsComplete (bool loadComplete);
		public static event VideoLoadCallback_IsComplete OnVideoLoadCallback_IsComplete;
		public delegate void VideoHasVideoCallback_IsHas (bool isHas);
		public static event VideoHasVideoCallback_IsHas OnVideoHasVideoCallback_IsHas;
		public delegate void RequestVideoADCallback_IsSuccessed (bool isSuccessed);
		public static event RequestVideoADCallback_IsSuccessed OnRequestVideoADCallback_IsSuccessed;
		public delegate void VideoPlayCompleteCallBack_IsComplete (bool isComplete);
		public static event VideoPlayCompleteCallBack_IsComplete OnVideoPlayCompleteCallBack_IsComplete;

		

#endregion

		
		#endif

		void Awake ()
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			DontDestroyOnLoad (gameObject);
			Init ();
			#endif
		}

		void Init ()
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			init ();
			PublisherID(chance_publisherID);
			#endif
		}

		public void PublisherID (string publisherID)
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			Debug.Log ("Chance_Utility PublisherID.");
			this.publisherID (publisherID);
			#endif
		}

		private void VideoLoadPlayVideo_Callback (string back)
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			if (back == "yes"&&OnVideoLoadCallback_IsComplete!=null) {
				OnVideoLoadCallback_IsComplete.Invoke (true);
			} else if (back == "no"&&OnVideoLoadCallback_IsComplete!=null) {
				OnVideoLoadCallback_IsComplete.Invoke (false);
			}
			#endif
		}

		private void VideoHasCanPlayVideo_Callback (string back)
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			if (OnVideoHasVideoCallback_IsHas != null) {
				if (back == "yes") {
					OnVideoHasVideoCallback_IsHas.Invoke (true);
				} else if (back == "no") {
					OnVideoHasVideoCallback_IsHas.Invoke (false);
				}
			}
			#endif
		}

		private void RequestVideoAD_Callback (string back)
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			if (back == "yes"&&OnRequestVideoADCallback_IsSuccessed!=null) {
				OnRequestVideoADCallback_IsSuccessed.Invoke (true);
			} else  if(back=="no"&&OnRequestVideoADCallback_IsSuccessed!=null){
				OnRequestVideoADCallback_IsSuccessed.Invoke (false);
			}
			#endif
		}

		private void VideoPlayComplete_CallBack (string back)
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			if (back == "yes"&&OnVideoPlayCompleteCallBack_IsComplete!=null) {
				OnVideoPlayCompleteCallBack_IsComplete.Invoke (true);
			} else  if(back=="no"&&OnVideoPlayCompleteCallBack_IsComplete!=null){
				OnVideoPlayCompleteCallBack_IsComplete.Invoke (false);
			}
			#endif
		}

		public void GetHasVideoAD ()
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			loadCSVideoAD();
			#endif
		}

		public void LoadCSVideoAD ()
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			loadCSVideoAD();
			#endif
		}

		public void QueryVideoAD ()
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			queryVideoAD ();
			#endif
		}

		public void PlayVideoAD ()
		{
			#if (UNITY_IPHONE && !UNITY_EDITOR) || SA_DEBUG_MODE
			playVideoAD ();
			#endif
		}
	}
}
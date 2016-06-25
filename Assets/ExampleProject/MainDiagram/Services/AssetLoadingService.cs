using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uFrame.Kernel;
using uFrame.IOC;
using UniRx;
using UnityEngine;
using uFrame.MVVM;
using pogorock;
using AssetBundles;
using pogorock;

namespace uFrame.ExampleProject
{
	public class AssetLoadingService : AssetLoadingServiceBase
	{
		[Inject] public static AssetLoadingService Instance;

		/*
         * This method handles StartAssetLoadingCommand. 
         * Even though StartAssetLoadingCommand is just another event, it has a bit different name.
         * This is because we use this event to invoke asset loading procedure. Thus, Command is 
         * an appropriate name for such an event.
         */

		public override void StartAssetLoadingCommandHandler (StartAssetLoadingCommand data)
		{
			base.StartAssetLoadingCommandHandler (data);
			StartCoroutine (Init ());
		}

		/*
         * This coroutine simulates asset loading. You can substitute this method to actually load
         * something from disk/cloud/external service. Please notice, that we publish progress event.
         * So, any part of the application intrested in the progress can subscribe to this event 
         * and perform additional logic.
         */

		private IEnumerator LoadAssets ()
		{
			for (int i = 0; i < 100; i++) {
				Publish (new AssetLoadingProgressEvent () {
					Message = string.Format ("Loaded {0}% of game assets...", i),
					Progress = i / 100f
				});
				yield return new WaitForSeconds (0.03f);
			}

			/*
             * Ensure, that we publish "1f progress" event with a different message, after we finish.
             */
			Publish (new AssetLoadingProgressEvent () {
				Message = "Loaded 100% of game assets!",
				Progress = 1f
			});
		}

		IEnumerator Init ()
		{
//			yield return StartCoroutine (Initialize ());
//			yield return StartCoroutine (InitializeMulti ());

			yield return StartCoroutine (InitializeUseSettings ());

			yield return new WaitForSeconds (.1f);

			Publish (new AssetLoadingProgressEvent () {
				Message = "Loaded 100% of game assets!",
				Progress = 1f
			});

		}

		public IEnumerator InitializeUseSettings ()
		{
			for (int i = 0; i < AssetBundleSettings.Instance.loadingUrls.Count; i++) {
				AssetBundleUrl_Loading url = AssetBundleSettings.Instance.loadingUrls [i];
				if (url.Enable) {


					if (url.IsLocal) {
						string full_url = string.Format (
							                  "{0}/{1}/{2}",
							                  AssetBundleManager.GetStreamingAssetsPath (),
							                  Utility.GetPlatformName (),
							                  url.UrlId
						                  );
						AssetBundleManager.SetBaseDownloadingURL (url.UrlId, full_url);
					} else {
						string full_url = string.Format (
							                  "{0}/{1}/{2}",
							                  url.Url,
							                  Utility.GetPlatformName (),
							                  url.UrlId
						                  );
						AssetBundleManager.SetBaseDownloadingURL (url.UrlId, url.Url);
					}

					var request = AssetBundleManager.Initialize (url.UrlId, url.UrlId);
					if (request != null) {
						yield return StartCoroutine (request);

						Debug.Log ("[Loading Completed] UrlId: " + url.UrlId + "\n" + AssetBundleManager.GetBaseDownloadingURL [url.UrlId]);
					}
				}
			}
		}
		// Initialize the downloading url and AssetBundleManifest object.
		//		public IEnumerator Initialize ()
		//		{
		//			Caching.CleanCache ();
		//
		//			// With this code, when in-editor or using a development builds: Always use the AssetBundle Server
		//			// (This is very dependent on the production workflow of the project.
		//			// 	Another approach would be to make this configurable in the standalone player.)
		//			#if UNITY_EDITOR
		//
		//			if (AssetBundleLoaderSettings.Instance.useStreamingAssets) {
		//				// 在编辑器下使用
		//				AssetBundleManager.SetSourceAssetBundleURL ("file://" + Application.dataPath + "/StreamingAssets/");
		//			} else {
		//				AssetBundleManager.SetDevelopmentAssetBundleServer ();
		//			}
		//
		//			#elif DEVELOPMENT_BUILD
		//				AssetBundleManager.SetDevelopmentAssetBundleServer ();
		//			#else
		//			// Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
		//			// AssetBundleManager.SetSourceAssetBundleURL(Application.dataPath + "/");
		//			// Or customize the URL based on your deployment or configuration
		//			// AssetBundleManager.SetSourceAssetBundleURL("http://www.MyWebsite/MyAssetBundles");
		//
		//			AssetBundleManager.SetSourceAssetBundleDirectory ("/" + Utility.GetPlatformName () + "/");
		//
		//			#endif
		//
		////			string url =
		////
		////			#if UNITY_ANDROID
		////				"jar:file://" + Application.dataPath + "!/assets";
		////			#elif UNITY_IOS
		////				"file://" + Application.dataPath + "/Raw";
		////			#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
		////				"file://" + Application.dataPath + "/StreamingAssets";
		////			#else
		////				string.Empty;
		////			#endif
		////
		////			AssetBundleManager.SetSourceAssetBundleURL (url + "/");
		//
		//			Debug.Log ("AssetBundleManager BaseDownlingURL: " + AssetBundleManager.BaseDownloadingURL);
		//
		//			// Initialize AssetBundleManifest which loads the AssetBundleManifest object.
		//			var request = AssetBundleManager.Initialize ();
		//			if (request != null)
		//				yield return StartCoroutine (request);
		//		}

		//		public IEnumerator InitializeMulti ()
		//		{
		//			Caching.CleanCache ();
		//
		////			string url1 = "http://192.168.199.215:7888/";
		//			string url2 = "ftp://192.168.199.215/";
		//
		//			// first
		////			AssetBundleManager.SetSourceAssetBundleURL(url1);
		////			Debug.Log ("AssetBundleManager BaseDownlingURL: " + AssetBundleManager.BaseDownloadingURL);
		////			var request = AssetBundleManager.Initialize ();
		////			if (request != null)
		////			yield return StartCoroutine (request);
		//
		//			AssetBundleManager.BaseDownloadingURL2 = url2 + "OSX2/";
		////			AssetBundleManager.SetSourceAssetBundleURL (url2);
		//			Debug.Log ("AssetBundleManager BaseDownlingURL: " + AssetBundleManager.BaseDownloadingURL);
		//			var request2 = AssetBundleManager.Initialize (Utility.GetPlatformName () + "2", 1);
		//			if (request2 != null)
		//				yield return StartCoroutine (request2);
		//
		//			AssetBundleManager.BaseDownloadingURL = "file://" + Application.dataPath + "/StreamingAssets/OSX/";
		////			AssetBundleManager.SetSourceAssetBundleURL ("file://" + Application.dataPath + "/StreamingAssets/");
		//			Debug.Log ("AssetBundleManager BaseDownlingURL: " + AssetBundleManager.BaseDownloadingURL);
		//			var request1 = AssetBundleManager.Initialize (Utility.GetPlatformName (), 0);
		//			if (request1 != null)
		//				yield return StartCoroutine (request1);
		//		}
	}
}
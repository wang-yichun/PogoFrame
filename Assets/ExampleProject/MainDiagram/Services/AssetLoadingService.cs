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
//			StartCoroutine (LoadAssets ());
			StartDownloadTargetPaths ();
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

		void StartDownloadTargetPaths ()
		{
			var targetPaths = AssetBundleLoaderSettings.Instance.targetPaths;
			for (int i = 0; i < targetPaths.Count; i++) {
				var targetPath = targetPaths [i];
				if (targetPath.enable) {
					string url = string.Format ("{0}", "file://" + targetPath.targetPath);
					StartCoroutine (DownloadAndCache (url, 2));
					break;
				}
			}
		}

		IEnumerator DownloadAndCache (string bundleURL, int version)
		{
			// Wait for the Caching system to be ready
			while (!Caching.ready)
				yield return null;

			// Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
			using (WWW www = WWW.LoadFromCacheOrDownload (bundleURL, version)) {
				yield return www;
				if (www.error != null)
					throw new Exception ("WWW download had an error:" + www.error);
				currentAssetBundle = www.assetBundle;

//				if (AssetName == "")
//					Instantiate (bundle.mainAsset);
//				else
//					Instantiate (bundle.LoadAsset (AssetName));
//				// Unload the AssetBundles compressed contents to conserve memory

//				bundle.Unload (false);


				yield return new WaitForSeconds (.1f);
				Publish (new AssetLoadingProgressEvent () {
					Message = "Loaded 100% of game assets!",
					Progress = 1f
				});

			} // memory is freed from the web stream (www.Dispose() gets called implicitly)
		}

		public AssetBundle currentAssetBundle;
	}
}
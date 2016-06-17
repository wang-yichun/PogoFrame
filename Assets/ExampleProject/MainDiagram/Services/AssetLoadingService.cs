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
			yield return StartCoroutine (Initialize ());

			Publish (new AssetLoadingProgressEvent () {
				Message = "Loaded 100% of game assets!",
				Progress = 1f
			});

		}

		// Initialize the downloading url and AssetBundleManifest object.
		protected IEnumerator Initialize ()
		{
			// Don't destroy this gameObject as we depend on it to run the loading script.
			DontDestroyOnLoad (gameObject);

			// With this code, when in-editor or using a development builds: Always use the AssetBundle Server
			// (This is very dependent on the production workflow of the project. 
			// 	Another approach would be to make this configurable in the standalone player.)
			#if DEVELOPMENT_BUILD || UNITY_EDITOR
			AssetBundleManager.SetDevelopmentAssetBundleServer ();
			#else
			// Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
			AssetBundleManager.SetSourceAssetBundleURL(Application.dataPath + "/");
			// Or customize the URL based on your deployment or configuration
			//AssetBundleManager.SetSourceAssetBundleURL("http://www.MyWebsite/MyAssetBundles");
			#endif

			// Initialize AssetBundleManifest which loads the AssetBundleManifest object.
			var request = AssetBundleManager.Initialize ();
			if (request != null)
				yield return StartCoroutine (request);
		}
	}
}
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

using pogorock.Joying;

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

			Joying_Utility.Instance.InitAppID ("3ca89e8b4b868b6f", "9b95a47c62a3b4b1");

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


					AssetBundleSettings.SetBaseDownloadingURL (url);

					var request = AssetBundleManager.Initialize (url.UrlId, url.UrlId);
					if (request != null) {
						yield return StartCoroutine (request);

						Debug.Log ("[Loading Completed] UrlId: " + url.UrlId + "\n" + AssetBundleManager.GetBaseDownloadingURL (url.UrlId));
					}
				}
			}
		}
	}
}
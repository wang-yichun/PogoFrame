using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uFrame.Kernel;
using uFrame.MVVM;
using uFrame.MVVM.Services;
using uFrame.MVVM.Bindings;
using uFrame.Serialization;
using UniRx;
using UnityEngine;
using AssetBundles;
using Unity.Linq;

namespace uFrame.ExampleProject
{
	public class LevelRootView : LevelRootViewBase
	{
		public Transform LevelContainer;

		protected override void InitializeViewModel (uFrame.MVVM.ViewModel model)
		{
			base.InitializeViewModel (model);
		}

		public override void Bind ()
		{
			base.Bind ();
		}

		public override void AfterBind ()
		{
			base.AfterBind ();
		}

		#region State Machine

		public override void StateChanged (Invert.StateMachine.State arg1)
		{
			base.StateChanged (arg1);
			Debug.Log ("LevelRoot State Changed: " + arg1.Name);
		}

		public override void OnLevel_Loading ()
		{
			base.OnLevel_Loading ();

			StartCoroutine (LoadAllAssets ());
		}

		public override void OnLevel_AssetsStandby ()
		{
			base.OnLevel_AssetsStandby ();

			if (LevelContainer.childCount == 0) {
				GameObject go = Instantiate<GameObject> (assetsDic ["L001"]);
				go.name = "L000";
				go.transform.SetParent (LevelContainer);
			}
		}

		public override void OnLevel_Closing ()
		{
			base.OnLevel_Closing ();

			assetsDic = null;
			AssetBundleManager.UnloadAssetBundle ("ingame");

			Publish (new UnloadSceneCommand () {
				SceneName = "LevelScene"
			});

			Resources.UnloadUnusedAssets ();

			LevelRoot.StateProperty.Level_Reset.OnNext (true);

			Publish (new LoadSceneCommand () {
				SceneName = "MainMenuScene"
			});
		}

		public override void OnLevel_Reloading ()
		{
			base.OnLevel_Reloading ();

			StartCoroutine (LoadAllAssets_HotReload ());
		}

		#endregion


		public Dictionary<string, GameObject> assetsDic;

		IEnumerator LoadAllAssets ()
		{
			if (AssetBundleManager.InitReady == false) {
				yield return StartCoroutine (AssetLoadingService.Instance.Initialize ());
			}

			yield return StartCoroutine (InstantiateGameObjectAsync ("ingame", "L001"));
			LevelRoot.StateProperty.Level_LoadingFinished.OnNext (true);
		}

		IEnumerator LoadAllAssets_HotReload ()
		{
			LevelContainer.gameObject.Children ().Destroy ();

			if (AssetBundleManager.InitReady == false) {
				yield return StartCoroutine (AssetLoadingService.Instance.Initialize ());
			}

			assetsDic = null;
			AssetBundleManager.UnloadAssetBundle ("ingame");
			Caching.CleanCache ();

			yield return StartCoroutine (InstantiateGameObjectAsync ("ingame", "L001"));
			LevelRoot.StateProperty.Level_LoadingFinished.OnNext (true);
		}

		protected IEnumerator InstantiateGameObjectAsync (string assetBundleName, string assetName)
		{
			// This is simply to get the elapsed time for this phase of AssetLoading.
			float startTime = Time.realtimeSinceStartup;

			// Load asset from assetBundle.
			AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync (assetBundleName, assetName, typeof(GameObject));
			if (request == null)
				yield break;
			yield return StartCoroutine (request);

			// Get the asset.
			if (assetsDic == null) {
				assetsDic = new Dictionary<string, GameObject> ();
			}

			GameObject prefab = request.GetAsset<GameObject> ();

			assetsDic.Add (assetName, prefab);

			// Calculate and display the elapsed time.
			float elapsedTime = Time.realtimeSinceStartup - startTime;
			Debug.Log (assetName + (prefab == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
		}
	}
}
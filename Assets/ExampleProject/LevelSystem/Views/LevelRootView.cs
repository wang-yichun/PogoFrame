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

namespace uFrame.ExampleProject
{
	public class LevelRootView : LevelRootViewBase
	{

		protected override void InitializeViewModel (uFrame.MVVM.ViewModel model)
		{
			base.InitializeViewModel (model);
			// NOTE: this method is only invoked if the 'Initialize ViewModel' is checked in the inspector.
			// var vm = model as LevelManagerViewModel;
			// This method is invoked when applying the data from the inspector to the viewmodel.  Add any view-specific customizations here.
		}

		public override void Bind ()
		{
			base.Bind ();
			// Use this.LevelManager to access the viewmodel.
			// Use this method to subscribe to the view-model.
			// Any designer bindings are created in the base implementation.
		}

		public bool assetReady = false;

		public override void AfterBind ()
		{
			base.AfterBind ();
			StartCoroutine (LoadAllAssets ());
		}

		public override void AddASpriteExecuted (AddASpriteCommand command)
		{
			if (assetReady == false) {
				Debug.Log ("asset is not ready");
				return;
			}

			if (assetsDic != null) {
				GameObject.Instantiate (assetsDic ["sample_go_sprite"]);
			}
		}

		public override void LevelRootUnLoadAssetsExecuted (LevelRootUnLoadAssetsCommand command)
		{
			base.LevelRootUnLoadAssetsExecuted (command);

			AssetBundleManager.UnloadAssetBundle ("_prefabs");
		}

		public Dictionary<string, GameObject> assetsDic;

		IEnumerator LoadAllAssets ()
		{
			// Load asset from assetBundle.
//			AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync ("_prefabs", "DummyAssetName", typeof(GameObject));
//			if (request == null)
//				yield break;
//			yield return StartCoroutine (request);

			yield return StartCoroutine (InstantiateGameObjectAsync ("_prefabs", "sample_go_sprite"));
			assetReady = true;
			Debug.Log ("asset is ready");

			LevelRoot.ExecuteAddASprite ();
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
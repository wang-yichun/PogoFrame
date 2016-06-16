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

		public override void AfterBind ()
		{
			base.AfterBind ();
			StartCoroutine (LoadAssets ());
		}

		UnityEngine.Object Assets_Animal;

		IEnumerator LoadAssets ()
		{
			AssetBundleRequest request = AssetLoadingService.Instance.currentAssetBundle.LoadAssetAsync ("sample_go_sprite");
			yield return request;

			Debug.Log ("loaded asset: " + request.asset.ToString ());
			Assets_Animal = request.asset;
		}

		public override void AddASpriteExecuted (AddASpriteCommand command)
		{
			
		}

		public override void LevelRootUnLoadAssetsExecuted (LevelRootUnLoadAssetsCommand command)
		{
			base.LevelRootUnLoadAssetsExecuted (command);

			AssetLoadingService.Instance.currentAssetBundle.Unload (false);
		}
	}
}
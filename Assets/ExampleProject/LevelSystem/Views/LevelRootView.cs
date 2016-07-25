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
using HedgehogTeam.EasyTouch;

namespace uFrame.ExampleProject
{
	public class LevelRootView : LevelRootViewBase
	{
		public LevelContainer LevelContainer;

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

			LevelRoot.LevelClose.OnNext (new LevelCloseCommand ());
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

			EasyTouchSubscribe ();
		}

		public override void OnLevel_AssetsStandby ()
		{
			base.OnLevel_AssetsStandby ();

			GameObject go = null;

			// 此处为了兼容调试时直接有 LevelNode
			if (LevelContainer.transform.childCount == 0) {
				go = Instantiate<GameObject> (assetsDic ["L001"]);
			} else {
				go = LevelContainer.transform.GetChild (0).gameObject;
			}

			go.name = "L000";

			LevelContainer.SetLevelNode (go.transform);
			LevelContainer.SetBalls_Standby (true);
		}

		public override void OnLevel_Running ()
		{
			base.OnLevel_Running ();

			LevelContainer.SetBalls_Standby (false);
			LevelContainer.SetMagnets_Standby (false);

			cancelRunningUpdate = Observable.EveryFixedUpdate ().Subscribe (RunningUpdate);
		}

		public override void OnLevel_Closing ()
		{
			base.OnLevel_Closing ();

			if (cancelRunningUpdate != null) {
				cancelRunningUpdate.Dispose ();
				cancelRunningUpdate = null;
			}

			EasyTouchUnsubscribe ();

			assetsDic = null;
			AssetBundleManager.UnloadAssetBundle ("ingame");

			LevelContainer.SetLevelNode (null);

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

			if (cancelRunningUpdate != null) {
				cancelRunningUpdate.Dispose ();
				cancelRunningUpdate = null;
			}

			StartCoroutine (LoadAllAssets_HotReload ());
		}

		#endregion

		public Dictionary<string, GameObject> assetsDic;

		IEnumerator LoadAllAssets ()
		{
//			if (AssetBundleManager.InitReady == false) {
//				yield return StartCoroutine (AssetLoadingService.Instance.Initialize ());
//			}

			yield return StartCoroutine (InstantiateGameObjectAsync ("ingame", "L001", "asset1"));

			AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync ("soa", "TestScriptObj", typeof(TestScriptObj), "asset1");
			if (request == null)
				yield break;
			yield return StartCoroutine (request);

			TestScriptObj tso = request.GetAsset<TestScriptObj> ();
			Debug.Log ("TestScriptObj: " + tso.varString);

			LevelRoot.StateProperty.Level_LoadingFinished.OnNext (true);
		}

		IEnumerator LoadAllAssets_HotReload ()
		{
			LevelContainer.SetLevelNode (null);

//			if (AssetBundleManager.InitReady == false) {
//				yield return StartCoroutine (AssetLoadingService.Instance.Initialize ());
//			}

			assetsDic = null;
			AssetBundleManager.UnloadAssetBundle ("ingame");
			Caching.CleanCache ();

			yield return StartCoroutine (InstantiateGameObjectAsync ("ingame", "L001"));
			LevelRoot.StateProperty.Level_LoadingFinished.OnNext (true);
		}

		protected IEnumerator InstantiateGameObjectAsync (string assetBundleName, string assetName, string url_id = "default")
		{
			// This is simply to get the elapsed time for this phase of AssetLoading.
			float startTime = Time.realtimeSinceStartup;

			// Load asset from assetBundle.
			AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync (assetBundleName, assetName, typeof(GameObject), url_id);
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

		void EasyTouchSubscribe ()
		{
			EasyTouch.On_TouchDown += EasyTouch_On_TouchDown;
		}

		void EasyTouchUnsubscribe ()
		{
			EasyTouch.On_TouchDown -= EasyTouch_On_TouchDown;
		}

		void EasyTouch_On_TouchDown (Gesture gesture)
		{
			if (LevelRoot.State is Level_AssetsStandby) {
				LevelRoot.StateProperty.Level_Run.OnNext (true);
			} else if (LevelRoot.State is Level_Running) {
				// TODO: BALL EFFECT

				Vector2 point = Camera.main.ScreenToWorldPoint ((Vector3)gesture.position);
				LevelContainer.SetMagnetsEffect (point);
			}
		}

		IDisposable cancelRunningUpdate;

		void RunningUpdate (long l)
		{
			LevelContainer.SetBallMagnetEffect ();
			LevelContainer.SetMagnetsEffectOff ();
		}
	}
}
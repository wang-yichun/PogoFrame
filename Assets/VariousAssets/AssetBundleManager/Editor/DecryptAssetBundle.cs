
namespace pogorock
{
	using UnityEngine;
	using UnityEditor;
	using System.Collections;
	using Newtonsoft.Json;
	using UniRx;
	using System;

	[ExecuteInEditMode]
	public class DecryptAssetBundle : EditorWindow
	{

		private string assetURL = "";
		private static WWW www;

		[MenuItem ("PogoTools/Decrypt Asset Bundle")]
		public static void Init (string assetURL = "", bool autoRun = false)
		{    
			DecryptAssetBundle window = EditorWindow.GetWindow (typeof(DecryptAssetBundle)) as DecryptAssetBundle;
			window.assetURL = assetURL;
			window.load (assetURL);
		}

		void OnGUI ()
		{
			assetURL = EditorGUILayout.TextField ("Asset bundle URL: ", assetURL);

			GUILayout.BeginHorizontal ();

			GUILayout.EndHorizontal ();
		}

		void OnDestroy ()
		{
			ContinuationManager.Clear ();
		}

		private void load (string s)
		{
			www = new WWW (s);

			ContinuationManager.Add (() => www.isDone, () => {
				if (!string.IsNullOrEmpty (www.error)) {
					Debug.Log ("WWW failed: " + www.error);
				} else {
					Debug.Log ("WWW result : " + www.text);
					AssetBundle bundle = www.assetBundle;
					string[] d = bundle.GetAllAssetNames ();
					Debug.Log (JsonConvert.SerializeObject (d, Formatting.Indented));
								
					bundle.Unload (true);
				}
			});
		}

	}
}
﻿using System.Collections;

namespace pogorock
{
	using UnityEngine;
	using UnityEditor;
	using System.IO;
	using AssetBundles;
	using System.Linq;
	using System.Text;
	using System;
	using Newtonsoft.Json;
	using UniRx;

	[CustomPropertyDrawer (typeof(AssetBundleUrl_Loading))]
	[CustomPropertyDrawer (typeof(AssetBundleUrl_Export))]
	public class AssetBundleUrlDrawer : PropertyDrawer
	{
		public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
		{
			Texture2D image = property.FindPropertyRelative ("Enable").boolValue ? AssetBundleSettingsEditor.gizmo_enable : AssetBundleSettingsEditor.gizmo_disable;

			bool is_local = false;
			bool clear = false;

			var s_is_local = property.FindPropertyRelative ("IsLocal");
			if (s_is_local != null) {
				is_local = s_is_local.boolValue;
			}

			var s_clear = property.FindPropertyRelative ("Clear");
			if (s_clear != null) {
				clear = s_clear.boolValue;
			}

			GUIContent gc = new GUIContent (
				                string.Format (
					                "        {0}{1} - [{2}]",
					                is_local || clear ? "  " : string.Empty,
					                property.FindPropertyRelative ("Title").stringValue,
					                property.FindPropertyRelative ("UrlId").stringValue
				                )
			                );
			GUI.Label (position, new GUIContent (image));

			if (is_local || clear) {
				GUI.Label (position, new GUIContent (AssetBundleSettingsEditor.gizmo_local));
			}

			EditorGUI.PropertyField (position, property, gc, true);

			Type t = GetPropertyType (property);
			if (t == typeof(AssetBundleUrl_Loading) && property.isExpanded) {
				UrlItemGUIMenu_Loading (position, property, label);
			} else if (t == typeof(AssetBundleUrl_Export) && property.isExpanded) {
				UrlItemGUIMenu_Export (position, property, label);
			}
		}

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			float height = EditorGUI.GetPropertyHeight (property, GUIContent.none, true) + 10f;

			Type t = GetPropertyType (property);
			if (t == typeof(AssetBundleUrl_Loading) && property.isExpanded) {
				height += GetUrlItemGUIMenuHeight_Loading ();
			} else if (t == typeof(AssetBundleUrl_Export) && property.isExpanded) {
				height += GetUrlItemGUIMenuHeight_Export ();
			}

			return height;
		}

		public Type GetPropertyType (SerializedProperty property)
		{
			if (property.propertyPath.StartsWith ("loadingUrls")) {
				return typeof(AssetBundleUrl_Loading);
			} else if (property.propertyPath.StartsWith ("exportUrls")) {
				return typeof(AssetBundleUrl_Export);
			}
			return typeof(object);
		}

		public void UrlItemGUIMenu_Loading (Rect position, SerializedProperty property, GUIContent label)
		{
			Rect menuRect = new Rect (position.x, position.y + position.height - GetUrlItemGUIMenuHeight_Loading () - 8f, position.width, GetUrlItemGUIMenuHeight_Loading ());
			int buttonCountInRow = 4;
			float spaceBetweenButton = 5f;
			float spaceTotal = (buttonCountInRow - 1) * spaceBetweenButton;
			float buttonWidth = (menuRect.width - spaceTotal) / buttonCountInRow;

			for (int i = 0; i < buttonCountInRow; i++) {
				Rect buttonRect = new Rect (menuRect.x + (buttonWidth + spaceBetweenButton) * i, menuRect.y, buttonWidth, menuRect.height);
				switch (i) {
				case 0:
					if (GUI.Button (buttonRect, "本地资源")) {
						SetStreamingAssetsButtonClicked (property);
					}
					break;
				case 1:
					if (GUI.Button (buttonRect, "本机HTTP")) {
						SetHTTPButtonClicked (property);
					}
					break;
				case 2:
					if (GUI.Button (buttonRect, "本机FTP")) {
						SetFTPButtonClicked (property);
					}
					break;
				case 3:
					if (GUI.Button (buttonRect, "模拟加载")) {
						OnMenuButtonClicked_Loading (property);
					}
					break;
				default:
					break;
				}
			}
		}

		public float GetUrlItemGUIMenuHeight_Loading ()
		{
			return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}

		public void UrlItemGUIMenu_Export (Rect position, SerializedProperty property, GUIContent label)
		{
			Rect menuRect = new Rect (position.x, position.y + position.height - GetUrlItemGUIMenuHeight_Loading () - 8f, position.width, GetUrlItemGUIMenuHeight_Loading ());
			if (GUI.Button (menuRect, "发布")) {
				OnMenuButtonClicked_Export (property);
			}
		}

		public float GetUrlItemGUIMenuHeight_Export ()
		{
			return 20f;
		}

		public T GetAssetBundleUrlByItemProperty<T> (SerializedProperty property) where T : AssetBundleUrl
		{
			int dummy;
			return GetAssetBundleUrlByItemProperty<T> (property, out dummy);
		}

		public T GetAssetBundleUrlByItemProperty<T> (SerializedProperty property, out int index) where T : AssetBundleUrl
		{
			string idx_str = property.displayName.Replace ("Element ", string.Empty);
			int idx = Convert.ToInt32 (idx_str);
			index = idx;
			T url = null;
			if (typeof(T) == typeof(AssetBundleUrl_Loading)) {
				url = (T)Convert.ChangeType (AssetBundleSettings.Instance.loadingUrls [idx], typeof(T));
			} else if (typeof(T) == typeof(AssetBundleUrl_Export)) {
				url = (T)Convert.ChangeType (AssetBundleSettings.Instance.exportUrls [idx], typeof(T));
			}
			return url;
		}

		public void OnMenuButtonClicked_Loading (SerializedProperty property)
		{
			AssetBundleUrl_Loading url = GetAssetBundleUrlByItemProperty<AssetBundleUrl_Loading> (property);
			string full_url = AssetBundleSettings.GetFullUrl (url) + url.UrlId;
			Debug.Log (AssetBundleSettings.logPrefix + "加载: " + JsonConvert.SerializeObject (
				new  {url = url, full = full_url}, 
				Formatting.Indented
			));

			DecryptAssetBundle.Init (full_url, true);

//			GetWWW (full_url).Subscribe (_ => {
//				Debug.Log (AssetBundleSettings.logPrefix + "GetWWW over!!");
//			});

//			GetWWW ("http://www.baidu.com/").Subscribe ();
//			ObservableWWW.LoadFromCacheOrDownload(full_url,
				
//			ObservableWWW.Get (full_url).Subscribe (x => {
//				Debug.Log (AssetBundleSettings.logPrefix + x.Substring (0, 100));
//			}, ex => {
//				Debug.LogException (ex);
//			});
//			ObservableWWW.LoadFromCacheOrDownload (full_url, 1).Subscribe (ab => {
//				Debug.Log (AssetBundleSettings.logPrefix + "ab_name: " + ab.name);
//			});

//			ObservableWWW.LoadFromCacheOrDownload (full_url, 0).Subscribe (ab => {
//				Debug.Log (AssetBundleSettings.logPrefix + "LoadFromCacheOrDownload: " + ab.name); 
//			});

		}

		//		public static IObservable<WWW> LoadFromCacheOrDownload (string url, UniRx.IProgress<float> progress = null)
		//		{
		//			return Observable.FromCoroutine<WWW> ((observer, cancellation) => LoadFromCacheOrDownloadCore (WWW.LoadFromCacheOrDownload (url, 1), observer, progress, cancellation));
		//		}
		//
		//		static IEnumerator LoadFromCacheOrDownloadCore (WWW www, IObserver<WWW> observer, UniRx.IProgress<float> reportProgress, CancellationToken cancel)
		//		{
		//			using (www) {
		//				while (!www.isDone && !cancel.IsCancellationRequested) {
		//					if (reportProgress != null) {
		//						try {
		//							reportProgress.Report (www.progress);
		//						} catch (Exception ex) {
		//							observer.OnError (ex);
		//							yield break;
		//						}
		//					}
		//					yield return null;
		//				}
		//
		//				if (cancel.IsCancellationRequested)
		//					yield break;
		//
		//				if (!string.IsNullOrEmpty (www.error)) {
		//					observer.OnError (new WWWErrorException (www));
		//				} else {
		//					observer.OnNext (www);
		//					observer.OnCompleted ();
		//				}
		//			}
		//		}

		public void OnMenuButtonClicked_Export (SerializedProperty property)
		{
			int idx;
			AssetBundleUrl_Export url = GetAssetBundleUrlByItemProperty<AssetBundleUrl_Export> (property, out idx);

			Debug.Log (AssetBundleSettings.logPrefix + "发布: " + JsonConvert.SerializeObject (url, Formatting.Indented));

			Observable.NextFrame ().Subscribe (_ => {
				AssetBundleSettingsEditor.Instance.PublishAssetBundles (url, idx);
			});
		}

		public void SetStreamingAssetsButtonClicked (SerializedProperty property)
		{
			property.FindPropertyRelative ("Url").stringValue = string.Empty;
			property.FindPropertyRelative ("Simulation").boolValue = false;
			property.FindPropertyRelative ("IsLocal").boolValue = true;
			property.FindPropertyRelative ("Title").stringValue = "StreamingAssets Mode(Auto Set)";

			AssetBundleSettingsEditor.Instance.serializedObject.ApplyModifiedProperties ();
			Selection.activeObject = AssetBundleSettings.Instance;
		}

		public void SetHTTPButtonClicked (SerializedProperty property)
		{
			property.FindPropertyRelative ("Url").stringValue = AssetBundleSettingsEditor.GetHTTPServerUrl ();
			property.FindPropertyRelative ("Simulation").boolValue = false;
			property.FindPropertyRelative ("IsLocal").boolValue = false;
			property.FindPropertyRelative ("Title").stringValue = "Local HTTP(Auto Set)";

			AssetBundleSettingsEditor.Instance.serializedObject.ApplyModifiedProperties ();
			Selection.activeObject = AssetBundleSettings.Instance;
		}

		public void SetFTPButtonClicked (SerializedProperty property)
		{
			property.FindPropertyRelative ("Url").stringValue = string.Format ("ftp://{0}/PogoFrameAssets", AssetBundleSettingsEditor.GetLocalIP ());
			property.FindPropertyRelative ("Simulation").boolValue = false;
			property.FindPropertyRelative ("IsLocal").boolValue = false;
			property.FindPropertyRelative ("Title").stringValue = "Local FTP(Auto Set)";

			AssetBundleSettingsEditor.Instance.serializedObject.ApplyModifiedProperties ();
			Selection.activeObject = AssetBundleSettings.Instance;
		}
	}
}
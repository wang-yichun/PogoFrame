using UnityEngine;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
[InitializeOnLoad]
#endif

public class SocialPlatfromSettings : ScriptableObject {

	public const string VERSION_NUMBER = "7.7";
	public const string FB_SDK_VERSION_NUMBER = "6.2.2";

	public bool ShowImageSharingSettings = false;
	public bool SaveImageToGallery = false;

	public bool showPermitions = true;
	public bool ShowActions = true;

	public bool ShowAPIS = true;

	public List<string> fb_scopes_list =  new List<string>();


	public string TWITTER_CONSUMER_KEY 	= "REPLACE_WITH_YOUR_CONSUMER_KEY";
	public string TWITTER_CONSUMER_SECRET 	= "REPLACE_WITH_YOUR_CONSUMER_SECRET";

	public string TWITTER_ACCESS_TOKEN 	= "";
	public string TWITTER_ACCESS_TOKEN_SECRET 	= "";

	public bool ShowEditorOauthTestingBlock = false;


	private const string ISNSettingsAssetName = "SocialSettings";
	private const string ISNSettingsAssetExtension = ".asset";


	public bool TwitterAPI = true;
	public bool NativeSharingAPI = true;
	public bool InstagramAPI = true;
	public bool EnableImageSharing = true;


	public bool KeepManifestClean = true;


	private static SocialPlatfromSettings instance = null;


	public static SocialPlatfromSettings Instance {

		get {
			if (instance == null) {
				instance = Resources.Load(ISNSettingsAssetName) as SocialPlatfromSettings;

				if (instance == null) {

					// If not found, autocreate the asset object.
					instance = CreateInstance<SocialPlatfromSettings>();
					#if UNITY_EDITOR
					//string properPath = Path.Combine(Application.dataPath, ISNSettingsPath);

					SA_FileStaticAPI.CreateFolder(SA_Config.SettingsPath);

					/*
					if (!Directory.Exists(properPath)) {
						AssetDatabase.CreateFolder("VariousSDK/AndroidNativePlugin/Extensions/", "GooglePlayCommon");
						AssetDatabase.CreateFolder("VariousSDK/AndroidNativePlugin/Extensions/GooglePlayCommon", "Resources");
					}
					*/

					string fullPath = Path.Combine(Path.Combine("Assets", SA_Config.SettingsPath),
					                               ISNSettingsAssetName + ISNSettingsAssetExtension
					                               );

					AssetDatabase.CreateAsset(instance, fullPath);


					instance.AddDefaultScopes();

					#endif
				}
			}
			return instance;
		}
	}


	public void AddDefaultScopes() {

		instance.fb_scopes_list.Add("user_about_me");
		instance.fb_scopes_list.Add("user_friends");
		instance.fb_scopes_list.Add("email");
		instance.fb_scopes_list.Add("publish_actions");
		instance.fb_scopes_list.Add("read_friendlists");
		instance.fb_scopes_list.Add("user_games_activity");
		instance.fb_scopes_list.Add("user_activities");
		instance.fb_scopes_list.Add("user_likes");
	}



}

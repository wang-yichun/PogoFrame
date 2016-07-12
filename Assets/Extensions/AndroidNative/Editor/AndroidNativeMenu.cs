////////////////////////////////////////////////////////////////////////////////
//  
// @module V2D
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;

public class AndroidNativeMenu : EditorWindow {
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	#if UNITY_EDITOR

	//--------------------------------------
	//  EDIT
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Edit Settings", false, 1)]
	public static void Edit() {
		Selection.activeObject = AndroidNativeSettings.Instance;
	}

	//--------------------------------------
	//  GETTING STARTED
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Getting Started/Plugin setup")]
	public static void GettingStartedPluginSetup() {
		string url = "https://unionassets.com/android-native-plugin/plugin-setup-79";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Getting Started/Updates")]
	public static void GettingStartedUpdates() {
		string url = "https://unionassets.com/android-native-plugin/updates-81";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Getting Started/Compatibility")]
	public static void GettingStartedCompatibility() {
		string url = "https://unionassets.com/android-native-plugin/compatibility-154";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  IN-APP-Purchases
	//--------------------------------------
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/In-App Purchases/Setup In Developer Console")]
	public static void InAppSetupInConsole() {
		string url = "https://unionassets.com/android-native-plugin/setup-in-developer-console-87";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/In-App Purchases/Setup In Editor")]
	public static void InAppSetupInEditor() {
		string url = "https://unionassets.com/android-native-plugin/setup-in-editor-88";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/In-App Purchases/Coding Guidelines")]
	public static void InAppCodingGuidelines() {
		string url = "https://unionassets.com/android-native-plugin/coding-guidelines-89";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/In-App Purchases/Video Tutorials")]
	public static void InAppVideoTutorials() {
		string url = "https://unionassets.com/android-native-plugin/video-tutorials-179";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  GOOGLE PLAY SERVICES
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Getting Started")]
	public static void GPSGettingStarted() {
		string url = "https://unionassets.com/android-native-plugin/getting-started-131";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Sign In")]
	public static void GPSSignIn() {
		string url = "https://unionassets.com/android-native-plugin/sign-in-132";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Achievements")]
	public static void GPSAchievements() {
		string url = "https://unionassets.com/android-native-plugin/achievements-139";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Leaderboards")]
	public static void GPSLeaderboards() {
		string url = "https://unionassets.com/android-native-plugin/leaderboards-140";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Friends")]
	public static void GPSFriends() {
		string url = "https://unionassets.com/android-native-plugin/friends-141";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Saved Games")]
	public static void GPSSavedGames() {
		string url = "https://unionassets.com/android-native-plugin/saved-games-142";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Game Gifts")]
	public static void GPSGameGifts() {
		string url = "https://unionassets.com/android-native-plugin/game-gifts-143";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Quests And Events")]
	public static void GPSQuestsAndEvents() {
		string url = "https://unionassets.com/android-native-plugin/quests-and-events-144";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Real-Time Multiplayer")]
	public static void GPSRealTimeMultiplayer() {
		string url = "https://unionassets.com/android-native-plugin/real-time-multiplayer-145";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Turn-Based Multiplayer")]
	public static void GPSTurnBasedMultiplayer() {
		string url = "https://unionassets.com/android-native-plugin/turn-based-multiplayer-306";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Multiplayer Invitations")]
	public static void GPSTMultiplayerInvitations() {
		string url = "https://unionassets.com/android-native-plugin/multiplayer-invitations-338";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google Play Services/Google Play Utilities")]
	public static void GPSGooglePlayUtilities() {
		string url = "https://unionassets.com/android-native-plugin/google-play-utilities-273";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  GOOGLE +
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Google+/Google+ Button")]
	public static void GooglePlusButton() {
		string url = "https://unionassets.com/android-native-plugin/google-button-171";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  SOCIAL
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Social/Native Sharing")]
	public static void SocialNativeSharing() {
		string url = "https://unionassets.com/android-native-plugin/native-sharing-164";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Social/Facebook")]
	public static void SocialFacebook() {
		string url = "https://unionassets.com/android-native-plugin/facebook-165";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Social/Twitter")]
	public static void SocialTwitter() {
		string url = "https://unionassets.com/android-native-plugin/twitter-167";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Social/Instagram Sharing")]
	public static void SocialInstagramSharing() {
		string url = "https://unionassets.com/android-native-plugin/instagram-sharing-199";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  MORE FEATURES
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Local Notifications")]
	public static void FeaturesNotifications() {
		string url = "https://unionassets.com/android-native-plugin/local-notifications-90";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Camera And Gallery")]
	public static void FeaturesCameraAndGallery() {
		string url = "https://unionassets.com/android-native-plugin/camera-and-gallery-93";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Immersive Mode")]
	public static void FeaturesImmersiveMode() {
		string url = "https://unionassets.com/android-native-plugin/immersive-mode-91";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Application Information")]
	public static void FeaturesApplicationInformation() {
		string url = "https://unionassets.com/android-native-plugin/application-information-94";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Run External App")]
	public static void FeaturesRunExternalApp() {
		string url = "https://unionassets.com/android-native-plugin/run-external-app-95";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Analytics")]
	public static void FeaturesAnalytics() {
		string url = "https://unionassets.com/android-native-plugin/analytics-96";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Google Cloud Save")]
	public static void FeaturesGoogleCloudSave() {
		string url = "https://unionassets.com/android-native-plugin/google-cloud-save-150";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Poups and Pre-loaders")]
	public static void FeaturesPoupsPreloaders() {
		string url = "https://unionassets.com/android-native-plugin/poups-and-pre-loaders-174";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Google Mobile Ad")]
	public static void FeaturesGoogleMobilAd() {
		string url = "https://unionassets.com/android-native-plugin/google-mobile-ad-178";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Native System Events")]
	public static void FeaturesNativeSystemEvents() {
		string url = "https://unionassets.com/android-native-plugin/native-system-events-180";
		Application.OpenURL(url);
	}


	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/App Licensing")]
	public static void FeaturesAppLicensing() {
		string url = "https://unionassets.com/android-native-plugin/app-licensing-261";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/AddressBook")]
	public static void FeaturesAddressBook() {
		string url = "https://unionassets.com/android-native-plugin/addressbook-262";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/Android TV API")]
	public static void FeaturesAndroidTVAPI() {
		string url = "https://unionassets.com/android-native-plugin/android-tv-api-366";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/More Features/System Utilities")]
	public static void FeaturesSystemUtilities() {
		string url = "https://unionassets.com/android-native-plugin/system-utilities-372";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  PUSH NOTIFICATIONS
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Push Notifications/Push Notifications")]
	public static void PNPushNotifications() {
		string url = "https://unionassets.com/android-native-plugin/push-notifications-169";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Push Notifications/Push with Parse")]
	public static void PNPushWithParse() {
		string url = "https://unionassets.com/android-native-plugin/push-notifications-with-parse-358";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Push Notifications/Push with OneSignal")]
	public static void PNPushWithOneSignal() {
		string url = "https://unionassets.com/android-native-plugin/push-notifications-with-gamethrive-359";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  THIRD PARTY PLUGINS SUPPORT
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Third-Party Plug-Ins Support/Anti-Cheat Toolkit")]
	public static void TPPSAntiCheatToolkit() {
		string url = "https://unionassets.com/android-native-plugin/anti-cheat-toolkit-420";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  PLAYMAKER
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Playmaker/Actions List")]
	public static void PlaymakerActionsList() {
		string url = "https://unionassets.com/android-native-plugin/actions-list-98";
		Application.OpenURL(url);
	}

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Playmaker/In-App Purchasing with Playmaker")]
	public static void PlaymakerInApp() {
		string url = "https://unionassets.com/android-native-plugin/in-app-purchasing-with-playmaker-99";
		Application.OpenURL(url);
	}
	
	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Playmaker/Google Ad With Playmaker")]
	public static void PlaymakerGoogleAd() {
		string url = "https://unionassets.com/android-native-plugin/google-ad-with-playmaker-100";
		Application.OpenURL(url);
	}

	//--------------------------------------
	//  FAQ
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/FAQ")]
	public static void FAQ() {
		string url = "https://unionassets.com/android-native-plugin/manual#faq";
		Application.OpenURL(url);
	}
	
	//--------------------------------------
	//  TROUBLESHOOTING
	//--------------------------------------

	[MenuItem("Window/Stan's Assets/Android Native/Documentation/Troubleshooting")]
	public static void Troubleshooting() {
		string url = "https://unionassets.com/android-native-plugin/manual#troubleshooting";
		Application.OpenURL(url);
	}

	#endif

}

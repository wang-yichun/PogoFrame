////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FacebookAndroidUseExample : MonoBehaviour {
	
	
	private static bool IsUserInfoLoaded = false;
	private static bool IsFrindsInfoLoaded = false;
	private static bool IsAuntificated = false;
	
	
	
	public DefaultPreviewButton[] ConnectionDependedntButtons;
	
	public DefaultPreviewButton connectButton;
	public SA_Texture avatar;
	public SA_Label Location;
	public SA_Label Language;
	public SA_Label Mail;
	public SA_Label Name;
	
	
	public SA_Label f1;
	public SA_Label f2;
	
	public SA_Texture fi1;
	public SA_Texture fi2;
	
	
	public Texture2D ImageToShare;
	
	public GameObject friends;
	
	private int startScore = 555;

//	private Dictionary<string, FBPermission> permissions = new Dictionary<string, FBPermission>();
	
	void Awake() {
		

		SPFacebook.OnInitCompleteAction += OnInit;
		SPFacebook.OnFocusChangedAction += OnFocusChanged;


		SPFacebook.OnAuthCompleteAction += OnAuth;

		

		SPFacebook.OnPostingCompleteAction += OnPost;

		

		
		//scores Api events
		SPFacebook.OnPlayerScoresRequestCompleteAction += OnPlayerScoreRequestComplete; 
		SPFacebook.OnAppScoresRequestCompleteAction += OnAppScoreRequestComplete; 
		SPFacebook.OnSubmitScoreRequestCompleteAction += OnSubmitScoreRequestComplete; 
		SPFacebook.OnDeleteScoresRequestCompleteAction += OnDeleteScoreRequestComplete; 

		//SPFacebook.Instance.OnPermissionsLoaded += HandleOnPermissionsLoaded;
		//SPFacebook.Instance.OnRevokePermission += HandleOnRevokePermission;
		
		SPFacebook.instance.Init();
		
		
		
		SA_StatusBar.text = "initializing Facebook";
		
		
		
	}

	void HandleOnRevokePermission (FB_Result result)
	{
		Debug.Log("[HandleOnRevokePermission] result.IsSucceeded: " + result.IsSucceeded + " Responce: " + result.RawData);
	}

	/*
	void HandleOnPermissionsLoaded (FBPermissionResult result)
	{
		Debug.Log("[HandleOnPermissionsLoaded] result.IsSucceeded: " + result.IsSucceeded);
		foreach (KeyValuePair<string, FBPermission> p in result.Permissions) {
			Debug.Log("[FBPermission Loaded] Name:" + p.Value.Name + " Status:" + p.Value.Status.ToString());
		}

		if (result.IsSucceeded) {
			permissions = result.Permissions;
		}
	}
	*/
	
	void FixedUpdate() {
		if(IsAuntificated) {
			connectButton.text = "Disconnect";
			Name.text = "Player Connected";
			foreach(DefaultPreviewButton btn in ConnectionDependedntButtons) {
				btn.EnabledButton();
			}
		} else {
			foreach(DefaultPreviewButton btn in ConnectionDependedntButtons) {
				btn.DisabledButton();
			}
			connectButton.text = "Connect";
			Name.text = "Player Disconnected";
			
			friends.SetActive(false);
			return;
		}
		
		if(IsUserInfoLoaded) {
			if(SPFacebook.instance.userInfo.GetProfileImage(FB_ProfileImageSize.square) != null) {
				avatar.texture = SPFacebook.instance.userInfo.GetProfileImage(FB_ProfileImageSize.square);
				Name.text = SPFacebook.instance.userInfo.Name + " aka " + SPFacebook.instance.userInfo.UserName;
				Location.text = SPFacebook.instance.userInfo.Location;
				Language.text = SPFacebook.instance.userInfo.Locale;
			}
		}
		
		
		if(IsFrindsInfoLoaded) {
			friends.SetActive(true);
			int i = 0;
			if (SPFacebook.instance.friendsList != null) {
				foreach(FB_UserInfo friend in SPFacebook.instance.friendsList) {

					if(i == 0) {
						f1.text = friend.Name;
						if(friend.GetProfileImage(FB_ProfileImageSize.square) != null) {
							fi1.texture = friend.GetProfileImage(FB_ProfileImageSize.square);
						} 
					} else {
						f2.text = friend.Name;
						if(friend.GetProfileImage(FB_ProfileImageSize.square) != null) {
							fi2.texture = friend.GetProfileImage(FB_ProfileImageSize.square);
						} 
					}
					
					i ++;
				}
			}
		} else {
			friends.SetActive(false);
		}
		
		
		
	}




	
	private void PostWithAuthCheck() {
		SPFacebook.instance.FeedShare (
			link: "https://example.com/myapp/?storyID=thelarch",
			linkName: "The Larch",
			linkCaption: "I thought up a witty tagline about larches",
			linkDescription: "There are a lot of larch trees around here, aren't there?",
			picture: "https://example.com/myapp/assets/1/larch.jpg"
			);
	}
	
	
	private void PostNativeScreenshot() {
		StartCoroutine(PostFBScreenshot());
	}
	
	private void PostImage() {
		AndroidSocialGate.StartShareIntent("Hello Share Intent", "This is my text to share", ImageToShare,  "facebook.katana");
	}
	
	
	
	private IEnumerator PostFBScreenshot() {
		
		
		yield return new WaitForEndOfFrame();
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D( width, height, TextureFormat.RGB24, false );
		// Read screen contents into the texture
		tex.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		tex.Apply();
		
		AndroidSocialGate.StartShareIntent("Hello Share Intent", "This is my text to share", tex,  "facebook.katana");
		
		Destroy(tex);
		
	}
	
	
	private void Connect() {
		if(!IsAuntificated) {
			SPFacebook.instance.Login();//"email","publish_actions","user_friends");
			SA_StatusBar.text = "Log in...";
		} else {
			LogOut();
			SA_StatusBar.text = "Logged out";
		}
	}
	
	private void LoadUserData() {
		SPFacebook.OnUserDataRequestCompleteAction += OnUserDataLoaded;
		SPFacebook.instance.LoadUserData();
		SA_StatusBar.text = "Loadin user data..";
	}
	
	private void PostMessage() {
		SPFacebook.instance.FeedShare (
			link: "https://example.com/myapp/?storyID=thelarch",
			linkName: "The Larch",
			linkCaption: "I thought up a witty tagline about larches",
			linkDescription: "There are a lot of larch trees around here, aren't there?",
			picture: "https://example.com/myapp/assets/1/larch.jpg"
			);
		
		SA_StatusBar.text = "Positng..";
	}
	
	private void PostScreehShot() {
		StartCoroutine(PostScreenshot());
		SA_StatusBar.text = "Positng..";
	}
	
	private void LoadFriends() {

		SPFacebook.OnFriendsDataRequestCompleteAction += OnFriendsDataLoaded;

		int limit = 5;
		SPFacebook.instance.LoadFrientdsInfo(limit);
		SA_StatusBar.text = "Loading friends..";
	}
	
	private void AppRequest() {
		SPFacebook.instance.AppRequest("Come play this great game!");
	}

	private void GetPermissions() {
	//	SPFacebook.Instance.CallPermissionCheck();
	}

	private void RevokePermission() {
		/*
		if (permissions.ContainsKey("user_friends")) {
			SPFacebook.Instance.RevokePermission(permissions["user_friends"]);
		} else {
			Debug.Log("There is NO 'user_friends' permission granted");
		}
		*/
	}
	
	private void LoadScore() {
		SPFacebook.instance.LoadPlayerScores();
	}
	
	private void LoadAppScores() {
		SPFacebook.instance.LoadAppScores();
	}
	
	public void SubmitScore() {
		startScore++;
		SPFacebook.instance.SubmitScore(startScore);
	}
	
	
	public void DeletePlayerScores() {
		SPFacebook.instance.DeletePlayerScores();
	}
	
	public void LikePage() {
		Application.OpenURL("https://www.facebook.com/unionassets");
	}
	
	
	private string UNION_ASSETS_PAGE_ID = "1435528379999137";


	public void CheckLike() {
		
		//checking if current user likes the page
		Debug.Log("[CheckLike]");

		bool IsLikes = SPFacebook.instance.IsUserLikesPage(SPFacebook.instance.UserId, UNION_ASSETS_PAGE_ID);
		if(IsLikes) {
			SA_StatusBar.text ="Current user Likes union assets";
		} else {
			//user do not like the page or we han't yet downloaded likes data
			//downloading likes for this page
			SPFacebook.OnLikesListLoadedAction += OnLikesLoaded;
			SPFacebook.instance.LoadLikes(SPFacebook.instance.UserId, UNION_ASSETS_PAGE_ID);

		}

	}
	

	// --------------------------------------
	// EVENTS
	// --------------------------------------
	
	private void OnLikesLoaded(FB_Result result) {
		Debug.Log("[OnLikesLoaded] result " + result.RawData);
		//The likes is loaded so now we can find out for sure if user is like our page
		bool IsLikes = SPFacebook.Instance.IsUserLikesPage(SPFacebook.instance.UserId, UNION_ASSETS_PAGE_ID);
		if(IsLikes) {
			SA_StatusBar.text ="Current user Likes union assets";
		} else {
			SA_StatusBar.text ="Current user does not like union assets";
		}
	}
	
	
	private void OnFocusChanged(bool focus) {

		Debug.Log("FB OnFocusChanged: " + focus);

		if (!focus)  {                                                                                        
			// pause the game - we will need to hide                                             
			Time.timeScale = 0;                                                                  
		} else  {                                                                                        
			// start the game back up - we're getting focus again                                
			Time.timeScale = 1;                                                                  
		}   
	}
	

	
	private void OnUserDataLoaded(FB_Result result) {

		SPFacebook.OnUserDataRequestCompleteAction -= OnUserDataLoaded;

		if (result.IsSucceeded)  { 
			SA_StatusBar.text = "User data loaded";
			IsUserInfoLoaded = true;

			//user data available, we can get info using
			//SPFacebook.instance.userInfo getter
			//and we can also use userInfo methods, for exmple download user avatar image
			SPFacebook.Instance.userInfo.LoadProfileImage(FB_ProfileImageSize.square);


		} else {
			SA_StatusBar.text ="Opps, user data load failed, something was wrong";
			Debug.Log("Opps, user data load failed, something was wrong");
		}

	}
	

	private void OnFriendsDataLoaded(FB_Result res) {
		SPFacebook.OnFriendsDataRequestCompleteAction -= OnFriendsDataLoaded;

		if(res.Error == null) {
			//friednds data available, we can get it using
			//SPFacebook.instance.friendsList getter
			//and we can also use FacebookUserInfo methods, for exmple download user avatar image
			
			foreach(FB_UserInfo friend in SPFacebook.instance.friendsList) {
				friend.LoadProfileImage(FB_ProfileImageSize.square);
			}
			
			IsFrindsInfoLoaded = true;
		} else {
			Debug.Log("Opps, friends data load failed, something was wrong");
		}
	}
	
	
	
	
	private void OnInit() {
		if(SPFacebook.instance.IsLoggedIn) {
			IsAuntificated = true;
		} else {
			SA_StatusBar.text = "user Login -> fale";
		}
	}
	
	
	private void OnAuth(FB_Result result) {
		if(SPFacebook.instance.IsLoggedIn) {
			IsAuntificated = true;
			SA_StatusBar.text = "user Login -> true";
		} else {
			Debug.Log("Failed to log in");
		}

	}
	
	private void OnPost(FB_PostResult res) {

		if(res.IsSucceeded) {
			Debug.Log("Posting complete");
			Debug.Log("Posy id: " + res.PostId);
			SA_StatusBar.text = "Posting complete";
		} else {
			SA_StatusBar.text = "Oops, post failed, something was wrong " + res.Error;
			Debug.Log("Oops, post failed, something was wrong " + res.Error);
		}
	}

	
	//scores Api events
	private void OnPlayerScoreRequestComplete(FB_Result result) {
		
		if(result.IsSucceeded) {
			string msg = "Player has scores in " + SPFacebook.instance.userScores.Count + " apps" + "\n";
			msg += "Current Player Score = " + SPFacebook.instance.GetCurrentPlayerIntScoreByAppId(SPFacebook.Instance.AppId);
			SA_StatusBar.text = msg;
			
		} else {
			SA_StatusBar.text = result.RawData;
		}
		
		
	}
	
	private void OnAppScoreRequestComplete(FB_Result result) {
		
		if(result.IsSucceeded) {
			string msg = "Loaded " + SPFacebook.instance.appScores.Count + " scores results" + "\n";
			msg += "Current Player Score = " + SPFacebook.instance.GetScoreByUserId(SPFacebook.instance.UserId);
			SA_StatusBar.text = msg;
			
		} else {
			SA_StatusBar.text = result.RawData;
		}
		
	}
	
	private void OnSubmitScoreRequestComplete(FB_Result result) {
		
	
		if(result.IsSucceeded) {
			string msg = "Score successfully submited" + "\n";
			msg += "Current Player Score = " + SPFacebook.instance.GetScoreByUserId(SPFacebook.instance.UserId);
			SA_StatusBar.text = msg;
			
		} else {
			SA_StatusBar.text = result.RawData;
		}
		
		
	}
	
	private void OnDeleteScoreRequestComplete(FB_Result result) {
		if(result.IsSucceeded) {
			string msg = "Score successfully deleted" + "\n";
			msg += "Current Player Score = " + SPFacebook.instance.GetScoreByUserId(SPFacebook.instance.UserId);
			SA_StatusBar.text = msg;
			
		} else {
			SA_StatusBar.text = result.RawData;
		}
		
		
	}
	
	
	
	
	
	
	
	// --------------------------------------
	// PRIVATE METHODS
	// --------------------------------------
	
	// --------------------------------------
	// PRIVATE METHODS
	// --------------------------------------
	
	private IEnumerator PostScreenshot() {
		
		
		yield return new WaitForEndOfFrame();
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D( width, height, TextureFormat.RGB24, false );
		// Read screen contents into the texture
		tex.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		tex.Apply();
		
		SPFacebook.instance.PostImage("My app ScreehShot", tex);
		
		Destroy(tex);
		
	}
	
	private void LogOut() {
		IsUserInfoLoaded = false;
		
		IsAuntificated = false;
		
		SPFacebook.instance.Logout();
	}
	
	
	
}

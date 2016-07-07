////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayServiceExample : AndroidNativeExampleBase {
	
	private int score = 100;


	//example
	private const string LEADERBOARD_NAME = "leaderboard_best_scores";
	//private const string LEADERBOARD_NAME = "REPLACE_WITH_YOUR_NAME";


	private  const string PIE_GIFT_ID = "Pie";
	//private  const string PIE_GIFT_ID = "REPLACE_WITH_YOUR_ID";


	//example
	private const string LEADERBOARD_ID = "CgkIipfs2qcGEAIQAA";
	//private const string LEADERBOARD_ID = "REPLACE_WITH_YOUR_ID";



	private const string INCREMENTAL_ACHIEVEMENT_ID = "CgkIipfs2qcGEAIQCg";
	//private const string INCREMENTAL_ACHIEVEMENT_ID = "REPLACE_WITH_YOUR_ID";



	public GameObject avatar;
	private Texture defaulttexture;
	public Texture2D pieIcon;

	public DefaultPreviewButton connectButton;
	public DefaultPreviewButton scoreSubmit;
	public SA_Label playerLabel;

	public DefaultPreviewButton[] ConnectionDependedntButtons;




	public SA_Label a_id;
	public SA_Label a_name;
	public SA_Label a_descr;
	public SA_Label a_type;
	public SA_Label a_state;
	public SA_Label a_steps;
	public SA_Label a_total;


	public SA_Label b_id;
	public SA_Label b_name;
	public SA_Label b_all_time;




	void Start() {

		playerLabel.text = "Player Disconnected";
		defaulttexture = avatar.GetComponent<Renderer>().material.mainTexture;

		//listen for GooglePlayConnection events
		GooglePlayConnection.ActionPlayerConnected +=  OnPlayerConnected;
		GooglePlayConnection.ActionPlayerDisconnected += OnPlayerDisconnected;
		GooglePlayConnection.ActionConnectionResultReceived += ActionConnectionResultReceived;

		//listen for GooglePlayManager events
		GooglePlayManager.ActionAchievementUpdated += OnAchievementUpdated;
		GooglePlayManager.ActionScoreSubmited += OnScoreSubmited;
		GooglePlayManager.ActionScoresListLoaded += OnScoreUpdated;

		GooglePlayManager.ActionSendGiftResultReceived += OnGiftResult;
		GooglePlayManager.ActionPendingGameRequestsDetected += OnPendingGiftsDetected;
		GooglePlayManager.ActionGameRequestsAccepted += OnGameRequestAccepted;
	
		GooglePlayManager.ActionOAuthTokenLoaded += ActionOAuthTokenLoaded;
		GooglePlayManager.ActionAvailableDeviceAccountsLoaded += ActionAvailableDeviceAccountsLoaded;
		GooglePlayManager.ActionAchievementsLoaded += OnAchievmnetsLoadedInfoListner;

		if(GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED) {
			//checking if player already connected
			OnPlayerConnected ();
		} 

	}

	private void OnDestroy() {
		if(!GooglePlayConnection.IsDestroyed) {

			GooglePlayConnection.ActionPlayerConnected -=  OnPlayerConnected;
			GooglePlayConnection.ActionPlayerDisconnected -= OnPlayerDisconnected;
			GooglePlayConnection.ActionConnectionResultReceived -= ActionConnectionResultReceived;
		}

		if(!GooglePlayManager.IsDestroyed) {

			GooglePlayManager.ActionAchievementUpdated -= OnAchievementUpdated;
			GooglePlayManager.ActionScoreSubmited -= OnScoreSubmited;
			GooglePlayManager.ActionScoresListLoaded -= OnScoreUpdated;

			GooglePlayManager.ActionSendGiftResultReceived -= OnGiftResult;
			GooglePlayManager.ActionPendingGameRequestsDetected -= OnPendingGiftsDetected;
			GooglePlayManager.ActionGameRequestsAccepted -= OnGameRequestAccepted;
			
			GooglePlayManager.ActionAvailableDeviceAccountsLoaded -= ActionAvailableDeviceAccountsLoaded;
			GooglePlayManager.ActionOAuthTokenLoaded -= ActionOAuthTokenLoaded;
			GooglePlayManager.ActionAchievementsLoaded -= OnAchievmnetsLoadedInfoListner;
		}
	}

	private void ConncetButtonPress() {
		Debug.Log("GooglePlayManager State  -> " + GooglePlayConnection.State.ToString());
		if(GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED) {
			SA_StatusBar.text = "Disconnecting from Play Service...";
			GooglePlayConnection.Instance.Disconnect ();
		} else {
			SA_StatusBar.text = "Connecting to Play Service...";
			GooglePlayConnection.Instance.Connect ();
		}
	}

	private void GetAccs() {
		GooglePlayManager.Instance.RetrieveDeviceGoogleAccounts();
	}

	private void RetrieveToken() {


		GooglePlayManager.Instance.LoadToken();
	}


	private void showLeaderBoardsUI() {
		GooglePlayManager.Instance.ShowLeaderBoardsUI ();
		SA_StatusBar.text = "Showing Leader Boards UI";
	}

	private void loadLeaderBoards() {
		if (GooglePlayManager.Instance.GetLeaderBoard(LEADERBOARD_ID).GetCurrentPlayerScore(GPBoardTimeSpan.ALL_TIME, GPCollectionType.FRIENDS) == null) {
			//listening for load event 
			GooglePlayManager.ActionLeaderboardsLoaded += OnLeaderBoardsLoaded;
			GooglePlayManager.Instance.LoadLeaderBoards ();
			SA_StatusBar.text = "Loading Leader Boards Data...";
		} else {
			SA_StatusBar.text = LEADERBOARD_NAME + "  score  " + GooglePlayManager.Instance.GetLeaderBoard(LEADERBOARD_ID).GetCurrentPlayerScore(GPBoardTimeSpan.ALL_TIME, GPCollectionType.FRIENDS).LongScore.ToString();
			AN_PoupsProxy.showMessage (LEADERBOARD_NAME + "  score",  GooglePlayManager.Instance.GetLeaderBoard(LEADERBOARD_ID).GetCurrentPlayerScore(GPBoardTimeSpan.ALL_TIME, GPCollectionType.FRIENDS).LongScore.ToString());

			UpdateBoardInfo();
		}
	}

	private void showLeaderBoard() {
		GooglePlayManager.Instance.ShowLeaderBoardById (LEADERBOARD_ID);
		SA_StatusBar.text = "Shwoing Leader Board UI for " + LEADERBOARD_ID;
	}

	private void submitScore() {
		score++;
		GooglePlayManager.Instance.SubmitScore (LEADERBOARD_NAME, score);
		SA_StatusBar.text = "Score " + score.ToString() + " Submited for " + LEADERBOARD_NAME;
	}


	private void ResetBoard() {
		GooglePlayManager.Instance.ResetLeaderBoard(LEADERBOARD_ID);
		UpdateBoardInfo();
	}




	private void showAchievementsUI() {
		GooglePlayManager.Instance.ShowAchievementsUI ();
		SA_StatusBar.text = "Showing Achievements UI";

	}

	private void loadAchievements() {
		GooglePlayManager.ActionAchievementsLoaded += OnAchievementsLoaded;
		GooglePlayManager.Instance.LoadAchievements ();

		SA_StatusBar.text = "Loading Achievements Data...";
	}

	private void reportAchievement() {
		GooglePlayManager.Instance.UnlockAchievement ("achievement_simple_achievement_example");
		SA_StatusBar.text = "Reporting achievement_prime...";
	}

	private void incrementAchievement() {
		GooglePlayManager.Instance.IncrementAchievementById (INCREMENTAL_ACHIEVEMENT_ID, 1);
		SA_StatusBar.text = "Incrementing achievement_bored...";
	}


	private void revealAchievement() {
		GooglePlayManager.Instance.RevealAchievement ("achievement_hidden_achievement_example");
		SA_StatusBar.text = "Revealing achievement_humble...";
	}

	private void ResetAchievement() {
		GooglePlayManager.Instance.ResetAchievement(INCREMENTAL_ACHIEVEMENT_ID);

		AN_PoupsProxy.showMessage ("Reset Complete: ", "Reset Complete, but since this is feature for testing only, achievement data cache will be updated after next interaction with acheivment");
	}

	private void ResetAllAchievements() {
		GooglePlayManager.Instance.ResetAllAchievements();
		AN_PoupsProxy.showMessage ("Reset Complete: ", "Reset Complete, but since this is feature for testing only, achievement data cache will be updated after next interaction with acheivment");

	}



	private void SendGiftRequest() {


	
		GooglePlayManager.Instance.SendGiftRequest(GPGameRequestType.TYPE_GIFT, 1, pieIcon, "Here is some pie", PIE_GIFT_ID);
	}


	private void OpenInbox() {
		GooglePlayManager.Instance.ShowRequestsAccepDialog();
	}



	
	public void clearDefaultAccount() {
		GooglePlusAPI.Instance.ClearDefaultAccount();
	}


	void FixedUpdate() {
		if(GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED) {
			if(GooglePlayManager.Instance.player.icon != null) {
				avatar.GetComponent<Renderer>().material.mainTexture = GooglePlayManager.Instance.player.icon;
			}
		}  else {
			avatar.GetComponent<Renderer>().material.mainTexture = defaulttexture;
		}


		string title = "Connect";
		if(GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED) {
			title = "Disconnect";

			foreach(DefaultPreviewButton btn in ConnectionDependedntButtons) {
				btn.EnabledButton();
			}


		} else {
			foreach(DefaultPreviewButton btn in ConnectionDependedntButtons) {
				btn.DisabledButton();

			}
			if(GooglePlayConnection.State == GPConnectionState.STATE_DISCONNECTED || GooglePlayConnection.State == GPConnectionState.STATE_UNCONFIGURED) {

				title = "Connect";
			} else {
				title = "Connecting..";
			}
		}

		connectButton.text = title;


		scoreSubmit.text = "Submit Score: " + score;
	}


	public void RequestAdvertisingId() {
		GooglePlayUtils.ActionAdvertisingIdLoaded += ActionAdvertisingIdLoaded;
		GooglePlayUtils.Instance.GetAdvertisingId();
	}



	
	//--------------------------------------
	// EVENTS
	//--------------------------------------

	private void ActionAdvertisingIdLoaded (GP_AdvertisingIdLoadResult res) {
		GooglePlayUtils.ActionAdvertisingIdLoaded -= ActionAdvertisingIdLoaded;

		if(res.IsSucceeded) {
			AndroidMessage.Create("Succeeded", "Advertising Id: " + res.id);
		} else {
			AndroidMessage.Create("Failed", "Advertising Id failed to loaed");
		}

		
	}

	private void OnAchievmnetsLoadedInfoListner(GooglePlayResult res) {
		GPAchievement achievement = GooglePlayManager.Instance.GetAchievement(INCREMENTAL_ACHIEVEMENT_ID);


		if(achievement != null) {
			a_id.text 		= "Id: " + achievement.Id;
			a_name.text 	= "Name: " +achievement.Name;
			a_descr.text 	= "Description: " + achievement.Description;
			a_type.text 	= "Type: " + achievement.Type.ToString();
			a_state.text 	= "State: " + achievement.State.ToString();
			a_steps.text 	= "CurrentSteps: " + achievement.CurrentSteps.ToString();
			a_total.text 	= "TotalSteps: " + achievement.TotalSteps.ToString();
		}
	}

	private void OnAchievementsLoaded(GooglePlayResult result) {
		GooglePlayManager.ActionAchievementsLoaded -= OnAchievementsLoaded;
		if(result.IsSucceeded) {

			foreach (GPAchievement achievement in GooglePlayManager.Instance.Achievements) {
				Debug.Log(achievement.Id);
				Debug.Log(achievement.Name);
				Debug.Log(achievement.Description);
				Debug.Log(achievement.Type);
				Debug.Log(achievement.State);
				Debug.Log(achievement.CurrentSteps);
				Debug.Log(achievement.TotalSteps);
			}

			SA_StatusBar.text = "Total Achievement: " + GooglePlayManager.Instance.Achievements.Count.ToString();
			AN_PoupsProxy.showMessage ("Achievments Loaded", "Total Achievements: " + GooglePlayManager.Instance.Achievements.Count.ToString());
		} else {
			SA_StatusBar.text = result.Message;
			AN_PoupsProxy.showMessage ("Achievments Loaded error: ", result.Message);
		}

	}

	private void OnAchievementUpdated(GP_AchievementResult result) {
		SA_StatusBar.text = "Achievment Updated: Id: " + result.achievementId + "\n status: " + result.Message;
		AN_PoupsProxy.showMessage ("Achievment Updated ", "Id: " + result.achievementId + "\n status: " + result.Message);
	}

	

	private void OnLeaderBoardsLoaded(GooglePlayResult result) {
		GooglePlayManager.ActionLeaderboardsLoaded -= OnLeaderBoardsLoaded;

		if(result.IsSucceeded) {
			if( GooglePlayManager.Instance.GetLeaderBoard(LEADERBOARD_ID) == null) {
				AN_PoupsProxy.showMessage("Leader boards loaded", LEADERBOARD_ID + " not found in leader boards list");
				return;
			}


			SA_StatusBar.text = LEADERBOARD_NAME + "  score  " + GooglePlayManager.Instance.GetLeaderBoard(LEADERBOARD_ID).GetCurrentPlayerScore(GPBoardTimeSpan.ALL_TIME, GPCollectionType.FRIENDS).LongScore.ToString();
			AN_PoupsProxy.showMessage (LEADERBOARD_NAME + "  score",  GooglePlayManager.Instance.GetLeaderBoard(LEADERBOARD_ID).GetCurrentPlayerScore(GPBoardTimeSpan.ALL_TIME, GPCollectionType.FRIENDS).LongScore.ToString());
		} else {
			SA_StatusBar.text = result.Message;
			AN_PoupsProxy.showMessage ("Leader-Boards Loaded error: ", result.Message);
		}

		UpdateBoardInfo();

	}

	private void UpdateBoardInfo() {
		GPLeaderBoard leaderboard = GooglePlayManager.Instance.GetLeaderBoard(LEADERBOARD_ID);
		if(leaderboard != null) {
			b_id.text 		= "Id: " + leaderboard.Id;
			b_name.text 	= "Name: " +leaderboard.Name;

			GPScore score = leaderboard.GetCurrentPlayerScore(GPBoardTimeSpan.ALL_TIME, GPCollectionType.FRIENDS);
			if (score != null) {
				b_all_time.text = "All Time Score: " + score.LongScore;
			} else {
				b_all_time.text = "All Time Score: EMPTY";
			}
		} else {
			b_all_time.text = "All Time Score: " + " -1";
		}
	}

	private void OnScoreSubmited(GP_LeaderboardResult result) {
		if (result.IsSucceeded) {
			SA_StatusBar.text = "Score Submited:  " + result.Message
			+ " LeaderboardId: " + result.Leaderboard.Id
			+ " LongScore: " + result.Leaderboard.GetCurrentPlayerScore(GPBoardTimeSpan.ALL_TIME, GPCollectionType.GLOBAL).LongScore;
		} else {
			SA_StatusBar.text = "Score Submit Fail:  " + result.Message;
		}
	}

	private void OnScoreUpdated(GooglePlayResult res) {
		UpdateBoardInfo();
	}



	private void OnPlayerDisconnected() {
		SA_StatusBar.text = "Player Disconnected";
		playerLabel.text = "Player Disconnected";
	}

	private void OnPlayerConnected() {
		SA_StatusBar.text = "Player Connected";
		playerLabel.text = GooglePlayManager.Instance.player.name + "(" + GooglePlayManager.Instance.currentAccount + ")";
	}

	private void ActionConnectionResultReceived(GooglePlayConnectionResult result) {

		if(result.IsSuccess) {
			Debug.Log("Connected!");
		} else {
			Debug.Log("Cnnection failed with code: " + result.code.ToString());
		}
		SA_StatusBar.text = "ConnectionResul:  " + result.code.ToString();
	}

	void OnGiftResult (GooglePlayGiftRequestResult result) {
		SA_StatusBar.text = "Gift Send Result:  " + result.code.ToString();
		AN_PoupsProxy.showMessage("Gfit Send Complete", "Gift Send Result: " + result.code.ToString());
	}

	void OnPendingGiftsDetected (List<GPGameRequest> gifts) {
		AndroidDialog dialog = AndroidDialog.Create("Pending Gifts Detected", "You got few gifts from your friends, do you whant to take a look?");
		dialog.ActionComplete += OnPromtGiftDialogClose;
	}
	

	private void OnPromtGiftDialogClose(AndroidDialogResult res) {
		//parsing result
		switch(res) {
		case AndroidDialogResult.YES:
			GooglePlayManager.Instance.ShowRequestsAccepDialog();
			break;
		
			
		}
	}

	void OnGameRequestAccepted (List<GPGameRequest> gifts) {
		foreach(GPGameRequest g in gifts) {
			AN_PoupsProxy.showMessage("Gfit Accepted", g.playload + " is excepted");
		}
	}




	private void ActionAvailableDeviceAccountsLoaded(List<string> accounts) {
		string msg = "Device contains following google accounts:" + "\n";
		foreach(string acc in GooglePlayManager.Instance.deviceGoogleAccountList) {
			msg += acc + "\n";
		} 

		AndroidDialog dialog = AndroidDialog.Create("Accounts Loaded", msg, "Sign With Fitst one", "Do Nothing");
		dialog.ActionComplete += SighDialogComplete;

	}

	private void SighDialogComplete (AndroidDialogResult res) {
		if(res == AndroidDialogResult.YES) {
			GooglePlayConnection.Instance.Connect(GooglePlayManager.Instance.deviceGoogleAccountList[0]);
		}

	}



	private void ActionOAuthTokenLoaded(string token) {

		AN_PoupsProxy.showMessage("Token Loaded", GooglePlayManager.Instance.loadedAuthToken);
	}





}

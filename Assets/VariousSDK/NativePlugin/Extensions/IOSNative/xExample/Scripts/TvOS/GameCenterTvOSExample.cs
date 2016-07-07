using UnityEngine;
using System.Collections;

public class GameCenterTvOSExample : MonoBehaviour {



	private int hiScore = 200;
	private bool _IsUILocked = false;


	private string TEST_LEADERBOARD_1 = "your.ios.leaderbord1.id";
//	private string TEST_LEADERBOARD_2 = "combined.board.1";



	private string TEST_ACHIEVEMENT_1_ID = "your.achievement.id1.here";
	private string TEST_ACHIEVEMENT_2_ID = "your.achievement.id2.here";




	void Start () {
		GameCenterManager.OnAuthFinished += OnAuthFinished;
		GameCenterManager.OnScoreSubmitted += OnScoreSubmitted;

		GameCenterManager.OnAchievementsProgress += HandleOnAchievementsProgress;
		GameCenterManager.OnAchievementsReset += HandleOnAchievementsReset;
		GameCenterManager.OnAchievementsLoaded += OnAchievementsLoaded;

		//Achievement registration. If you skip this step GameCenterManager.achievements array will contain only achievements with reported progress 
		GameCenterManager.RegisterAchievement (TEST_ACHIEVEMENT_1_ID);
		GameCenterManager.RegisterAchievement (TEST_ACHIEVEMENT_2_ID);

		GameCenterManager.OnGameCenterViewDismissed += GameCenterManager_OnGameCenterViewDismissed;



		//Initializing Game Center class. This action will trigger authentication flow
		GameCenterManager.Init();
	}


	
	void OnAuthFinished (ISN_Result res) {
		_IsUILocked = true;

		IOSMessage msg = null;
		if (res.IsSucceeded) {
			
			msg = IOSMessage.Create("Player Authed ", "ID: " + GameCenterManager.Player.Id + "\n" + "Alias: " + GameCenterManager.Player.Alias);
			GameCenterManager.LoadLeaderboardInfo(TEST_LEADERBOARD_1);
		} else {
			msg = IOSMessage.Create("Game Center ", "Player authentication failed");
		}

		msg.OnComplete += delegate {
			_IsUILocked = false;
		};
	}


	public void ShowAchivemnets() {
		Debug.Log("ShowAchivemnets " + _IsUILocked);
		if(_IsUILocked) { return; }
		_IsUILocked = true;
		GameCenterManager.ShowAchievements();


	}

	public void SubmitAchievement() {
		Debug.Log("SubmitAchievement");
		GameCenterManager.SubmitAchievement(GameCenterManager.GetAchievementProgress(TEST_ACHIEVEMENT_1_ID) + 2.432f, TEST_ACHIEVEMENT_1_ID);

	}
		
	public void ResetAchievements() {
		Debug.Log("ResetAchievements");
		GameCenterManager.ResetAchievements();
	}
		

	public void ShowLeaderboards() {
		Debug.Log("ShowLeaderboards" + _IsUILocked);
		if(_IsUILocked) { return; }
		_IsUILocked = true;
		GameCenterManager.ShowLeaderboards ();
	}


	public void ShowLeaderboardByID() {
		Debug.Log("ShowLeaderboardByID");
		GameCenterManager.OnFriendsListLoaded += (ISN_Result obj) => {
			Debug.Log("Loaded: " + GameCenterManager.FriendsList.Count + " fiends");
		};
		GameCenterManager.RetrieveFriends();
	}


	public void ReportScore() {
		Debug.Log("ReportScore");
		hiScore++;

		GameCenterManager.ReportScore(hiScore, TEST_LEADERBOARD_1, 17);
	}

	void OnScoreSubmitted (GK_LeaderboardResult result) {
		
		if(result.IsSucceeded) {
			GK_Score score = result.Leaderboard.GetCurrentPlayerScore(GK_TimeSpan.ALL_TIME, GK_CollectionType.GLOBAL);
			IOSNativePopUpManager.showMessage("Leaderboard " + score.LongScore, "Score: " + score.LongScore + "\n" + "Rank:" + score.Rank);
		}
	}



	private void OnAchievementsLoaded(ISN_Result result) {

		ISN_Logger.Log("OnAchievementsLoaded");
		ISN_Logger.Log(result.IsSucceeded);

		if(result.IsSucceeded) {
			ISN_Logger.Log ("Achievements were loaded from iOS Game Center");

			foreach(GK_AchievementTemplate tpl in GameCenterManager.Achievements) {
				ISN_Logger.Log (tpl.Id + ":  " + tpl.Progress);
			}
		}

	}

	void HandleOnAchievementsReset (ISN_Result obj){
		ISN_Logger.Log ("All Achievements were reset");
	}


	private void HandleOnAchievementsProgress (GK_AchievementProgressResult result) {
		if(result.IsSucceeded) {
			GK_AchievementTemplate tpl = result.Achievement;
			ISN_Logger.Log (tpl.Id + ":  " + tpl.Progress.ToString());
		}
	}

	private void GameCenterManager_OnGameCenterViewDismissed () {
		Debug.Log("GameCenterManager ViewDismissed");
		_IsUILocked = false;
	}
}

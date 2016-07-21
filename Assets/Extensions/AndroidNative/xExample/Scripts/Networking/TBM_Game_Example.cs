//#define SA_DEBUG_MODE
using UnityEngine;
using System.Collections;

public class TBM_Game_Example : AndroidNativeExampleBase {


	public GameObject avatar;
	public GameObject hi;
	public SA_Label playerLabel;
	public SA_Label gameState;
	public SA_Label parisipants;

	public DefaultPreviewButton connectButton;


	public DefaultPreviewButton helloButton;
	public DefaultPreviewButton leaveRoomButton;
	public DefaultPreviewButton showRoomButton;


	public DefaultPreviewButton[] ConnectionDependedntButtons;
	public SA_PartisipantUI[] patrisipants;


	//private Texture defaulttexture;

	void Start() {
		
		playerLabel.text = "Player Disconnected";
		//defaulttexture = avatar.GetComponent<Renderer>().material.mainTexture;
		


		GooglePlayConnection.ActionPlayerConnected +=  OnPlayerConnected;
		GooglePlayConnection.ActionPlayerDisconnected += OnPlayerDisconnected;
		
		GooglePlayConnection.ActionConnectionResultReceived += OnConnectionResult;

		
		

		
		if(GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED) {
			//checking if player already connected
			OnPlayerConnected ();
		} 

		InitTBM();
	}




	public void Init() {
		GooglePlayTBM.ActionMatchUpdated += ActionMatchUpdated;
	}



	#if UNITY_ANDROID
	private GP_TBM_Match mMatch = null;
	
	//...

	// Call this method when a player has completed his turn and wants to
	// go onto the next player, which may be himself.
	public void playTurn() {

		string nextParticipantId = string.Empty;

		// Get the next participant in the game-defined way, possibly round-robin.
		//nextParticipantId = getNextParticipantId();


		// Get the updated state. In this example, we simply use a
		// a pre-defined string. In your game, there may be more complicated state.
		string mTurnData = "My turn data sample";

		// At this point, you might want to show a waiting dialog so that
		// the current player does not try to submit turn actions twice.
		AndroidNativeUtility.ShowPreloader("Loading..", "Sending the tunr data");

		// Invoke the next turn. We are converting our data to a byte array.

		System.Text.UTF8Encoding  encoding = new System.Text.UTF8Encoding();
		byte[] byteArray = encoding.GetBytes(mTurnData);

		GooglePlayTBM.instance.TakeTrun(mMatch.Id, byteArray, nextParticipantId);

	}

	#endif

	void ActionMatchUpdated (GP_TBM_UpdateMatchResult result) {
		// Match Date updated
	}

	
	public void InitTBM() {
		int variant = 1;
		GooglePlayTBM.instance.SetVariant (variant);

		int ROLE_WIZARD = 0x4; // 100 in binary
		GooglePlayTBM.instance.SetExclusiveBitMask (ROLE_WIZARD);

		GooglePlayTBM.instance.RegisterMatchUpdateListener();
	}

	public void ShowInboxUI() {
		GooglePlayTBM.instance.ShowInbox();
	}

	public void FinishMathc() {


	}


	public void findMatch() {


		GooglePlayTBM.ActionMatchCreationCanceled += ActionMatchCreationCanceled;
		GooglePlayTBM.ActionMatchInitiated += ActionMatchInitiated;

		int minPlayers = 2;
		int maxPlayers = 2; 
		bool allowAutomatch = true;

		GooglePlayTBM.instance.StartSelectOpponentsView (minPlayers, maxPlayers, allowAutomatch);


	}

	

	void ActionMatchCreationCanceled (AndroidActivityResult result) {
		// Match Creation was cnaceled by user
	}

	void ActionMatchInitiated (GP_TBM_MatchInitiatedResult result) {
		if(!result.IsSucceeded) {
			AndroidMessage.Create("Match Initi Failed", "Status code: " + result.Response);
			return;
		}

		GP_TBM_Match  Match = result.Match;
		// If this player is not the first player in this match, continue.
		if (Match.Data != null) {
			//showTurnUI(match); 
			return;
		}

		// Otherwise, this is the first player. Initialize the game state.
		//initGame(match);
		
		// Let the player take the first turn
		//showTurnUI(match);
	}

	public void LoadAllMatchersInfo() {
		GooglePlayTBM.instance.LoadAllMatchesInfo (GP_TBM_MatchesSortOrder.SORT_ORDER_MOST_RECENT_FIRST);
	}

	public void LoadActiveMatchesInfo() {
		GooglePlayTBM.instance.LoadMatchesInfo (GP_TBM_MatchesSortOrder.SORT_ORDER_MOST_RECENT_FIRST, GP_TBM_MatchTurnStatus.MATCH_TURN_STATUS_MY_TURN, GP_TBM_MatchTurnStatus.MATCH_TURN_STATUS_THEIR_TURN);
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





	

	private void DrawParticipants() {
		

	

	}

	
	void FixedUpdate() {
		DrawParticipants();


		
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
	}

	private void OnPlayerDisconnected() {
		SA_StatusBar.text = "Player Disconnected";
		playerLabel.text = "Player Disconnected";
	}
	
	private void OnPlayerConnected() {
		SA_StatusBar.text = "Player Connected";
		playerLabel.text = GooglePlayManager.instance.player.name;
	}

	private void OnConnectionResult(GooglePlayConnectionResult result) {

		SA_StatusBar.text = "ConnectionResul:  " + result.code.ToString();
		Debug.Log(result.code.ToString());
	}
	
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameSavesExample : BaseIOSFeaturePreview {
	
	private Dictionary<string, List<GK_SavedGame>> GameSaves = new Dictionary<string, List<GK_SavedGame>>();
	private Dictionary<string, List<GK_SavedGame>> SavesConflicts = new Dictionary<string, List<GK_SavedGame>>();

	private string test_name = "savedgame1";
	private string test_name_2 = "savedgame2";

	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {
	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	void OnGUI() {
		UpdateToStartPos();

		GUI.Label(new Rect(StartX, StartY, Screen.width, 40), "GameCenter Game Saves", style);

		StartY+= YLableStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Init")) {
			GameCenterManager.OnAuthFinished += HandleOnAuthFinished;
			GameCenterManager.Init ();
		}

		StartY+= YButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Save Game")) {
			Save (test_name);
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Save Game 2")) {
			Save (test_name_2);
		}

		StartX = XStartPos;
		StartY+= YButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Fetch Saved Games")) {
			Fetch ();
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Load Saved Game")) {
			Load ();
		}

		StartX = XStartPos;
		StartY+= YButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Delete Saved Game")) {
			Delete (test_name);
		}

		StartX += XButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Delete Saved Game 2")) {
			Delete (test_name_2);
		}

		StartX = XStartPos;
		StartY+= YButtonStep;
		if(GUI.Button(new Rect(StartX, StartY, buttonWidth, buttonHeight), "Resolve Conflicts")) {
			ResolveConflicts ();
		}
	}

	//--------------------------------------
	//  GET/SET
	//--------------------------------------

	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------

	private void Save (string name) {
		ISN_Logger.Log ("Start to save game!");
		ISN_GameSaves.ActionGameSaved += HandleActionGameSaved;

		byte[] data = System.Text.Encoding.UTF8.GetBytes("Some data");
		ISN_GameSaves.Instance.SaveGame(data, name);
	}

	private void Fetch() {
		ISN_Logger.Log ("Start to fetch games!");
		ISN_GameSaves.ActionSavesFetched += HandleActionSavesFetched;
		ISN_GameSaves.Instance.FetchSavedGames();
	}

	private void Delete(string name) {
		ISN_Logger.Log ("Start to delete game by name!");
		ISN_GameSaves.ActionSaveRemoved += HandleActionSaveRemoved;
		ISN_GameSaves.Instance.DeleteSavedGame(name);
	}

	private void Load() {
		ISN_Logger.Log ("Start to load game!");
		GK_SavedGame save = GetLoadedSave(test_name);

		if (save == null) {
			ISN_Logger.Log ("You don't have any saved game!");

			return;
		}

		save.ActionDataLoaded += HandleActionDataLoaded;
		save.LoadData ();
	}

	/// <summary>
	/// Call this method separately for each set of saved game conflicts. 
	/// For example, if you have multiple saved game files with the name of “savedgame1” and “savedgame2”,
	/// you need to call this method twice—once with an array containing the GKSavedGame objects with the “savedgame1”
	/// name and once for the “savedgame2” objects. All saved game conflicts are resolved asynchronously.
	/// </summary>
	/// <returns>The count conflicts.</returns>
	private void ResolveConflicts() {
		ISN_Logger.Log ("Trying to fix conflicts");
		List<GK_SavedGame> conflicts = GetConflict();

		if (conflicts == null) {
			ISN_Logger.Log ("You don't have any conflicts!");

			return;
		}

		ISN_GameSaves.ActionSavesResolved += HandleActionSavesResolved;
		byte[] data = System.Text.Encoding.UTF8.GetBytes ("Some data after resolving");//data to be taken as final {money, exp, lvl, etc.}
		ISN_GameSaves.Instance.ResolveConflictingSavedGames (conflicts, data);
	}


	private GK_SavedGame GetLoadedSave(string saveGameName) {
		return GameSaves.ContainsKey (saveGameName) ? GameSaves [saveGameName][0] : null;
	}

	private List<GK_SavedGame> GetConflict() {
		List<GK_SavedGame> currentConflictList = null;

		foreach (List<GK_SavedGame> conflicts in SavesConflicts.Values) {
			currentConflictList = conflicts;
			return currentConflictList;
		}

		return currentConflictList;
	}

	private int GetConflictsCount() {
		ISN_Logger.Log("The total number of duplicates =" + SavesConflicts.Count);
		return SavesConflicts.Count;
	}

	private void CheckSavesOnDuplicates() {
		Dictionary<string, List<GK_SavedGame>> tempSavedDictionary = new Dictionary<string, List<GK_SavedGame>> (GameSaves);

		foreach (KeyValuePair<string, List<GK_SavedGame>> pair in tempSavedDictionary) {
			if(pair.Value.Count > 1) {
				if (!SavesConflicts.ContainsKey (pair.Key)) {
					SavesConflicts.Add (pair.Key, pair.Value);
				}
				GameSaves.Remove(pair.Key); 
			}
		}
		ISN_Logger.Log("------------------------------------------");
		ISN_Logger.Log("Duplicates " + SavesConflicts.Count);
		ISN_Logger.Log("Unique saves " + GameSaves.Count);
		ISN_Logger.Log("------------------------------------------");
	}

	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void HandleOnAuthFinished (ISN_Result result) {
		GameCenterManager.OnAuthFinished -= HandleOnAuthFinished;

		if (result.IsSucceeded) {
		    ISN_Logger.Log("Player Authed");
		} else {
		    IOSNativePopUpManager.showMessage("Game Center ", "Player authentication failed");
		}
	}

	private void HandleActionGameSaved (GK_SaveResult res) {
		ISN_GameSaves.ActionGameSaved -= HandleActionGameSaved;

		if(res.IsSucceeded) {
			ISN_Logger.Log("Saved game with name " + res.SavedGame.Name);
			ISN_Logger.Log("------------------------------------------");
		} else {
			ISN_Logger.Log("Failed: " + res.Error.Description);
		}
	}

	private void HandleActionSaveRemoved (GK_SaveRemoveResult res) {
		ISN_GameSaves.ActionSaveRemoved -= HandleActionSaveRemoved;

		if(res.IsSucceeded) {
			ISN_Logger.Log("Deleted game with name " + res.SaveName);
			ISN_Logger.Log("------------------------------------------");
		} else {
			ISN_Logger.Log("Failed: " + res.Error.Description);
		}
	}

	private void HandleActionDataLoaded (GK_SaveDataLoaded res) {
		res.SavedGame.ActionDataLoaded -= HandleActionDataLoaded;
		if(res.IsSucceeded) {
			ISN_Logger.Log("Data loaded. data Length: " + res.SavedGame.Data.Length);
		} else {
			ISN_Logger.Log("Failed: " + res.Error.Description);
		}
	}

	private void HandleActionSavesFetched (GK_FetchResult res) {
		ISN_GameSaves.ActionSavesFetched -= HandleActionSavesFetched;

		if(res.IsSucceeded) {
			ISN_Logger.Log("Received " + res.SavedGames.Count + " game saves");
			foreach (GK_SavedGame game in res.SavedGames) {
				ISN_Logger.Log("The name of the save game " + game.Name);
			}
			ISN_Logger.Log("------------------------------------------");

			GameSaves.Clear ();
			foreach (GK_SavedGame game in res.SavedGames) {
                
				if (!GameSaves.ContainsKey (game.Name)) {
					GameSaves.Add (game.Name, new List<GK_SavedGame> ());
				}
				GameSaves [game.Name].Add (game);

			}

			ISN_Logger.Log("Check the saves on duplicates");
			CheckSavesOnDuplicates();
		} else {
			ISN_Logger.Log("Failed: " + res.Error.Description + " with code " + res.Error.Code);
		}
	}

	private void HandleActionSavesResolved (GK_SavesResolveResult res) {
		ISN_GameSaves.ActionSavesResolved -= HandleActionSavesResolved;

		if(res.IsSucceeded) {
			ISN_Logger.Log("The conflict is resolved");

			foreach (GK_SavedGame game in res.SavedGames) {
				SavesConflicts.Remove (game.Name);

				if (!GameSaves.ContainsKey (game.Name)) {
					GameSaves.Add (game.Name, new List<GK_SavedGame> ());
					GameSaves [game.Name].Add (game);
				}
			}

			ISN_Logger.Log("------------------------------------------");
			ISN_Logger.Log("Duplicates " + SavesConflicts.Count);
			ISN_Logger.Log("Unique saves " + GameSaves.Count);
			ISN_Logger.Log("------------------------------------------");
			foreach (GK_SavedGame game in res.SavedGames) {
				ISN_Logger.Log("The name of the save game " + game.Name);
			}
		} else {
			ISN_Logger.Log("Failed: " + res.Error.Description);
		}
	}

	//--------------------------------------
	//  DESTROY
	//--------------------------------------
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GK_FetchResult : ISN_Result {

	private List<GK_SavedGame> _SavedGames = new List<GK_SavedGame>();



	public GK_FetchResult(List<GK_SavedGame> saves):base(true) {
		_SavedGames = saves;
	}
	
	public GK_FetchResult(string errorData):base(errorData) {}


	public List<GK_SavedGame> SavedGames {
		get {
			return _SavedGames;
		}
	}
}

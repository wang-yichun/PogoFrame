using UnityEngine;
using System.Collections;

public class GK_SaveResult : ISN_Result {

	private GK_SavedGame _SavedGame = null;



	public GK_SaveResult(GK_SavedGame save):base(true) {
		_SavedGame = save;
	}
	
	public GK_SaveResult(string errorData):base(errorData) {}


	public GK_SavedGame SavedGame {
		get {
			return _SavedGame;
		}
	}
}

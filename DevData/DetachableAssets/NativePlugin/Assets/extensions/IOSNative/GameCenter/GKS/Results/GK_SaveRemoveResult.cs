using UnityEngine;
using System.Collections;

public class GK_SaveRemoveResult : ISN_Result {

	private string _SaveName = string.Empty;



	public GK_SaveRemoveResult(string name):base(true) {
		_SaveName = name;
	}
	
	public GK_SaveRemoveResult(string name, string errorData):base(errorData) {
		_SaveName = name;
	}



	public string SaveName {
		get {
			return _SaveName;
		}
	}
}

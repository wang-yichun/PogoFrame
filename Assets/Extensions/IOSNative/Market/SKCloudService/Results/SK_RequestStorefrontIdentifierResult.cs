using UnityEngine;
using System.Collections;

public class SK_RequestStorefrontIdentifierResult : ISN_Result {

	private string _StorefrontIdentifier = string.Empty;


	public SK_RequestStorefrontIdentifierResult():base(true) {
		
	}

	public SK_RequestStorefrontIdentifierResult(string errorData):base(errorData) {

	}


	public string StorefrontIdentifier {
		get {
			return _StorefrontIdentifier;
		}

		set {
			_StorefrontIdentifier = value;
		}
	}
}

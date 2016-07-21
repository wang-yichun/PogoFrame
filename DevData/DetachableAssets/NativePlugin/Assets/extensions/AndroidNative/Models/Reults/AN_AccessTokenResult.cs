using UnityEngine;
using System.Collections;

public class AN_AccessTokenResult : AN_Result {

	private string _accessToken = string.Empty;
	private string _tokenType = string.Empty;
	private long _expiresIn = 0L;
	private string _error = string.Empty;

	public AN_AccessTokenResult(bool result, string error) : base (result) {
		_error = error;
	}

	public AN_AccessTokenResult(bool result, string accessToken, string tokenType, long expiresIn) : base(result) {
		_accessToken = accessToken;
		_tokenType = tokenType;
		_expiresIn = expiresIn;
	}

	public string AccessToken {
		get {
			return _accessToken;
		}
	}

	public string TokenType {
		get {
			return _tokenType;
		}
	}

	public long ExpiresIn {
		get {
			return _expiresIn;
		}
	}

	public string Error {
		get {
			return _error;
		}
	}
}

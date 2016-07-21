using UnityEngine;
using System.Collections;

public class AN_RefreshTokenResult : AN_Result {

	private string _accessToken = string.Empty;
	private string _refreshToken = string.Empty;
	private string _tokenType = string.Empty;
	private long _expiresIn = 0L;
	private string _error = string.Empty;

	public AN_RefreshTokenResult(bool result, string error) : base(result) {
		_error = error;
	}

	public AN_RefreshTokenResult(bool result, string accessToken, string refreshToken, string tokenType, long expiresIn) : base(result) {
		_accessToken = accessToken;
		_refreshToken = refreshToken;
		_tokenType = tokenType;
		_expiresIn = expiresIn;
	}
	
	public string AccessToken {
		get {
			return _accessToken;
		}
	}

	public string RefreshToken {
		get {
			return _refreshToken;
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

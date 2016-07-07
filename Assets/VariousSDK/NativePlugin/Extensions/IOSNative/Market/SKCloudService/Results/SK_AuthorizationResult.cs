using UnityEngine;
using System.Collections;

public class SK_AuthorizationResult : ISN_Result {

	private SK_CloudServiceAuthorizationStatus _AuthorizationStatus = SK_CloudServiceAuthorizationStatus.NotDetermine;

	public SK_AuthorizationResult(SK_CloudServiceAuthorizationStatus status):base(true) {
		_AuthorizationStatus = status;
	}

	public SK_CloudServiceAuthorizationStatus AuthorizationStatus {
		get {
			return _AuthorizationStatus;
		}
	}
}

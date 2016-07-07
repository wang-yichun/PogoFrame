using UnityEngine;
using System.Collections;

public class SK_RequestCapabilitieResult : ISN_Result {

	private SK_CloudServiceCapability _Capability = SK_CloudServiceCapability.None;

	public SK_RequestCapabilitieResult(SK_CloudServiceCapability capability):base(true) {
		_Capability = capability;
	}

	public SK_RequestCapabilitieResult(string errorData):base(errorData) {

	}


	public SK_CloudServiceCapability Capability {
		get {
			return _Capability;
		}
	}
}

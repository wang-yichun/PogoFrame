#if SDK_PlayFabSDK

using UnityEngine;
using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;

public class TestPlayFab : MonoBehaviour
{
	public string titleId = "D84D";

	// Use this for initialization
	void Start ()
	{
		PlayFabSettings.TitleId = titleId;
		LoginWithPlayFabRequest request = new LoginWithPlayFabRequest ();
		request.Username = "ethan012";
		request.Password = "ethan012";
			
		PlayFabClientAPI.LoginWithPlayFab (request, OnLoginCallback, OnErrorCallback);
	}

	public string playFabId;

	void OnLoginCallback (LoginResult result)
	{
		Debug.Log (JsonConvert.SerializeObject (result));

		playFabId = result.PlayFabId;

		GetUserCombinedInfoRequest request = new GetUserCombinedInfoRequest ();
		request.PlayFabId = playFabId;
		request.GetAccountInfo = true;
		request.GetVirtualCurrency = true;

		PlayFabClientAPI.GetUserCombinedInfo (request, OnGetUserCombinedInfoCallback, OnErrorCallback);
	}

	void OnGetUserCombinedInfoCallback (GetUserCombinedInfoResult result)
	{
		Debug.Log (JsonConvert.SerializeObject (result));
	}

	void OnErrorCallback (PlayFabError error)
	{
		Debug.Log (JsonConvert.SerializeObject (error));
	}
}

#endif
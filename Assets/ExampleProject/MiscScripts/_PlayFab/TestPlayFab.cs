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
		request.InfoRequestParameters = new GetPlayerCombinedInfoRequestParams () {
			GetUserAccountInfo = true
		};
			
		PlayFabClientAPI.LoginWithPlayFab (request, OnLoginCallback, OnErrorCallback_Login);
	}

	void OnLoginCallback (LoginResult result)
	{
		Debug.Log (JsonConvert.SerializeObject (result));
	}

	void OnErrorCallback_Login (PlayFabError error)
	{
		Debug.Log (JsonConvert.SerializeObject (error));

		GetUserCombinedInfoRequest request = new GetUserCombinedInfoRequest ();
		request.PlayFabId = "67529B907D4E74A";
		request.GetAccountInfo = true;
		request.GetVirtualCurrency = true;

		PlayFabClientAPI.GetUserCombinedInfo (request, OnGetUserCombinedInfoCallback, OnErrorCallback_Login);
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

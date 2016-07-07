using UnityEngine;
using System.Collections;

public class CSharpAPIHelper : MonoBehaviour {



	public void ConnectToPlaySertivce() {
		//listen for GooglePlayConnection events
		GooglePlayConnection.ActionPlayerConnected += OnPlayerConnected;
		GooglePlayConnection.ActionPlayerDisconnected += OnPlayerDisconnected;


		
		if(GooglePlayConnection.State == GPConnectionState.STATE_CONNECTED) {
			//checking if player already connected
			OnPlayerConnected ();
		}  else {
			Debug.Log("Connecting....");
			GooglePlayConnection.instance.Connect();
		}




	}


	private void OnPlayerConnected() {

		//Sedning Event back to JS
		gameObject.SendMessage("PlayerConnectd");
	}

	private void OnPlayerDisconnected() {

		//Sedning Event back to JS
		gameObject.SendMessage("PlayerDisconected");
	}
}

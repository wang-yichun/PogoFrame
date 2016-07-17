using UnityEngine;
using System.Collections;

public class SA_FriendUI : MonoBehaviour {

	private string _pId = string.Empty;

	public GameObject avatar;
	public SA_Label playerId;
	public SA_Label playerName;
	
	private Texture defaulttexture;
	
	void Awake() {
		defaulttexture = avatar.GetComponent<Renderer>().material.mainTexture;
	}
	
	public void SetFriendId(string pId) {
		_pId = pId;

		playerId.text = "";
		playerName.text = "";

		avatar.GetComponent<Renderer>().material.mainTexture = defaulttexture;
		
		
		GooglePlayerTemplate player = GooglePlayManager.instance.GetPlayerById(pId);
		if(player != null) {
			playerId.text = "Player Id: " + _pId;
			playerName.text = "Name: " + player.name;
			
			if(player.icon != null) {
				avatar.GetComponent<Renderer>().material.mainTexture = player.icon;
			}
			
		}
		
	}

	public void PlayWithFried() {
		GooglePlayRTM.instance.FindMatch(1, 1, _pId);
	}

	void FixedUpdate() {
		if(_pId != string.Empty) {
			SetFriendId(_pId);
		}
	}
}

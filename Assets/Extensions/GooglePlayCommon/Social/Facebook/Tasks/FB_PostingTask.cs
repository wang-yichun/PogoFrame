using UnityEngine;
using System;
using System.Collections;

public class FB_PostingTask : AsyncTask {

	private string _toId = "";
	private string _link = "";
	private string _linkName = "";
	private string _linkCaption = "";
	private string _linkDescription = "";
	private string _picture = "";
	private string _actionName = "";
	private string _actionLink = "";
	private string _reference = "";


	public static FB_PostingTask Cretae() {
		return	new GameObject("FB_PostingTask").AddComponent<FB_PostingTask>();
	}


	public void FeedShare (
		string toId = "",
		string link = "",
		string linkName = "",
		string linkCaption = "",
		string linkDescription = "",
		string picture = "",
		string actionName = "",
		string actionLink = "",
		string reference = ""
		) {

		_toId = toId;
		_link = link;
		_linkName = linkName;
		_linkCaption = linkCaption;
		_linkDescription = linkDescription;
		_picture = picture;
		_actionName = actionName;
		_actionLink = actionLink;
		_reference = reference;


		if(SPFacebook.instance.IsInited) {
			OnFBInited();
		} else {
			SPFacebook.OnInitCompleteAction += OnFBInited;
			SPFacebook.instance.Init();
		}


	}


	private void OnFBInited() {
		SPFacebook.OnInitCompleteAction -= OnFBInited;
		if(SPFacebook.instance.IsLoggedIn) {
			OnFBAuth(null);
		} else {
			SPFacebook.OnAuthCompleteAction += OnFBAuth;
			SPFacebook.instance.Login();
		}
	}


	private void OnFBAuth(FB_Result result) {

		SPFacebook.OnAuthCompleteAction -= OnFBAuth;

		if(SPFacebook.Instance.IsLoggedIn) {

			SPFacebook.instance.FB.FeedShare(_toId,
			                         _link,
			                         _linkName,
			                         _linkCaption,
			                         _linkDescription,
			                         _picture,
			                         _actionName,
			                         _actionLink,
			                         _reference);

		} else {
			FB_PostResult postResult =  new FB_PostResult("", "Auth failed");
			SPFacebook.Instance.PostCallback(postResult);
		}

		Destroy(gameObject);

	}
	

}

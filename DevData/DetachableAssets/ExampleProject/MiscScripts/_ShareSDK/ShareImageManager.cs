#if SDK_SHARESDK

using UnityEngine;
using System.Collections;
using cn.sharesdk.unity3d;

public class ShareImageManager : MonoBehaviour
{
	private ShareSDK ssdk;
	public static ShareImageManager Instance;

	void Awake ()
	{
		Instance = this;
	}

	void Start ()
	{
#if !UNITY_EDITOR
		ssdk = transform.GetComponent<ShareSDK> ();
		ssdk.shareHandler = ShareResultHandler;
		ssdk.authHandler = AuthResultHandler;
		ssdk.showUserHandler = GetUserInfoResultHandler;
#endif
	}

	void OnGUI ()
	{
		GUILayout.Space (100f);
		if (GUILayout.Button ("分享", GUILayout.Width (300), GUILayout.Height (100))) {
			TestShareWebPage ();
		}
	}
	// Update is called once per frame
	public void TestShareWebPage ()
	{
		print ("执行下");
#if !UNITY_EDITOR
		ssdk.GetUserInfo (PlatformType.WeChat);
		ssdk.Authorize (PlatformType.WeChat);
		ssdk.ShowShareContentEditor (PlatformType.WeChat,GetShareContent());
#endif
	}

	public ShareContent GetShareContent ()
	{
		ShareContent content = new ShareContent ();
		content.SetTitle ("test title");
//	//	content.SetImagePath
//		content.SetDesc ("test description");
//		content.SetUrl ("http://www.baidu.com");
//		content.SetContentType (ContentType.Webpage);
		content.SetImageUrl ("https://f1.webshare.mob.com/code/demo/img/1.jpg");
//		content.SetTitle ("test title");
//		content.SetDesc ("test description");
//		content.SetUrl ("http://sharesdk.cn");
		content.SetContentType (ContentType.Image);
		return content;
	}
	//以下为回调的定义:
	void GetUserInfoResultHandler (int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (state == ResponseState.Success) {
			print ("get user info result :");
			print (MiniJSON.jsonEncode (result));
		} else if (state == ResponseState.Fail) {
			print ("fail! error code = " + result ["error_code"] + "; error msg = " + result ["error_msg"]);
		} else if (state == ResponseState.Cancel) {
			print ("cancel !");
		}
	}

	//以下为回调的定义:
	void ShareResultHandler (int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (state == ResponseState.Success) {
			print ("share result :");
			print (MiniJSON.jsonEncode (result));
		} else if (state == ResponseState.Fail) {
			print ("fail! error code = " + result ["error_code"] + "; error msg = " + result ["error_msg"]);
		} else if (state == ResponseState.Cancel) {
			print ("cancel !");
		}
	}
	//以下为回调的定义:
	void AuthResultHandler (int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		if (state == ResponseState.Success) {
			print ("authorize success !");
		} else if (state == ResponseState.Fail) {
			print ("fail! error code = " + result ["error_code"] + "; error msg = " + result ["error_msg"]);
		} else if (state == ResponseState.Cancel) {
			print ("cancel !");
		}
	}
}
#endif
#if UNITY_ADS
using UnityEngine;
using System.ComponentModel;
using UnityEngine.Advertisements;

public partial class SROptions
{
	[Category ("Unity Ads")]
	public string gameId {
		get { return Advertisement.gameId; }
		private set { }
	}

	[Category ("Unity Ads")]
	public bool IsReady { 
		get { return Advertisement.IsReady (); } 
		private set { }
	}

	[Category ("Unity Ads"), DisplayName ("展示")] 
	public void UnityAds_SimpleShow ()
	{
		if (Advertisement.IsReady ()) {
			Advertisement.Show ();
		}
	}

	[Category ("Unity Ads"), DisplayName ("展示&检查结果")] 
	public void UnityAds_ShowResult ()
	{
		if (Advertisement.IsReady ()) {
			var options = new ShowOptions {
				resultCallback = HandleShowResult
			};
			Advertisement.Show ();
		}
	}

	private void HandleShowResult (ShowResult result)
	{
		switch (result) {
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
			//
			// YOUR CODE TO REWARD THE GAMER
			// Give coins etc.
			fakeGolds++;
			break;
		case ShowResult.Skipped:
			Debug.Log ("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError ("The ad failed to be shown.");
			break;
		}
	}

	int fakeGolds;

	[Category ("Unity Ads - Fake Currency"), DisplayName ("Golds")]
	public int FakeGolds {
		get;
		set;
	}
}
#endif
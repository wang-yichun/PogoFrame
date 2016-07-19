#if SDK_Vungle
using UnityEngine;
using System.ComponentModel;
using UnityEngine.Advertisements;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

public partial class SROptions
{
	[Category ("Vungle"), DisplayName ("初始化")] 
	public void Vungle_Init ()
	{
		Debug.Log ("Initializing the Vungle SDK");

		Vungle.onAdStartedEvent += Vungle_onAdStartedEvent;
		Vungle.onAdFinishedEvent += Vungle_onAdFinishedEvent;
		Vungle.adPlayableEvent += Vungle_adPlayableEvent;
		Vungle.init ("5789f6e5b903546d23000015", "578a02ad0f48d5260d000059");
	}

	void Vungle_onAdStartedEvent ()
	{
		Debug.Log ("Vungle_onAdStartedEvent");
	}

	void Vungle_adPlayableEvent (bool obj)
	{
		Debug.Log ("Vungle_adPlayableEvent: " + obj);
	}

	void Vungle_onAdFinishedEvent (AdFinishedEventArgs obj)
	{
		Debug.Log ("Vungle_onAdFinishedEvent: " + JsonConvert.SerializeObject (obj, Formatting.Indented));
	}

	[Category ("Vungle"), DisplayName ("展示")] 
	public void Vungle_ShowAd ()
	{
		Dictionary<string, object> options = new Dictionary<string, object> ();
		Vungle.playAdWithOptions (options);
	}

}
#endif
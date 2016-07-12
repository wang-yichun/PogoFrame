#define SDK_JOYINGMOBI

#if SDK_JOYINGMOBI
using UnityEngine;
using System.ComponentModel;
using UnityEngine.Advertisements;
using pogorock.Joying;

public partial class SROptions
{
	[Category ("JoyingMobi"), DisplayName ("初始化")] 
	public void Joying_InitAppID ()
	{
		Joying_Utility.Instance.InitAppID ("3ca89e8b4b868b6f", "9b95a47c62a3b4b1");
	}

	[Category ("JoyingMobi"), DisplayName ("准备好?")] 
	public void Joying_VideoHasCanPlayVideo ()
	{
		Joying_Utility.instance.VideoHasCanPlayVideo ();
	}

//	public void Joying
}
#endif
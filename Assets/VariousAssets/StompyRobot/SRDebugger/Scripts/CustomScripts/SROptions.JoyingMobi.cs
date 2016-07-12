//#define SDK_JOYINGMOBI

#if SDK_JOYINGMOBI
using UnityEngine;
using System.ComponentModel;
using UnityEngine.Advertisements;
using pogorock.Joying;

public partial class SROptions
{
	[Category ("JoyingMobi"), DisplayName ("InitAppID")] 
	public void Joying_InitAppID ()
	{
		Joying_Utility.Instance.InitAppID ("3ca89e8b4b868b6f", "9b95a47c62a3b4b1");
	}
}
#endif
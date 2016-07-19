
using UnityEngine;
using System.ComponentModel;
using UnityEngine.Advertisements;
using pogorock.Joying;
using pogorock;

public partial class SROptions
{
	[Category ("PogoAdsX"), DisplayName ("初始化")] 
	public void PogoAdsX_Init ()
	{
		PogoAdsX.Instance.loadConfigFile ();
	}
}

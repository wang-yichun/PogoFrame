using UnityEngine;
using System.ComponentModel;
using pogorock;

public partial class SROptions
{
	[Category ("PogoAdsX"), DisplayName ("初始化")] 
	public void PogoAdsX_Init ()
	{
		PogoAdsX.Instance.Init ();
	}

	[Category ("PogoAdsX")]
	public string PogoAdsX_IsReady { 
		get { return PogoAdsX.Instance.IsReady () ?? "UNREADY"; } 
		private set { }
	}

	[Category ("PogoAdsX"), DisplayName ("播放广告")] 
	public void PogoAdsX_Show ()
	{
		string key = PogoAdsX.Instance.IsReady ();
		PogoAdsX.Instance.Show (key);

		Debug.Log ("播放广告: " + key);
	}
}

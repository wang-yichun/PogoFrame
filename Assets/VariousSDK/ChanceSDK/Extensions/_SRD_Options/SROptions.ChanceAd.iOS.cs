#if SDK_CHANCEAD


using UnityEngine;
using System.ComponentModel;
using UnityEngine.Advertisements;
using pogorock.ChanceAd;

public partial class SROptions
{
	#if UNITY_IOS
	
	[Category ("ChanceAd UNITY_IOS"), DisplayName ("查询是否有广告")] 
	public void ChanceAd_TestQueryVideoADIOS ()
	{
		Chance_Utility.Instance.QueryVideoAD();
	}
	[Category ("ChanceAd UNITY_IOS"), DisplayName ("播放广告")] 
	public void ChanceAd_TestPlayVideoADIOS ()
	{
		Chance_Utility.Instance.PlayVideoAD();
	}
	[Category ("ChanceAd UNITY_IOS"), DisplayName ("载入广告")] 
	public void ChanceAd_TestLoadCSVideoADIOS ()
	{
		Chance_Utility.Instance.LoadCSVideoAD();
	}
	#endif
}
#endif
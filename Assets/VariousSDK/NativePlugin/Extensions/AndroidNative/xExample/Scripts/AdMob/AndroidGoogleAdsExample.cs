////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;

public class AndroidGoogleAdsExample : MonoBehaviour {



	
	//replace with your ids
	private const string MY_BANNERS_AD_UNIT_ID		 = "ca-app-pub-6101605888755494/1824764765"; 
	private const string MY_INTERSTISIALS_AD_UNIT_ID =  "ca-app-pub-6101605888755494/3301497967"; 
	private const string MY_REWARDED_VIDEO_AD_UNIT_ID =  "ca-app-pub-6101605888755494/4996523169";
	
	private GoogleMobileAdBanner banner1;
	private GoogleMobileAdBanner banner2;

	private bool IsInterstisialsAdReady = false;
	private bool IsRewardedVideoAdReady = false;

	public DefaultPreviewButton ShowIntersButton;
	public DefaultPreviewButton ShowRewardedVideoButton;

	public DefaultPreviewButton[] b1CreateButtons;
	public DefaultPreviewButton b1Hide;
	public DefaultPreviewButton b1Show;
	public DefaultPreviewButton b1Refresh;
	public DefaultPreviewButton ChangePost1;
	public DefaultPreviewButton ChangePost2;
	public DefaultPreviewButton b1Destroy;


	public DefaultPreviewButton[] b2CreateButtons;
	public DefaultPreviewButton b2Hide;
	public DefaultPreviewButton b2Show;
	public DefaultPreviewButton b2Refresh;
	public DefaultPreviewButton b2Destroy;



	//--------------------------------------
	// INITIALIZE
	//--------------------------------------



	void Start() {

		AndroidAdMob.Client.Init(MY_BANNERS_AD_UNIT_ID);

		//If yoi whant to use Interstisial ad also, you need to set additional ad unin id for Interstisial as well
		AndroidAdMob.Client.SetInterstisialsUnitID(MY_INTERSTISIALS_AD_UNIT_ID);
		//If yoi whant to use Rewarded Video Ads also, you need to set additional ad unin id for Rewarded Video as well
		AndroidAdMobController.Instance.SetRewardedVideoAdUnitID(MY_REWARDED_VIDEO_AD_UNIT_ID);
		
		//Optional, add data for better ad targeting
		AndroidAdMob.Client.SetGender(GoogleGender.Male);
		AndroidAdMob.Client.AddKeyword("game");
		AndroidAdMob.Client.SetBirthday(1989, AndroidMonth.MARCH, 18);
		AndroidAdMob.Client.TagForChildDirectedTreatment(false);

		//Causes a device to receive test ads. The deviceId can be obtained by viewing the logcat output after creating a new ad
		//AndroidAdMobController.instance.AddTestDevice("6B9FA8031AEFDC4758B7D8987F77A5A6");

		AndroidAdMob.Client.OnInterstitialLoaded += OnInterstisialsLoaded; 
		AndroidAdMob.Client.OnInterstitialOpened += OnInterstisialsOpen;

		AndroidAdMobController.Instance.OnRewardedVideoLoaded += HandleOnRewardedVideoLoaded;
		AndroidAdMobController.Instance.OnRewardedVideoAdClosed += HandleOnRewardedVideoAdClosed;

		//listening for InApp Event
		//You will only receive in-app purchase (IAP) ads if you specifically configure an IAP ad campaign in the AdMob front end.
		AndroidAdMob.Client.OnAdInAppRequest += OnInAppRequest;
	}

	void HandleOnRewardedVideoAdClosed ()
	{
		IsRewardedVideoAdReady = false;
	}

	void HandleOnRewardedVideoLoaded ()
	{
		IsRewardedVideoAdReady = true;
	}


	private void StartInterstitialAd() {
		AndroidAdMob.Client.StartInterstitialAd ();
	}

	private void LoadInterstitialAd() {
		AndroidAdMob.Client.LoadInterstitialAd ();
	}

	private void ShowInterstitialAd() {
		AndroidAdMob.Client.ShowInterstitialAd ();
	}

	private void LoadRewardedVideoAd () {
		AndroidAdMobController.Instance.LoadRewardedVideo();
	}

	private void ShowRewardedVideoAd () {
		AndroidAdMobController.Instance.ShowRewardedVideo();
	}

	private void CreateBannerCustomPos() {
		banner1 = AndroidAdMob.Client.CreateAdBanner(300, 100, GADBannerSize.BANNER);
	}

	private void CreateBannerUpperLeft() {
		banner1 = AndroidAdMob.Client.CreateAdBanner(TextAnchor.UpperLeft, GADBannerSize.BANNER);
	}

	private void CreateBannerUpperCneter() {
		banner1 = AndroidAdMob.Client.CreateAdBanner(TextAnchor.UpperCenter, GADBannerSize.BANNER);
	}

	private void CreateBannerBottomLeft() {
		banner1 = AndroidAdMob.Client.CreateAdBanner(TextAnchor.LowerLeft, GADBannerSize.BANNER);
	}

	private void CreateBannerBottomCenter() {
		banner1 = AndroidAdMob.Client.CreateAdBanner(TextAnchor.LowerCenter, GADBannerSize.BANNER);
	}

	private void CreateBannerBottomRight() {
		banner1 = AndroidAdMobController.instance.CreateAdBanner(TextAnchor.LowerRight, GADBannerSize.BANNER);
	}

	private void B1Hide() {
		banner1.Hide();
	}


	private void B1Show() {
		banner1.Show();
	}

	private void B1Refresh() {
		banner1.Refresh();
	}

	private void B1Destrouy() {
		AndroidAdMob.Client.DestroyBanner(banner1.id);
		banner1 = null;
	}


	private void SmartTOP() {
		banner2 = AndroidAdMob.Client.CreateAdBanner(TextAnchor.UpperCenter, GADBannerSize.SMART_BANNER);
	}

	private void SmartBottom() {
		banner2 = AndroidAdMob.Client.CreateAdBanner(TextAnchor.LowerCenter, GADBannerSize.SMART_BANNER);
	}

	
	private void B2Hide() {
		banner2.Hide();
	}
	
	
	private void B2Show() {
		banner2.Show();
	}
	
	private void B2Refresh() {
		banner2.Refresh();
	}
	
	private void B2Destrouy() {
		AndroidAdMob.Client.DestroyBanner(banner2.id);
		banner2 = null;
	}

	private void ChnagePostToMiddle() {
		banner1.SetBannerPosition(TextAnchor.MiddleCenter);
	}

	private void ChangePostRandom() {
		banner1.SetBannerPosition(Random.Range(0, Screen.width), Random.Range(0, Screen.height));
	}




	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------
	


	void FixedUpdate() {
		if(IsInterstisialsAdReady) {
			ShowIntersButton.EnabledButton();
		} else {
			ShowIntersButton.DisabledButton();
		}

		if (IsRewardedVideoAdReady) {
			ShowRewardedVideoButton.EnabledButton();
		} else {
			ShowRewardedVideoButton.DisabledButton();
		}

		if(banner1 != null) {
			foreach(DefaultPreviewButton pb in b1CreateButtons) {
				pb.DisabledButton();
			}

			b1Destroy.EnabledButton();

			if(banner1.IsLoaded) {
				b1Refresh.EnabledButton();
				ChangePost1.EnabledButton();
				ChangePost2.EnabledButton();
				if(banner1.IsOnScreen) {
					b1Hide.EnabledButton();
					b1Show.DisabledButton();
				} else {
					b1Hide.DisabledButton();
					b1Show.EnabledButton();
				}
			} else { 
				b1Refresh.DisabledButton();
				ChangePost1.DisabledButton();
				ChangePost2.DisabledButton();
				b1Hide.DisabledButton();
				b1Show.DisabledButton();
			}



		} else {
			foreach(DefaultPreviewButton pb in b1CreateButtons) {
				pb.EnabledButton();
			}

			b1Hide.DisabledButton();
			b1Show.DisabledButton();
			b1Refresh.DisabledButton();
			b1Destroy.DisabledButton();
		}





		if(banner2 != null) {
			foreach(DefaultPreviewButton pb in b2CreateButtons) {
				pb.DisabledButton();
			}
			
			b2Destroy.EnabledButton();
			
			if(banner2.IsLoaded) {
				b2Refresh.EnabledButton();
				if(banner2.IsOnScreen) {
					b2Hide.EnabledButton();
					b2Show.DisabledButton();
				} else {
					b2Hide.DisabledButton();
					b2Show.EnabledButton();
				}
			} else { 
				b2Refresh.DisabledButton();
				b2Hide.DisabledButton();
				b2Show.DisabledButton();
			}
			
			
			
		} else {
			foreach(DefaultPreviewButton pb in b2CreateButtons) {
				pb.EnabledButton();
			}
			
			b2Hide.DisabledButton();
			b2Show.DisabledButton();
			b2Refresh.DisabledButton();
			b2Destroy.DisabledButton();
		}


	}
	
	//--------------------------------------
	//  GET/SET
	//--------------------------------------
	
	//--------------------------------------
	//  EVENTS
	//--------------------------------------

	private void OnInterstisialsLoaded() {
		IsInterstisialsAdReady = true;
	}

	private void OnInterstisialsOpen() {
		IsInterstisialsAdReady = false;
	}

	private void OnInAppRequest(string productId) {

		AN_PoupsProxy.showMessage ("In App Request", "In App Request for product Id: " + productId + " received");


		//Then you should perfrom purchase  for this product id, using this or another game billing plugin
		//Once the purchase is complete, you should call RecordInAppResolution with one of the constants defined in GADInAppResolution:

		AndroidAdMob.Client.RecordInAppResolution(GADInAppResolution.RESOLUTION_SUCCESS);

	}


	
	//--------------------------------------
	//  PRIVATE METHODS
	//--------------------------------------
	
	//--------------------------------------
	//  DESTROY
	//--------------------------------------

}

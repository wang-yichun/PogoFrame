////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////


using UnityEngine;
using System.Collections;

public class FacebookAnalyticsExample : MonoBehaviour {
	




	public void ActivateApp() {

		//An app is being activated, typically in the AppDelegate's applicationDidBecomeActive.
		SPFacebookAnalytics.ActivateApp ();
	}


	public void AchievedLevel() {
		//The user has achieved a level in the app.
		SPFacebookAnalytics.AchievedLevel (1);
	}


	public void AddedPaymentInfo() {
		//The user has entered their payment info.
		SPFacebookAnalytics.AddedPaymentInfo (true);
	}


	public void AddedToCart() {
		//The user has added an item to their cart. 
		SPFacebookAnalytics.AddedToCart(54.23f, "HDFU-8452", "shoes", "USD");
	}


	public void AddedToWishlist() {
		//The user has added an item to their wishlist. 
		SPFacebookAnalytics.AddedToWishlist(54.23f, "HDFU-8452", "shoes", "USD");
	}

	public void CompletedRegistration() {
		//A user has completed registration with the app.
		//Facebook, Email, Twitter, etc.

		SPFacebookAnalytics.CompletedRegistration("Email");
	}

	public void InitiatedCheckout() {
		//The user has entered the checkout process. The valueToSum passed to logEvent should be the total price in the cart.
		SPFacebookAnalytics.InitiatedCheckout(54.23f, 3, "HDFU-8452", "shoes", true, "USD");
	}
	

	public void Purchased() {
		//The user has completed a purchase
		SPFacebookAnalytics.Purchased (54.23f, 3, "shoes", "HDFU-8452", "USD");
	}

	public void Rated() {
		//The user has rated an item in the app.
		SPFacebookAnalytics.Rated (4, "HDFU-8452", "shoes", 5);
	}


	public void Searched() {
		//A user has performed a search within the app.
		SPFacebookAnalytics.Searched ("shoes", "HD", true);
	}

	public void SpentCredits() {
		//The user has spent app credits
		SPFacebookAnalytics.SpentCredits (120f, "shoes", "HDFU-8452");
	}

	public void UnlockedAchievement() {
		//The user has unlocked an achievement in the app.
		SPFacebookAnalytics.UnlockedAchievement ("ShoeMan");
	}


	public void ViewedContent() {
		//A user has viewed a form of content in the app.
		SPFacebookAnalytics.ViewedContent (54.23f, "shoes", "HDFU-8452", "USD");
	}

	
}

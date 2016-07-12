////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GPaymnetManagerExample : MonoBehaviour {

	private static bool _isInited = false;

	//--------------------------------------
	//  INITIALIZE
	//--------------------------------------
	

	public const string ANDROID_TEST_PURCHASED = "android.test.Purchased";

	public const string ANDROID_TEST_CANCELED = "android.test.canceled";
	public const string ANDROID_TEST_REFUNDED = "android.test.refunded";
	public const string ANDROID_TEST_ITEM_UNAVAILABLE = "android.test.item_unavailable";



	public static void init() {


		//Filling product list

		//When you will add your own proucts you can skip this code section of you already have added
		//your products ids under the editor setings menu
		AndroidInAppPurchaseManager.Client.AddProduct(ANDROID_TEST_PURCHASED);
		AndroidInAppPurchaseManager.Client.AddProduct(ANDROID_TEST_CANCELED);
		AndroidInAppPurchaseManager.Client.AddProduct(ANDROID_TEST_REFUNDED);
		AndroidInAppPurchaseManager.Client.AddProduct(ANDROID_TEST_ITEM_UNAVAILABLE);



		string MyProdcutSKU = "my.prod.id";
		AndroidInAppPurchaseManager.Client.AddProduct(MyProdcutSKU);

		//or
		GoogleProductTemplate tpl = new GoogleProductTemplate();
		tpl.SKU = MyProdcutSKU;
		AndroidInAppPurchaseManager.Client.AddProduct(tpl);


		//listening for Purchase and consume events
		AndroidInAppPurchaseManager.ActionProductPurchased += OnProductPurchased;  
		AndroidInAppPurchaseManager.ActionProductConsumed  += OnProductConsumed;


		//listening for store initialising finish
		AndroidInAppPurchaseManager.ActionBillingSetupFinished += OnBillingConnected;
	

		//you may use loadStore function without parameter if you have filled base64EncodedPublicKey in plugin settings
		AndroidInAppPurchaseManager.Client.Connect();

		//or You can pass base64EncodedPublicKey using scirption:
		//AndroidInAppPurchaseManager.instance.loadStore(YOU_BASE_64_KEY_HERE);



	}

	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------


	public static void Purchase(string SKU) {
		AndroidInAppPurchaseManager.Client.Purchase (SKU);
	}

	public static void consume(string SKU) {
		AndroidInAppPurchaseManager.Client.Consume (SKU);
	}

	//--------------------------------------
	//  GET / SET
	//--------------------------------------

	public static bool isInited {
		get {
			return _isInited;
		}
	}


	//--------------------------------------
	//  EVENTS
	//--------------------------------------
	
	private static void OnProcessingPurchasedProduct(GooglePurchaseTemplate Purchase) {
		//some stuff for processing product purchse. Add coins, unlock track, etc
	}

	private static void OnProcessingConsumeProduct(GooglePurchaseTemplate Purchase) {
		//some stuff for processing product consume. Reduse tip anount, reduse gold token, etc
	}

	private static void OnProductPurchased(BillingResult result) {


		if(result.IsSuccess) {
			AndroidMessage.Create ("Product Purchased", result.Purchase.SKU+ "\n Full Response: " + result.Purchase.OriginalJson);
			OnProcessingPurchasedProduct (result.Purchase);
		} else {
			AndroidMessage.Create("Product Purchase Failed", result.Response.ToString() + " " + result.Message);
		}

		Debug.Log ("Purchased Responce: " + result.Response.ToString() + " " + result.Message);

	}


	private static void OnProductConsumed(BillingResult result) {

		if(result.IsSuccess) {
			AndroidMessage.Create ("Product Consumed", result.Purchase.SKU + "\n Full Response: " + result.Purchase.OriginalJson);
			OnProcessingConsumeProduct (result.Purchase);
		} else {
			AndroidMessage.Create("Product Cousume Failed", result.Response.ToString() + " " + result.Message);
		}

		Debug.Log ("Cousume Responce: " + result.Response.ToString() + " " + result.Message);
	}
	

	private static void OnBillingConnected(BillingResult result) {
		AndroidInAppPurchaseManager.ActionBillingSetupFinished -= OnBillingConnected;

		if(result.IsSuccess) {
			//Store connection is Successful. Next we loading product and customer purchasing details
			AndroidInAppPurchaseManager.Client.RetrieveProducDetails();
			AndroidInAppPurchaseManager.ActionRetrieveProducsFinished += OnRetrieveProductsFinised;
		} 

		AndroidMessage.Create("Connection Responce", result.Response.ToString() + " " + result.Message);
		Debug.Log ("Connection Responce: " + result.Response.ToString() + " " + result.Message);
	}




	private static void OnRetrieveProductsFinised(BillingResult result) {
		AndroidInAppPurchaseManager.ActionRetrieveProducsFinished -= OnRetrieveProductsFinised;


		if(result.IsSuccess) {
			_isInited = true;
			AndroidMessage.Create("Success", "Billing init complete inventory contains: " + AndroidInAppPurchaseManager.Client.Inventory.Purchases.Count + " products");

			foreach(GoogleProductTemplate tpl in AndroidInAppPurchaseManager.Client.Inventory.Products) {
				Debug.Log(tpl.Title);
				Debug.Log(tpl.OriginalJson);
			}
		} else {
			AndroidMessage.Create("Connection Response", result.Response.ToString() + " " + result.Message);
		}

		Debug.Log ("Connection Response: " + result.Response.ToString() + " " + result.Message);

	}






















}
